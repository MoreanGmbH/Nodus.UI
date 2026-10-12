using System;
using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Nodus.ECS;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;

namespace Nodus.UI {
    [SystemPhase(SystemPhase.Update)]
    public sealed class SetThemeSystem : QuerySystem<SetTheme> {
        protected override void OnUpdate() {
            foreach (var request in Query.Entities.ToEntityList()) {
                if (request.IsNull)
                    continue;
                var themeName = request.GetComponent<SetTheme>().ThemeName;
                bool found = false;
                foreach (var state in Query.Store.Query<ThemeState>().Entities) {
                    ref var current = ref state.GetComponent<ThemeState>();
                    current.ThemeName = themeName;
                    found = true;
                    break;
                }
                if (!found)
                    Query.Store.CreateEntity(new ThemeState { ThemeName = themeName });
                CommandCompletion.Complete(request);
            }
        }
    }

    [SystemPhase(SystemPhase.Update)]
    public sealed class SetLocaleSystem : QuerySystem<SetLocale> {
        protected override void OnUpdate() {
            foreach (var request in Query.Entities.ToEntityList()) {
                if (request.IsNull)
                    continue;
                var code = request.GetComponent<SetLocale>().Code;
                bool found = false;
                foreach (var state in Query.Store.Query<LocaleState>().Entities) {
                    ref var current = ref state.GetComponent<LocaleState>();
                    current.Code = code;
                    found = true;
                    break;
                }
                if (!found)
                    Query.Store.CreateEntity(new LocaleState { Code = code });
                CommandCompletion.Complete(request);
            }
        }
    }

    [SystemPhase(SystemPhase.Update)]
    public sealed class LocaleSystem : QuerySystem<LocaleState> {
        protected override void OnUpdate() {
            if (!LocalizationSettings.InitializationOperation.IsDone)
                return;
            foreach (var entity in Query.Entities) {
                var code = entity.GetComponent<LocaleState>().Code;
                if (string.IsNullOrWhiteSpace(code) || LocalizationSettings.SelectedLocale?.Identifier.Code == code)
                    continue;
                var locale = LocalizationSettings.AvailableLocales.GetLocale(code);
                if (locale != null)
                    LocalizationSettings.SelectedLocale = locale;
            }
        }
    }

    [SystemPhase(SystemPhase.Update)]
    public sealed class ThemeSystem : QuerySystem<RootReference> {
        protected override void OnUpdate() {
            string themeName = null;
            foreach (var entity in Query.Store.Query<ThemeState>().Entities) {
                themeName = entity.GetComponent<ThemeState>().ThemeName;
                break;
            }
            if (string.IsNullOrEmpty(themeName))
                return;
            foreach (var entity in Query.Entities) {
                ref var reference = ref entity.GetComponent<RootReference>();
                if (reference.Root == null || reference.AppliedThemeName == themeName)
                    continue;
                if (reference.Configuration == null || reference.Settings == null)
                    throw new InvalidOperationException("UIConfiguration and PanelSettings are required.");
                var profile = reference.Configuration.FindTheme(themeName);
                if (profile.Theme == null)
                    throw new InvalidOperationException($"Generate the Theme Style Sheet for '{themeName}'.");
                reference.Root.AddToClassList("nodus-root");
                reference.Settings.themeStyleSheet = profile.Theme;
                reference.Root.style.unityFontDefinition = profile.Font
                    ? new StyleFontDefinition(profile.Font)
                    : new StyleFontDefinition(StyleKeyword.Null);
                reference.AppliedThemeName = themeName;
            }
        }
    }

    [SystemPhase(SystemPhase.Update)]
    public sealed class PushViewSystem : QuerySystem<PushView> {
        protected override void OnUpdate() {
            foreach (var request in Query.Entities.ToEntityList()) {
                if (request.IsNull)
                    continue;
                var value = request.GetComponent<PushView>();
                Query.Store.PushView(value.StackId, value.ViewId);
                CommandCompletion.Complete(request);
            }
        }
    }

    [SystemPhase(SystemPhase.Update)]
    public sealed class PopViewSystem : QuerySystem<PopView> {
        protected override void OnUpdate() {
            foreach (var request in Query.Entities.ToEntityList()) {
                if (request.IsNull)
                    continue;
                Query.Store.PopView(request.GetComponent<PopView>().StackId);
                CommandCompletion.Complete(request);
            }
        }
    }

    [SystemPhase(SystemPhase.Update)]
    public sealed class VisibilitySystem : QuerySystem<ElementState> {
        protected override void OnUpdate() {
            foreach (var rootEntity in Query.Store.Query<RootReference>().Entities) {
                var root = rootEntity.GetComponent<RootReference>().Root;
                if (root == null)
                    continue;
                foreach (var entity in Query.Entities) {
                    var state = entity.GetComponent<ElementState>();
                    var element = root.Q<VisualElement>(state.Name);
                    if (element == null)
                        continue;
                    element.style.display = state.Visible ? DisplayStyle.Flex : DisplayStyle.None;
                    element.SetEnabled(state.Enabled);
                }
            }
        }
    }

    [SystemPhase(SystemPhase.Update)]
    public sealed class ViewsSystem : QuerySystem<ViewState> {
        protected override void OnUpdate() {
            foreach (var rootEntity in Query.Store.Query<RootReference>().Entities) {
                var reference = rootEntity.GetComponent<RootReference>();
                if (reference.Root == null)
                    continue;
                if (reference.Configuration == null)
                    throw new InvalidOperationException("UIConfiguration is not assigned.");
                foreach (var stack in reference.Configuration.Stacks)
                    Reconcile(reference.Root, reference.Configuration, stack);
            }
        }

        void Reconcile(VisualElement root, UIConfiguration config, StackDefinition stack) {
            var host = root.Q<VisualElement>(stack.ContainerName);
            if (host == null)
                throw new InvalidOperationException($"UI stack container '{stack.ContainerName}' does not exist.");
            var states = new List<ViewState>();
            foreach (var entity in Query.Entities) {
                if (!entity.Parent.IsNull)
                    continue;
                var state = entity.GetComponent<ViewState>();
                if (state.StackId == stack.Id)
                    states.Add(state);
            }
            states.Sort((a, b) => a.Order.CompareTo(b.Order));
            var validIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var state in states)
                validIds.Add(state.InstanceId);

            for (int i = host.childCount - 1; i >= 0; i--) {
                var child = host[i];
                if (child.ClassListContains("nodus-view-instance") && !validIds.Contains(child.name))
                    child.RemoveFromHierarchy();
            }

            for (int i = 0; i < states.Count; i++) {
                var state = states[i];
                var element = host.Q<VisualElement>(state.InstanceId);
                if (element == null) {
                    var view = config.FindView(state.ViewId);
                    element = new VisualElement { name = state.InstanceId, pickingMode = PickingMode.Ignore };
                    element.AddToClassList("nodus-view-instance");
                    var template = view.Instantiate();
                    template.pickingMode = PickingMode.Ignore;
                    template.style.flexGrow = 1;
                    element.Add(template);
                    host.Add(element);
                }
                element.style.display = stack.Presentation == StackPresentation.Overlay || i == states.Count - 1
                    ? DisplayStyle.Flex : DisplayStyle.None;
                element.BringToFront();
            }
        }
    }

    internal static class CommandCompletion {
        internal static void Complete(Entity entity) {
            if (entity.HasComponent<CommandStatus>()) {
                ref var status = ref entity.GetComponent<CommandStatus>();
                status.Handled = true;
            } else {
                entity.DeleteEntity();
            }
        }
    }
}

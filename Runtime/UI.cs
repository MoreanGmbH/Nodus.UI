using System;
using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.Unity;
using Nodus.ECS;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using FrifloECS.Unity.EntityVisualize;
#endif

namespace Nodus.UI {
    [RequireComponent(typeof(ECSStore))]
    public sealed class UI : MonoBehaviour {
        public static EntityStore Store;
        [SerializeField] public string storeId = "UI";
        [SerializeField] public UIConfiguration configuration;
        [SerializeField] public List<PanelRenderer> renderers = new();
        readonly Dictionary<PanelRenderer, Entity> roots = new();

        void Awake() {
            if (!configuration || configuration.Themes.Count == 0 || !configuration.Themes[0])
                throw new InvalidOperationException("UIConfiguration must contain at least one theme.");

            Store = GetComponent<ECSStore>().EntityStore;
            EcsRegistry.RegisterStore(storeId, Store);
            EcsRegistry.RegisterComponent<CommandStatus>();
            EcsRegistry.RegisterComponent<ThemeState>();
            EcsRegistry.RegisterComponent<LocaleState>();
            EcsRegistry.RegisterComponent<SetLocale>();
            EcsRegistry.RegisterComponent<SetTheme>();
            EcsRegistry.RegisterComponent<ViewState>();
            EcsRegistry.RegisterComponent<PushView>();
            EcsRegistry.RegisterComponent<PopView>();
            EcsRegistry.RegisterComponent<ElementState>();
            Store.CreateEntity(new ThemeState { ThemeName = configuration.Themes[0].name });
            Store.CreateEntity(new LocaleState());

            if (renderers.Count == 0 && TryGetComponent<PanelRenderer>(out var renderer))
                renderers.Add(renderer);
            foreach (var panelRenderer in renderers)
                RegisterRoot(panelRenderer);
        }

#if UNITY_EDITOR
        [SerializeField] public bool visualize = true;
        void Start() {
            if (visualize)
                EntityVisualizer.Register(name, Store);
        }
#endif

        public void RegisterRoot(PanelRenderer renderer) {
            if (renderer == null || roots.ContainsKey(renderer))
                return;
            roots.Add(renderer, Store.CreateEntity(new RootReference { Configuration = configuration }));
            renderer.RegisterUIReloadCallback(OnUIReload);
        }

        void OnUIReload(PanelRenderer renderer, VisualElement root, int version) {
            if (!roots.TryGetValue(renderer, out var entity) || entity.IsNull)
                return;
            entity.AddComponent(new RootReference {
                Root = root,
                Settings = renderer.panelSettings,
                Configuration = configuration
            });
        }

        void OnDestroy() {
            foreach (var renderer in roots.Keys)
                if (renderer)
                    renderer.UnregisterUIReloadCallback(OnUIReload);
            roots.Clear();
            EcsRegistry.UnregisterStore(storeId, GetComponent<ECSStore>().EntityStore);
            if (Store == GetComponent<ECSStore>().EntityStore)
                Store = null;
        }
    }
}

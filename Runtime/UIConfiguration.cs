using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nodus.UI {
    public enum StackPresentation { TopOnly, Overlay }

    [Serializable]
    public struct StackDefinition {
        public string Id;
        public string ContainerName;
        public StackPresentation Presentation;
    }

    [CreateAssetMenu(menuName = "Nodus/UI/Configuration", fileName = "UIConfiguration")]
    public sealed class UIConfiguration : ScriptableObject {
        [FormerlySerializedAs("themes")]
        [SerializeField] public List<ThemeProfile> Themes = new();

        [FormerlySerializedAs("views")]
        [SerializeField] public List<ViewDefinition> Views = new();

        [FormerlySerializedAs("stacks")]
        [SerializeField] public List<StackDefinition> Stacks = new();

        public ThemeProfile FindTheme(string name) {
            ThemeProfile found = null;
            foreach (var theme in Themes) {
                if (theme == null || !string.Equals(theme.name, name, StringComparison.Ordinal))
                    continue;
                if (found != null)
                    throw new InvalidOperationException($"Duplicate UI theme name: {name}");
                found = theme;
            }
            return found != null ? found : throw new InvalidOperationException($"Unknown UI theme: {name}");
        }

        public ViewDefinition FindView(string name) {
            ViewDefinition found = null;
            foreach (var view in Views) {
                if (view == null || !string.Equals(view.name, name, StringComparison.Ordinal))
                    continue;
                if (found != null)
                    throw new InvalidOperationException($"Duplicate UI view name: {name}");
                found = view;
            }
            return found != null ? found : throw new InvalidOperationException($"Unknown UI view: {name}");
        }

#if UNITY_EDITOR
        void OnValidate() {
            // Keep the first entry for each asset name or stack ID.
            // This also removes repeated references to the same asset.
            var names = new HashSet<string>(StringComparer.Ordinal);
            for (var i = 0; i < Themes.Count; i++) {
                var theme = Themes[i];
                if (theme != null && !string.IsNullOrWhiteSpace(theme.name) && names.Add(theme.name))
                    continue;
                Debug.LogWarning($"UIConfiguration '{name}': removing null or duplicate theme '{(theme != null ? theme.name : "null")}'.", this);
                Themes.RemoveAt(i--);
            }

            names.Clear();
            for (var i = 0; i < Views.Count; i++) {
                var view = Views[i];
                if (view != null && !string.IsNullOrWhiteSpace(view.name) && names.Add(view.name))
                    continue;
                Debug.LogWarning($"UIConfiguration '{name}': removing null or duplicate view '{(view != null ? view.name : "null")}'.", this);
                Views.RemoveAt(i--);
            }

            names.Clear();
            for (var i = 0; i < Stacks.Count; i++) {
                var stack = Stacks[i];
                if (!string.IsNullOrWhiteSpace(stack.Id) && names.Add(stack.Id))
                    continue;
                Debug.LogWarning($"UIConfiguration '{name}': removing empty or duplicate stack ID '{stack.Id}'.", this);
                Stacks.RemoveAt(i--);
            }
        }
#endif
    }
}

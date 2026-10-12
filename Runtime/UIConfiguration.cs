using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        [SerializeField] public List<ThemeProfile> Themes = new();
        [SerializeField] public List<VisualTreeAsset> Views = new();
        [SerializeField] public List<StackDefinition> Stacks = new();

        public ThemeProfile FindTheme(string name) {
            ThemeProfile found = null;
            foreach (var theme in Themes) {
                if (!theme || !string.Equals(theme.name, name, StringComparison.Ordinal))
                    continue;
                if (found)
                    throw new InvalidOperationException($"Duplicate UI theme name: {name}");
                found = theme;
            }
            return found ? found : throw new InvalidOperationException($"Unknown UI theme: {name}");
        }

        public VisualTreeAsset FindView(string name) {
            VisualTreeAsset found = null;
            foreach (var view in Views) {
                if (!view || !string.Equals(view.name, name, StringComparison.Ordinal))
                    continue;
                if (found)
                    throw new InvalidOperationException($"Duplicate UI view name: {name}");
                found = view;
            }
            return found ? found : throw new InvalidOperationException($"Unknown UI view: {name}");
        }

#if UNITY_EDITOR
        void OnValidate() {
            var names = new HashSet<string>(StringComparer.Ordinal);
            for (var i = 0; i < Themes.Count; i++) {
                if (Themes[i] && names.Add(Themes[i].name))
                    continue;
                Debug.LogWarning($"UIConfiguration '{name}': duplicate or missing theme removed.", this);
                Themes.RemoveAt(i--);
            }

            names.Clear();
            for (var i = 0; i < Views.Count; i++) {
                if (Views[i] && names.Add(Views[i].name))
                    continue;
                Debug.LogWarning($"UIConfiguration '{name}': duplicate or missing view removed.", this);
                Views.RemoveAt(i--);
            }

            names.Clear();
            for (var i = 0; i < Stacks.Count; i++) {
                if (!string.IsNullOrWhiteSpace(Stacks[i].Id) && names.Add(Stacks[i].Id))
                    continue;
                Debug.LogWarning($"UIConfiguration '{name}': duplicate or empty stack removed.", this);
                Stacks.RemoveAt(i--);
            }
        }
#endif
    }
}

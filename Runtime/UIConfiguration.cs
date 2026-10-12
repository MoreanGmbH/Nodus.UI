using System;
using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField] List<ThemeProfile> themes = new();
        [SerializeField] List<ViewDefinition> views = new();
        [SerializeField] List<StackDefinition> stacks = new();
        public IReadOnlyList<ThemeProfile> Themes => themes;
        public IReadOnlyList<StackDefinition> Stacks => stacks;

        public ThemeProfile FindTheme(string id) {
            foreach (var theme in themes)
                if (theme && theme.Id == id)
                    return theme;
            throw new InvalidOperationException($"Unknown UI theme: {id}");
        }

        public ViewDefinition FindView(string id) {
            foreach (var view in views)
                if (view && view.Id == id)
                    return view;
            throw new InvalidOperationException($"Unknown UI view: {id}");
        }
    }
}

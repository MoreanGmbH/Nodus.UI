using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace Nodus.UI {
    [CreateAssetMenu(menuName = "Nodus/UI/Theme Profile", fileName = "Theme")]
    public sealed class ThemeProfile : ScriptableObject {
        // The asset's name is the only theme identifier.
        // Compatibility accessors for older Nodus.UI systems; neither stores a separate ID.
        public string Id => name;
        public string DisplayName => name;

        [Header("Generated UI Toolkit theme")]
        public ThemeStyleSheet Theme;
        public FontAsset Font;

        [Header("Palette")]
        public Color Primary = new(0.137f, 0.608f, 0.827f);
        public Color PrimaryDark = new(0.086f, 0.490f, 0.718f);
        public Color PrimarySoft = new(0.882f, 0.953f, 0.984f);
        public Color Surface = Color.white;
        public Color SurfaceMuted = new(0.941f, 0.961f, 0.973f);
        public Color Text = new(0.145f, 0.216f, 0.275f);
        public Color TextMuted = new(0.431f, 0.494f, 0.529f);
        public Color Border = new(0.824f, 0.871f, 0.898f);
        public Color Danger = new(0.784f, 0.208f, 0.267f);

        [Header("Typography (px)")]
        [Min(9)] public int TextSmall = 12;
        [Min(9)] public int TextBody = 14;
        [Min(9)] public int TextTitle = 20;

        [Header("Shape and spacing (px)")]
        [Min(0)] public int Radius = 12;
        [Min(0)] public int Spacing = 8;
    }
}

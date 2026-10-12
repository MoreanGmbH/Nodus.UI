using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nodus.UI.Editor {
    [CustomEditor(typeof(ThemeProfile))]
    public sealed class ThemeProfileEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            if (GUILayout.Button("Generate Theme Assets"))
                Generate();
        }

        void Generate() {
            var profile = (ThemeProfile)target;
            var path = AssetDatabase.GetAssetPath(profile);
            if (string.IsNullOrEmpty(path) || !path.StartsWith("Assets/", StringComparison.Ordinal)) {
                Debug.LogError("ThemeProfile must be saved under Assets/.");
                return;
            }

            var name = Path.ChangeExtension(path, null);
            var tokensPath = name + ".tokens.uss";
            var themePath = name + ".tss";
            var sb = new StringBuilder();
            sb.AppendLine(":root, .nodus-root {");
            AddColor(sb, "primary", profile.Primary);
            AddColor(sb, "primary-dark", profile.PrimaryDark);
            AddColor(sb, "primary-soft", profile.PrimarySoft);
            AddColor(sb, "surface", profile.Surface);
            AddColor(sb, "surface-muted", profile.SurfaceMuted);
            AddColor(sb, "text", profile.Text);
            AddColor(sb, "text-muted", profile.TextMuted);
            AddColor(sb, "border", profile.Border);
            AddColor(sb, "danger", profile.Danger);
            AddPixel(sb, "font-small", profile.TextSmall);
            AddPixel(sb, "font-body", profile.TextBody);
            AddPixel(sb, "font-title", profile.TextTitle);
            AddPixel(sb, "radius", profile.Radius);
            AddPixel(sb, "space", profile.Spacing);
            sb.AppendLine("}");

            var tss = new StringBuilder();
            tss.AppendLine("@import url(\"unity-theme://default\");");
            tss.AppendLine($"@import url(\"project://database/{tokensPath}\");");
            tss.AppendLine("@import url(\"project://database/Packages/com.nodus.ui/Runtime/Styles/Nodus.uss\");");

            File.WriteAllText(tokensPath, sb.ToString());
            File.WriteAllText(themePath, tss.ToString());
            AssetDatabase.ImportAsset(tokensPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(themePath, ImportAssetOptions.ForceUpdate);

            var theme = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(themePath);
            if (theme == null) {
                Debug.LogError($"Failed to import theme: {themePath}");
                return;
            }
            Undo.RecordObject(profile, "Generate Nodus UI Theme");
            profile.Theme = theme;
            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
            Debug.Log($"Generated {tokensPath} and {themePath}");
        }

        static void AddColor(StringBuilder sb, string key, Color color)
            => sb.AppendLine($"    --nodus-{key}: #{ColorUtility.ToHtmlStringRGBA(color)};");

        static void AddPixel(StringBuilder sb, string key, int value)
            => sb.AppendLine($"    --nodus-{key}: {value}px;");
    }
}

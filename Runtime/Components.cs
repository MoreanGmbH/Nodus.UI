using Friflo.Engine.ECS;
using UnityEngine.UIElements;

namespace Nodus.UI {
    [ComponentKey("Nodus.UI.ThemeState")]
    public struct ThemeState : IComponent {
        public string ThemeId;
    }

    [ComponentKey("Nodus.UI.SetTheme")]
    public struct SetTheme : IComponent {
        public string ThemeId;
    }

    [ComponentKey("Nodus.UI.LocaleState")]
    public struct LocaleState : IComponent {
        public string Code;
    }

    [ComponentKey("Nodus.UI.SetLocale")]
    public struct SetLocale : IComponent {
        public string Code;
    }

    [ComponentKey("Nodus.UI.ViewState")]
    public struct ViewState : IComponent {
        public string StackId;
        public string ViewId;
        public string InstanceId;
        public int Order;
    }

    [ComponentKey("Nodus.UI.PushView")]
    public struct PushView : IComponent {
        public string StackId;
        public string ViewId;
    }

    [ComponentKey("Nodus.UI.PopView")]
    public struct PopView : IComponent {
        public string StackId;
    }

    [ComponentKey("Nodus.UI.ElementState")]
    public struct ElementState : IComponent {
        public string Name;
        public bool Visible;
        public bool Enabled;
    }

    [ComponentKey(null)]
    public struct RootReference : IComponent {
        public VisualElement Root;
        public PanelSettings Settings;
        public UIConfiguration Configuration;
        public string AppliedThemeId;
    }
}

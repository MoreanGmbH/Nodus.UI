using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nodus.UI.Tests {
    public class ThemeTests {
        [Test]
        public void ThemeHasEditableTokens() {
            var profile = ScriptableObject.CreateInstance<ThemeProfile>();
            try {
                Assert.Greater(profile.TextBody, 0);
                Assert.Greater(profile.Radius, -1);
            } finally { Object.DestroyImmediate(profile); }
        }

        [Test]
        public void ConfigurationFindsAssetsByName() {
            var configuration = ScriptableObject.CreateInstance<UIConfiguration>();
            var theme = ScriptableObject.CreateInstance<ThemeProfile>();
            var view = ScriptableObject.CreateInstance<VisualTreeAsset>();
            try {
                theme.name = "MoreanLightTheme";
                view.name = "FeedbackPanel";
                configuration.Themes.Add(theme);
                configuration.Views.Add(view);
                Assert.AreSame(theme, configuration.FindTheme("MoreanLightTheme"));
                Assert.AreSame(view, configuration.FindView("FeedbackPanel"));
            } finally {
                Object.DestroyImmediate(view);
                Object.DestroyImmediate(theme);
                Object.DestroyImmediate(configuration);
            }
        }

        [Test]
        public void ViewStackPushAndPop() {
            var store = new Friflo.Engine.ECS.EntityStore();
            store.PushView("Panels", "FeedbackPanel");
            store.PushView("Panels", "DetailsPanel");
            Assert.AreEqual(2, store.Query<ViewState>().Count);
            Assert.IsTrue(store.PopView("Panels"));
            Assert.AreEqual(1, store.Query<ViewState>().Count);
            Assert.IsTrue(store.PopView("Panels"));
            Assert.IsFalse(store.PopView("Panels"));
        }
    }
}

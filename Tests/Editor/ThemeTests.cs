using NUnit.Framework;
using UnityEngine;

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
        public void ViewStackPushAndPop() {
            var store = new Friflo.Engine.ECS.EntityStore();
            store.PushView("Panels", "Feedback");
            store.PushView("Panels", "Details");
            Assert.AreEqual(2, store.Query<ViewState>().Count);
            Assert.IsTrue(store.PopView("Panels"));
            Assert.AreEqual(1, store.Query<ViewState>().Count);
            Assert.IsTrue(store.PopView("Panels"));
            Assert.IsFalse(store.PopView("Panels"));
        }
    }
}

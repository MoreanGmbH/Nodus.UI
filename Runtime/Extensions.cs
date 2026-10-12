using System;
using Friflo.Engine.ECS;

namespace Nodus.UI {
    public static class Extensions {
        public static Entity PushView(this EntityStore store, string stackId, string viewId) {
            if (string.IsNullOrWhiteSpace(stackId) || string.IsNullOrWhiteSpace(viewId))
                throw new ArgumentException("Stack and view IDs are required.");

            int order = -1;
            foreach (var entity in store.Query<ViewState>().Entities) {
                if (!entity.Parent.IsNull)
                    continue;
                var state = entity.GetComponent<ViewState>();
                if (state.StackId == stackId && state.Order > order)
                    order = state.Order;
            }
            return store.CreateEntity(new ViewState {
                StackId = stackId,
                ViewId = viewId,
                InstanceId = Guid.NewGuid().ToString("N"),
                Order = order + 1
            });
        }

        public static bool PopView(this EntityStore store, string stackId) {
            Entity latest = default;
            int order = int.MinValue;
            foreach (var entity in store.Query<ViewState>().Entities) {
                if (!entity.Parent.IsNull)
                    continue;
                var state = entity.GetComponent<ViewState>();
                if (state.StackId != stackId || state.Order <= order)
                    continue;
                latest = entity;
                order = state.Order;
            }
            if (latest.IsNull)
                return false;
            latest.DeleteEntity();
            return true;
        }

        public static void SetElementState(this EntityStore store, string name, bool visible, bool enabled = true) {
            foreach (var entity in store.Query<ElementState>().Entities) {
                ref var current = ref entity.GetComponent<ElementState>();
                if (current.Name != name)
                    continue;
                current.Visible = visible;
                current.Enabled = enabled;
                return;
            }
            store.CreateEntity(new ElementState { Name = name, Visible = visible, Enabled = enabled });
        }
    }
}

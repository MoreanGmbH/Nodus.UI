using System;
using Nodus.ECS;
using UnityEngine.UIElements;

namespace Nodus.UI {
    [UxmlElement]
    public partial class EntitiesButton : Button {
        [UxmlAttribute] public EntitiesDefinition Entities { get; set; }

        public EntitiesButton() {
            AddToClassList("nodus-button");
            clicked += Execute;
        }

        void Execute() {
            if (!Entities)
                return;
            try { EcsRuntime.Spawn(Entities); }
            catch (Exception exception) { UnityEngine.Debug.LogException(exception); }
        }
    }
}

using System;
using Nodus.ECS;
using UnityEngine.UIElements;
using Entities = Nodus.ECS.Definitions.Entities;

namespace Nodus.UI {
    [UxmlElement]
    public partial class EntitiesButton : Button {
        [UxmlAttribute] public Entities Entities { get; set; }

        public EntitiesButton() {
            AddToClassList("nodus-button");
            clicked += Execute;
        }

        void Execute() {
            if (!Entities) return;
            try { EcsRuntime.Spawn(Entities); }
            catch (Exception exception) { UnityEngine.Debug.LogException(exception); }
        }
    }
}

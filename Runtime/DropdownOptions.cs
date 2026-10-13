using System;
using System.Collections.Generic;
using UnityEngine;
using Entities = Nodus.ECS.Definitions.Entities;

namespace Nodus.UI {
    public enum DropdownSelection { None, Locale, Theme }

    [Serializable]
    public struct DropdownOption {
        [Tooltip("Stable value used by the dropdown and the state system. Never localize this key.")]
        public string Key;

        [Tooltip("Fallback text displayed until the localized string is available.")]
        public string Label;

        [Tooltip("Entry key in the localization table. Leave empty to display Label unchanged.")]
        public string LocalizationKey;

        [Tooltip("Entities action to spawn when this option is selected.")]
        public Entities Action;
    }

    [CreateAssetMenu(menuName = "Nodus/UI/Dropdown Options", fileName = "DropdownOptions")]
    public sealed class DropdownOptions : ScriptableObject {
        [Tooltip("Where the current selected key comes from. None keeps the dropdown's own selection.")]
        public DropdownSelection Selection;

        [Tooltip("Unity Localization string table containing the option labels.")]
        public string Table = "MainTable";

        [Tooltip("Ordered entries. Each key maps to a localized label and a reusable Entities action.")]
        public List<DropdownOption> Items = new();
    }
}

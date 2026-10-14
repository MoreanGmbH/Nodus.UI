using System;
using System.Collections.Generic;
using Nodus.ECS;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

namespace Nodus.UI {
    [UxmlElement]
    public partial class EntitiesDropdown : DropdownField {
        DropdownOptions options;
        StringTable stringTable;
        string requestedLocale;
        IVisualElementScheduledItem update;
        int requestVersion;

        [UxmlAttribute]
        public DropdownOptions Options {
            get => options;
            set {
                if (options == value) return;
                options = value;
                requestVersion++;
                requestedLocale = null;
                stringTable = null;
                if (panel != null) Configure();
            }
        }

        public EntitiesDropdown() {
            RegisterCallback<AttachToPanelEvent>(_ => {
                requestVersion++;
                requestedLocale = null;
                stringTable = null;
                Configure();
                if (update == null) update = schedule.Execute(Tick).Every(100);
                else update.Resume();
                Tick();
            });
            RegisterCallback<DetachFromPanelEvent>(_ => {
                requestVersion++;
                update?.Pause();
            });
            RegisterCallback<ChangeEvent<string>>(OnChanged);
            RegisterCallback<PointerDownEvent>(_ => FitPopup());
        }

        void Configure() {
            var keys = new List<string>();
            if (options != null) {
                foreach (var item in options.Items) {
                    if (string.IsNullOrWhiteSpace(item.Key) || keys.Contains(item.Key)) {
                        Debug.LogWarning($"DropdownOptions '{options.name}' contains an empty or duplicate key.", options);
                        continue;
                    }
                    keys.Add(item.Key);
                }
            }

            choices = keys;
            formatSelectedValueCallback = Label;
            formatListItemCallback = Label;
            if (keys.Count == 0) return;

            var selected = CurrentKey();
            if (string.IsNullOrEmpty(selected) || !keys.Contains(selected))
                selected = keys[0];
            SetValueWithoutNotify(selected);
            FitSelected();
        }

        void OnChanged(ChangeEvent<string> evt) {
            if (!options) return;
            foreach (var item in options.Items) {
                if (item.Key != evt.newValue) continue;
                if (!item.Action) {
                    Debug.LogWarning($"Dropdown option '{item.Key}' has no action.", options);
                    return;
                }
                try { EcsRuntime.Spawn(item.Action); }
                catch (Exception exception) { Debug.LogException(exception); }
                FitSelected();
                return;
            }
        }

        string CurrentKey() {
            if (!options) return value;
            if (options.Selection == DropdownSelection.Locale) {
                if (!LocalizationSettings.InitializationOperation.IsDone) return value;
                return LocalizationSettings.SelectedLocale?.Identifier.Code;
            }
            if (options.Selection == DropdownSelection.Theme) {
                var store = UI.Store;
                if (store != null) {
                    foreach (var entity in store.Query<ThemeState>().Entities)
                        return entity.GetComponent<ThemeState>().ThemeName;
                }
            }
            return value;
        }

        void Tick() {
            if (!Application.isPlaying || panel == null || !options) return;
            if (LocalizationSettings.InitializationOperation.IsDone) {
                var locale = LocalizationSettings.SelectedLocale?.Identifier.Code;
                if (locale != requestedLocale) LoadStrings(locale);
            }
            var key = CurrentKey();
            if (!string.IsNullOrEmpty(key) && choices.Contains(key) && value != key) {
                SetValueWithoutNotify(key);
                FitSelected();
            }
        }

        void LoadStrings(string locale) {
            requestedLocale = locale;
            stringTable = null;
            int version = ++requestVersion;
            if (string.IsNullOrEmpty(locale) || string.IsNullOrWhiteSpace(options.Table)) return;

            var expectedOptions = options;
            var request = LocalizationSettings.StringDatabase.GetTableAsync(options.Table);
            request.Completed += operation => {
                if (version != requestVersion || panel == null || !Application.isPlaying ||
                    options != expectedOptions || requestedLocale != locale) return;
                stringTable = operation.Result;
                if (!string.IsNullOrEmpty(value) && choices.Contains(value))
                    SetValueWithoutNotify(value);
                FitSelected();
            };
        }

        string Label(string key) {
            if (string.IsNullOrEmpty(key) || !options) return key;
            foreach (var item in options.Items) {
                if (item.Key != key) continue;
                var fallback = string.IsNullOrEmpty(item.Label) ? key : item.Label;
                if (string.IsNullOrEmpty(item.LocalizationKey)) return fallback;
                var translated = stringTable?.GetEntry(item.LocalizationKey)?.LocalizedValue;
                return string.IsNullOrWhiteSpace(translated) ? fallback : translated;
            }
            return key;
        }

        float ContentWidth() {
            var input = this.Q<VisualElement>(className: "unity-base-popup-field__input");
            var text = this.Q<TextElement>(className: "unity-base-popup-field__text");
            if (input == null || text == null) return 0;

            float labelWidth = 0;
            foreach (var key in choices) {
                var size = text.MeasureTextSize(Label(key), 0, VisualElement.MeasureMode.Undefined,
                    0, VisualElement.MeasureMode.Undefined);
                labelWidth = Mathf.Max(labelWidth, size.x);
            }

            var inputStyle = input.resolvedStyle;
            return labelWidth + inputStyle.paddingLeft + inputStyle.paddingRight +
                inputStyle.borderLeftWidth + inputStyle.borderRightWidth;
        }

        void FitSelected() {
            schedule.Execute(() => {
                if (panel == null) return;
                var group = parent?.parent;
                if (ClassListContains("nodus-select") && group != null &&
                    group.ClassListContains("nodus-toolbar__settings")) {
                    float width = 0;
                    foreach (var dropdown in group.Query<EntitiesDropdown>(className: "nodus-select").ToList())
                        width = Mathf.Max(width, dropdown.ContentWidth());
                    if (width > 0) group.style.width = Mathf.Ceil(width);
                    return;
                }

                float contentWidth = ContentWidth();
                if (contentWidth > 0) style.width = Mathf.Ceil(contentWidth);
            });
        }

        void FitPopup() {
            schedule.Execute(() => {
                if (panel == null) return;
                var popup = panel.visualTree.Q<VisualElement>(className: GenericDropdownMenu.containerOuterUssClassName);
                if (popup == null) return;
                popup.style.width = StyleKeyword.Auto;
                popup.style.minWidth = resolvedStyle.width;
                popup.style.maxWidth = StyleKeyword.None;
                popup.schedule.Execute(() => {
                    if (panel == null) return;
                    float left = popup.resolvedStyle.left + worldBound.x - popup.worldBound.x;
                    popup.style.left = Mathf.Max(0, left);
                });
            });
        }
    }
}

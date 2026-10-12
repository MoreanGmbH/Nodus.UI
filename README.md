# Nodus.UI

UI Toolkit themes, locale selection, views and stacks driven by ECS.

## Themes

Create a **Theme Profile**, generate its TSS, and add it to `UIConfiguration.Themes`.
The theme's Unity asset name is its identifier (for example `MoreanLightTheme`).
The **first theme** in the list is used initially; reordering the list changes the default.

## Views and stacks

Add **VisualTreeAsset (UXML)** files directly to `UIConfiguration.Views`.
No `ViewDefinition` assets are needed. Each UXML asset name must be unique in the list.

Configure stacks with a unique `Id` and a `ContainerName` identifying a UXML VisualElement.
To display a view, send a request with `ViewId` equal to the **UXML asset name**:

```csharp
Navigation.CreateRequest(new PushView { StackId = "Main", ViewId = "FeedbackPanel" });
```

The view's visual root name inside UXML may differ from its asset name.
`UIConfiguration` removes duplicate list entries in the Editor and lookup also rejects ambiguous names.

## Localization

UI Toolkit `LocalizedString` bindings on UXML elements use native Unity Localization.
Dropdowns can store the theme asset name or the locale code (`en`, `de`), while displaying
localized text with keys such as `Theme.MoreanLightTheme` and `Locale.de`.

## ECS

Put `ECSStore`, `NodusSystemSet` and `Nodus.UI.UI` on a UI prefab with a `PanelRenderer`.
Assign the configuration and root UXML. In the Update group add:
`SetThemeSystem`, `SetLocaleSystem`, `PushViewSystem`, `PopViewSystem`,
`ThemeSystem`, `LocaleSystem`, `VisibilitySystem`, `ViewsSystem`.

Examples:

```csharp
UI.Store.CreateEntity(new SetTheme { ThemeName = "MoreanDarkTheme" });
UI.Store.CreateEntity(new SetLocale { Code = "de" });
UI.Store.CreateEntity(new PushView { StackId = "Main", ViewId = "FeedbackPanel" });
UI.Store.CreateEntity(new PopView { StackId = "Main" });
UI.Store.SetElementState("BackButton", visible: false);
```

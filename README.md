# Nodus.UI

UI Toolkit design system with ECS-driven theme, locale, views, stacks and element visibility.

## Installation

```powershell
git submodule add https://github.com/MoreanGmbH/Nodus.UI.git Packages/com.nodus.ui
```

Requires Unity 6.6, Nodus.ECS, Unity Localization and a Friflo ECSStore. Keep project assets in `Assets/`.

## Designer — theme

1. Create `Assets > Create > Nodus > UI > Theme Profile`. Set a unique **Id**, palette, sizes and a TextCore font asset.
2. Click **Generate Theme Assets**. This creates `<Name>.tokens.uss` and `<Name>.tss` beside the profile.
3. Create a Panel Settings asset and assign the generated `.tss` for UI Builder preview.
4. Create `Assets > Create > Nodus > UI > Configuration`; add your Theme Profile under **Themes**.
5. Set **Initial Theme Id** on the `UI` component to the theme's **Id**.

## Designer — stacks and views

1. Create UXML using UI Builder. Place a full-viewport VisualElement named `Panels` in your root UXML.
2. Create a view UXML; design the size and position of its own root element using absolute positioning or USS.
3. Create `Assets > Create > Nodus > UI > View Definition`, assign a stable **Id** and the view UXML.
4. Add the View Definition to **Configuration > Views**.
5. In **Configuration > Stacks** set **Id**=`Panels`, **Container Name**=`Panels`; choose **TopOnly** or **Overlay**.
6. Create an ECS Entities Definition with a `Nodus.UI.PushView` component (`StackId=Panels`, `ViewId=<your view ID>`). Assign it to an `EntitiesButton` in UI Builder.

The stack fills its container; the view UXML controls its own layout. `PopView` removes the top view.

## Designer — localization

1. Install/configure Unity Localization and create the desired Locales.
2. Create a String Table Collection under **Window > Asset Management > Localization Tables**.
3. In UI Builder select a Label or Button, select **Text > Add Binding > LocalizedString**, and choose a table entry.

UI Toolkit updates these native bindings when Unity's selected Locale changes. For an ECS language action, create an Entities Definition containing `Nodus.UI.SetLocale` with `Code` such as `de`.

## Developer

1. Put Friflo `ECSStore`, `ECSSystemSet` and `Nodus.UI.UI` on the UI prefab. Add a `PanelRenderer` beside it or assign one or more PanelRenderers in **Renderers**. Assign the Configuration asset and root UXML. Additional panels can register with `RegisterRoot(renderer)` on the UI component.
2. Add these systems to the **Update** group, in order: `SetThemeSystem`, `SetLocaleSystem`, `PushViewSystem`, `PopViewSystem`, `ThemeSystem`, `LocaleSystem`, `VisibilitySystem`, `ViewsSystem`.
3. The UI component registers types and its store (`Store Id` defaults to `UI`). The systems process ECS commands and reconcile state with the UI Toolkit root.

```csharp
UI.Store.CreateEntity(new SetTheme { ThemeId = "Dark" });
UI.Store.CreateEntity(new SetLocale { Code = "de" });
UI.Store.CreateEntity(new PushView { StackId = "Panels", ViewId = "Feedback" });
UI.Store.CreateEntity(new PopView { StackId = "Panels" });
UI.Store.SetElementState("BackButton", visible: false);
```

UI Toolkit `name` identifiers and ECS IDs are case-sensitive. `Nodus.UI.ElementState` controls `display` and `enabled` for named UXML elements. UI-specific CSS is generated/shared through a native Theme Style Sheet. Native Unity Localization bindings manage text updates.

## Tests

Use EditMode tests. If they are not discovered, add `"testables": ["com.nodus.ui"]` to `Packages/manifest.json`.

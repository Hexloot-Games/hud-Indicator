# HUD Indicator v2 — Documentation

HUD Indicator draws icons that track world objects on your HUD. A single **Indicator** component can
show an icon **over the target** while it is on screen, pin an icon **to the screen edge** with an
arrow while it is off screen, or both — each with its own optional text/distance label.

- **Unity 6 (6000.0) or newer.**
- The indicator scripts are **pipeline-independent** (uGUI + TextMeshPro), so they work in Built-in,
  URP and HDRP. The included **demo scene is authored for URP** (the project's configured pipeline).

---

## Concepts

| Component | Where it goes | What it does |
|-----------|---------------|--------------|
| **Indicator Renderer** | A `RectTransform` under a `Canvas` | Defines the screen area indicators are drawn in, and the camera used to project world targets. Add one per screen region (one per camera for split-screen). |
| **Indicator** | The world object to track | Shows an on-screen icon, an off-screen edge icon + arrow, or both, plus an optional label. |

Each indicator creates one lightweight view per renderer, which is why split-screen works with no
extra code: add a renderer per camera and every indicator shows up on all of them.

---

## Setup

1. **Create the HUD** — menu **GameObject ▸ UI ▸ HUD Indicator Renderer** (or **Tools ▸ HUD
   Indicator ▸ Create Indicator Renderer**). This adds a Canvas if the scene has none, plus a
   full-screen **Indicator Renderer** with the main camera assigned.
2. **Add an indicator** — on a world object, **Add Component ▸ HUD Indicator ▸ Indicator**, and
   assign an icon **Texture** in the **Icon** field.
3. Play. If an indicator's **Renderers** list is empty it uses every renderer in the scene;
   otherwise it only draws on the renderers you assign.

> The **Margin** on a renderer is the inset from the rect edges. Off-screen icons clamp to this inset
> border, and on-screen icons switch to off-screen when they cross it.

---

## On screen / off screen

The `Indicator` has two independent toggles:

- **Show On Screen** – draw the icon over the target while it is visible. Optionally **Scale With
  Distance** (shrinks between Min/Max Scale across the Near/Far range).
- **Show Off Screen** – pin the icon to the screen edge while the target is off screen, with an
  optional **Arrow** (pointing from the centre towards the target) and an optional **Rotate Icon**.

Enable either, or both, per indicator.

---

## Text & distance labels

- **Label** – free text shown next to the icon.
- **Show Distance** – append the camera→target distance.
- **Show Text On Screen** / **Show Text Off Screen** – choose where the label appears.

The label is composed like this:

| Label | Show Distance | Result |
|-------|---------------|--------|
| "Enemy" | ✔ | `Enemy` with the distance on the line below |
| *(empty)* | ✔ | the distance, as the single line |
| "Enemy" | ✘ | `Enemy` |
| *(empty)* | ✘ | *(no label shown)* |

**Text Style**: font (defaults to the project TMP font when empty), size, color, style, the **side**
of the icon the label hugs (Right/Left/Above/Below/Center), spacing, offset, and the distance
**format** (e.g. `0 m`, `0.0 units`) + a distance **scale** multiplier. The label stays aligned with
the icon's centre, and with **Auto Flip** on (default) it flips towards the screen so it never
overflows the edge when the icon is pinned to a corner.

---

## Scripting API

```csharp
using HUDIndicator;

var indicator = target.GetComponent<Indicator>();

indicator.Visible = true;                 // master on/off
indicator.SetLabel("Objective");          // same as indicator.Label = "..."
indicator.ShowDistance = true;

indicator.ShowOnScreen = true;
indicator.ShowOffScreen = true;
indicator.ShowTextOnScreen = true;
indicator.ShowTextOffScreen = false;

indicator.Icon.color = Color.red;         // icon / arrow / text styles are mutable
indicator.TextStyle.distanceFormat = "0.0 m";
indicator.ScaleWithDistance = true;

// Renderers (leave empty to auto-use all; or drive it from code)
indicator.SetRenderer(renderer);
indicator.SetRenderers(renderers);
indicator.AddRenderer(renderer);
indicator.RemoveRenderer(renderer);
IReadOnlyList<IndicatorRenderer> list = indicator.GetRenderers();
```

Changing the renderer set at runtime rebuilds the views automatically. Style and label changes are
picked up every frame — no call needed.

---

## Render pipelines

The indicator HUD is uGUI + TextMeshPro and has no render-pipeline dependency, so the scripts work in
Built-in, URP and HDRP. The **demo scene is authored for URP** (the project's pipeline); to view it in
another pipeline, convert the demo materials in `Demo/Materials` (e.g. Edit ▸ Rendering ▸ Materials ▸
Convert…). Your own indicator setups need no conversion.

---

## Demo

`Assets/HUD Indicator/Demo/Demo.unity` shows an orbiting camera and several labelled targets (a
static Objective, an orbiting Enemy, an Ally, and a distance-only Loot pickup) so you can see both
on-screen icons and off-screen edge arrows, with live distances. Rebuild it any time from **Tools ▸
HUD Indicator ▸ Rebuild Demo Scene**.

---

## Tests

Edit-mode tests live in `Assets/HUD Indicator/Tests`. Open **Window ▸ General ▸ Test Runner**,
select **EditMode**, and run them. They cover the projection/clamping math, label composition and the
component API.

---

## Troubleshooting

- **Nothing shows up** – Make sure the Indicator Renderer is under a Canvas and a Camera is assigned
  (or a `MainCamera`-tagged camera exists). The renderer's inspector warns when it is not under a Canvas.
- **Text is invisible** – Import *TMP Essentials* (`Window ▸ TextMeshPro ▸ Import TMP Essential
  Resources`) or assign a **Font** in the Text Style.

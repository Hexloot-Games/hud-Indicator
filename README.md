# HUD Indicator v2

![GitHub](https://img.shields.io/github/license/lincolncpp/pixteal?color=red&style=flat-square)

HUD Indicator is a simple, production-ready system for on-screen and off-screen target
indicators. Point the player at objectives, enemies, pickups or teammates with clean icons,
directional arrows, and optional text/distance labels.

<img src="https://assetstorev1-prd-cdn.unity3d.com/key-image/c645035e-bf64-41a8-9c85-e30873d15683.webp" width=700>

## Features 🔥

- **One component does it all** – a single `Indicator` shows an icon over the target on screen, pins an icon + arrow to the edge off screen, or both.
- **Text & distance labels** – show a label, the distance to the target, or both (distance sits below the text, or stands alone when there is no text). Rendered with TextMeshPro, hugging the side of the icon.
- **Per-state text** – choose independently whether the label shows on screen and/or off screen.
- **No overflow** – labels auto-flip towards the screen so they never run off the edge.
- **Distance scaling** – on-screen icons can shrink with distance.
- **Split-screen ready** – add one renderer per camera; every indicator draws on all of them.
- **Pipeline-independent core** – the HUD is uGUI + TextMeshPro, so the indicator scripts work in Built-in, URP and HDRP. The included demo is authored for URP.
- **One-click setup** – *GameObject ▸ UI ▸ HUD Indicator Renderer* builds the Canvas + Renderer for you.
- **Runtime API, clean inspectors, editor gizmos, and unit tests.**

## Requirements

- **Unity 6 (6000.0) or newer.** (For older Unity versions, use HUD Indicator v1.)
- TextMeshPro (included with `com.unity.ugui`, which ships with Unity 6).
- The included demo scene is set up for **URP**. The indicator scripts themselves have no render-pipeline dependency, so you can use them in Built-in or HDRP too.

## Quick start

1. **Create the HUD**: menu **GameObject ▸ UI ▸ HUD Indicator Renderer**. This adds a Canvas (if
   needed) and a full-screen **Indicator Renderer** with the main camera assigned.
2. **Add an indicator**: on any world object, **Add Component ▸ HUD Indicator ▸ Indicator**, and
   assign an icon texture in **Icon**.
3. Press Play. Indicators auto-use every renderer in the scene (or assign specific ones for split-screen).

### Text & distance

Set **Label** and/or tick **Show Distance**, then choose where the text appears with
**Show Text On Screen** / **Show Text Off Screen**:

- Label + Show Distance → the label with the distance on the line below.
- Show Distance only → the distance as the single line.
- Label only → just the label.

```csharp
var indicator = enemy.GetComponent<HUDIndicator.Indicator>();
indicator.SetLabel("Boss");
indicator.ShowDistance = true;        // -> "Boss\n42 m"
indicator.ShowTextOnScreen = true;
indicator.ShowTextOffScreen = true;   // also label the edge arrow
```

### Render pipelines

The indicator scripts are uGUI + TextMeshPro and have **no dependency on any render pipeline** — drop
them into a Built-in, URP or HDRP project. The included **demo scene** is authored for **URP** (the
project's configured pipeline); its lit materials would need converting to use it in another pipeline.

See [`Assets/HUD Indicator/Documentation.md`](Assets/HUD%20Indicator/Documentation.md) for the full guide and API reference.

## Unity Asset Store
https://assetstore.unity.com/packages/tools/gui/hud-indicator-220695

## License
This project is licensed under the MIT License - see the [LICENSE](/LICENSE) file for details.

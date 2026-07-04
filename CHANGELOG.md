# Changelog

All notable changes to HUD Indicator are documented here.

## [2.0.0]

Version 2 is a full rewrite that targets **Unity 6 (6000.0) and newer only**, with breaking changes.

### Added
- **Unified `Indicator` component.** A single component now handles on-screen and off-screen
  indication with independent **Show On Screen** / **Show Off Screen** toggles — no more adding two
  components. (Replaces the separate `IndicatorOnScreen` / `IndicatorOffScreen`.)
- **Text & distance labels** (TextMeshPro): a `Label` field and a `Show Distance` toggle. With text
  + distance, the distance is drawn on the line below; with distance only, it becomes the single
  line. The label hugs the chosen side of the icon and stays aligned with its centre.
- **Per-state text**: `Show Text On Screen` and `Show Text Off Screen` control the label
  independently in each state.
- **Auto-flipping labels**: the label flips towards the screen interior near edges/corners so it
  never overflows off screen (`Auto Flip`, on by default).
- **Distance scaling** for on-screen icons; **Rotate Icon** for off-screen.
- **One-click setup**: *GameObject ▸ UI ▸ HUD Indicator Renderer* builds the Canvas + Renderer.
- **Runtime API**: `SetLabel`, `Label`, `Visible`, `ShowOnScreen/OffScreen`,
  `ShowTextOnScreen/OffScreen`, `ShowDistance`, `SetRenderer(s)`/`AddRenderer`/`RemoveRenderer`,
  `Rebuild`, and mutable `Icon` / `ArrowStyle` / `TextStyle`.
- Context-sensitive custom inspector and a scene gizmo that previews the drawable area.
- A rebuilt, livelier **URP** demo (orbiting camera, moving enemy, labelled targets with live
  distances) using authored URP/Lit materials. Rebuildable via *Tools ▸ HUD Indicator ▸ Rebuild Demo
  Scene*. The indicator scripts remain pipeline-independent (uGUI + TextMeshPro).
- **Edit-mode unit tests** (39) covering the projection/clamping math, label composition and the
  component API. Assembly definitions for the editor, demo and tests.

### Changed
- The project is now configured for the **Universal Render Pipeline** (URP 17). The indicator scripts
  stay pipeline-independent; only the demo depends on URP.
- Rewrote world→screen projection using resolution-independent viewport coordinates, fixing
  positioning drift at non-default resolutions and under canvas scaling. The tricky math lives in a
  pure, unit-tested `IndicatorMath`.
- Reworked off-screen edge clamping and behind-camera handling into a single, tested routine.
- Indicators update in `LateUpdate` so positions use the camera's final transform each frame.
- `IndicatorRenderer` now requires a `RectTransform`, caches it, and warns when not under a Canvas.

### Fixed
- Replaced the deprecated `FindObjectsOfType` with `FindObjectsByType`.
- Removed the per-editor-frame `Texture2D` allocation in the renderer gizmo (now drawn with `Handles`).
- Indicators are hidden when their target/renderer is disabled or destroyed at runtime instead of
  throwing missing-reference errors.
- The icon quad no longer draws a white box when no texture is assigned.
- Distance formatting uses invariant culture and never throws on a malformed format string.

### Removed
- Support for Unity versions older than 6.
- The separate `IndicatorOnScreen` / `IndicatorOffScreen` components (merged into `Indicator`).
- The deprecated `com.unity.ide.vscode` package.

# Magnet Game

A 2D platformer where you play as a little magnet. Instead of running and jumping your way through the levels, you throw yourself into magnetic fields, flip your polarity, and let attraction and repulsion build up the speed you need to clear each challenge.

Built with Unity **2020.3.30f1** (LTS) using the built-in 2D physics.

## Controls

| Action | Binding |
| --- | --- |
| Move | `A` / `D` or the arrow keys |
| Jump | `Space` (hold for a higher jump, release early for a short hop) |
| Activate magnet | Hold `Left Shift` or left mouse button |
| Flip polarity | `Q` or right mouse button (End scene only) |
| Respawn | `R` (drops you back at the start point) |

Magnet input and polarity swaps are read through the default `Fire1` / `Fire2` axes, so they can be rebound in **Project Settings → Input Manager** without touching code.

## How the magnetism works

Every magnetic body has a `MagneticObject` component carrying its `magnetStrength`, and it is either red or blue. A `MagneticField` trigger carries a `fieldStrength` and a colour of its own.

When a magnetic object sits inside a field, `MagneticField.OnTriggerStay2D` checks the colours with an XOR:

```csharp
bool isAttraction = isRed ^ colliderMagneticObject.isRed;
```

Same colour means attraction, opposite colours mean repulsion, which is why flipping your polarity in the End scene reverses every field in the room at once. The resulting force scales with `fieldStrength * magnetStrength`, is negated for repulsion, and the object is smoothly rotated to face the field. Circular fields pull toward their centre; square fields push along the axis defined by their two corner markers.

While the player is being pulled, the controller hands over control: gravity is set to zero, the player's own directional input is added to the magnetic force as a small steering nudge, and the usual auto-upright rotation is suspended so you can spin with the field.

## Player controller

`PlayerController.cs` is a hand-written 2D controller rather than a stock asset. It covers:

- Acceleration and deceleration curves, with separate multipliers while airborne and a `speedPower` exponent so acceleration grows at higher speeds
- Coyote time and jump buffering, plus a short-hop multiplier when jump is released mid-rise
- Extra gravity while falling and a terminal fall speed
- Ground friction, suppressed while you are being sucked toward a field
- A speed-dependent jump bonus, so moving fast gets you a little higher

The camera (`CameraScript.cs`) follows the player and zooms out as horizontal speed rises, up to a configurable maximum. That widening view is what makes the high-speed sections readable.

## Project structure

```
Assets/
  Animations/   Player animator controller and colour-state animations
  Palette1/     Red / blue / grey / white palette assets
  Prefabs/      Player, CircleField, SquareField, door
  Scenes/       Menu, Level1, Level2, Level3, End
  Scripts/      Gameplay scripts (see below)
  Sprites/      Artwork
  TextMesh Pro/ Imported TMP essentials
Packages/       Package manifest
ProjectSettings/ Unity project settings, input axes, build scene list
```

| Script | Role |
| --- | --- |
| `PlayerController.cs` | Movement, jumping, friction, magnet input, polarity flip |
| `MagneticField.cs` | Applies attraction/repulsion force and rotation inside a field |
| `MagneticObject.cs` | Per-object magnet state: strength, colour, whether it is active |
| `CameraScript.cs` | Follows the player and zooms with speed |
| `LevelFlag.cs` | Goal trigger: plays a sound, fades out, loads the next scene |
| `MainMenu.cs` | Start and quit buttons |
| `PauseMenu.cs` | `Esc` pause menu, driven by a static `isPaused` flag |
| `UI.cs` | Debug readout of the player's velocity |

## Running the project

1. Install **Unity 2020.3.30f1** through Unity Hub (the LTS line is fine, but matching the version avoids asset upgrade churn).
2. Add this folder as a project in Unity Hub and open it.
3. Open `Assets/Scenes/Menu.unity` and press Play.

The build settings list `Menu`, `Level1`, `Level2`, `Level3` and `End` in that order, plus the leftover `SampleScene`, which can be ignored. Level progression is purely `buildIndex + 1`, so new levels must be inserted in the build list in the right position.

## Notes

- `R` respawns the player at the hardcoded world position `(0, 5)`, not at a spawn point, so it only behaves correctly in levels laid out around that origin.
- The polarity flip is gated on the active scene being named `End`, which makes the End scene the only place the mechanic is exposed.
- There is no test suite and no CI; everything is verified by playing the scenes in the editor.

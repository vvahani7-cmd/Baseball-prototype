
# Day 1 – Initial Setup & Pitch Trajectory System

## Overview

On Day 1, I set up the foundation for a baseball pitch trajectory system in Unity. I began by placing a basic batter FBX in the scene to establish scale, orientation, and spatial context. The main focus of the day was designing a controllable and visualizable pitch motion system rather than implementing full gameplay.

----------

## Scripts Created

### PitchData

`PitchData` is a serializable data class that defines all parameters required to construct a pitch trajectory. Each cubic Bezier control point has independent vertical, horizontal, and depth offsets, allowing fine-grained control over pitch shape.

`using UnityEngine;

[System.Serializable] public  class  PitchData { public Vector3 startPosition; public Vector3 endPosition; public  float timeOfFlight;

    [Header("Control Point 1 (Early)")] public  float cp1Height; public  float cp1Horizontal; public  float cp1Depth;

    [Header("Control Point 2 (Late)")] public  float cp2Height; public  float cp2Horizontal; public  float cp2Depth;
}` 

This structure keeps the trajectory data-driven, easy to tune, and suitable for future difficulty scaling and experimentation.

----------

### BezierCurveUtility

`BezierCurveUtility` is a static helper class responsible for generating cubic Bezier curves and building an arc-length lookup table. The arc-length table enables constant-speed traversal of the curve regardless of its shape.

`Vector3[] curve = BezierCurveUtility.GenerateCubicBezier(
    p0, p1, p2, p3, resolution
); float[] arcLengths = BezierCurveUtility.BuildArcLengthTable(curve);` 

This approach avoids per-frame Bezier evaluation and ensures deterministic motion.

----------

### BallController

`BallController` handles ball movement along the precomputed curve using distance-based motion. The ball advances along the curve based on traveled distance rather than normalized time (`t`), ensuring consistent speed.

`travelledDistance += speed * Time.deltaTime;
transform.position = GetPositionAtDistance(travelledDistance);` 

The ball is represented by:

-   A parent GameObject with a `Rigidbody`
    
-   A child GameObject containing the visual mesh and a `SphereCollider`
    

Movement begins when a pitch is initiated by the `PitcherController`.

----------

### PitcherController

`PitcherController` is responsible for:

-   Defining pitch start and end positions using transforms
    
-   Constructing two cubic Bezier control points from `PitchData`
    
-   Launching the ball
    
-   Visualizing the trajectory using Gizmos
    

`Vector3 control1 = start + forward * cp1Depth + up * cp1Height + right * cp1Horizontal;
Vector3 control2 = end   - forward * cp2Depth + up * cp2Height + right * cp2Horizontal;` 

Gizmos are used to draw:

-   Start position
    
-   End position
    
-   Both Bezier control points
    
-   Helper lines between points
    
-   The final cubic Bezier trajectory
    

This allows real-time visual debugging and tuning directly in the Scene view.

----------

## Design Decision: Curve vs Physics

During this process, I studied cubic Bezier curves and deliberately chose a curve-based approach over pure physics for pitch motion. This decision was made because swing timing and trajectory readability are critical for gameplay and must remain under designer control.

Based on prior experience:

-   In a badminton project, quadratic curves were used to create shuttlecock-like trajectories.
    
-   In a cricket project, motion was handled primarily using physics simulation.
    

For this baseball prototype, I decided to use a **hybrid curve + physics approach**:

-   Curves define pitch intent, trajectory shape, and difficulty.
    
-   Physics will later handle bat–ball interaction and post-hit behavior.

# Day 2 – Interactive Targeting & Hybrid Curve-to-Physics Pitch Flow

## Overview
On Day 2, I focused on making the pitch system **interactive** and **player-driven**. The goal was to move beyond a fixed trajectory and allow the pitcher and batter to influence pitch intent through direct input. This was achieved by introducing a controllable target plane, mouse-driven positioning, and a clear separation between *aiming*, *confirmation*, and *execution*.

This approach follows the same design philosophy I have previously applied in **golf, badminton, and cricket prototypes**—designer-controlled intent first, physics second.

To immerse myself further in the mindset and strategic depth of baseball, I also watched *Moneyball* as part of the assignment. The film reinforced the idea that baseball is as much about **decision-making, positioning, and intent** as it is about raw mechanics—concepts that directly influenced today’s implementation.

---

## Interactive Pitch Targeting System

### Target Plane Setup
I created a plane in front of the batter that represents the **pitch interaction zone**. This plane serves as a visual and logical reference for where the pitch will terminate.

Two target markers were introduced:
- **Pitcher Target** – Used when the pitcher is selecting pitch intent.
- **Batter Target** – Used later for batter-side interaction and anticipation.

Both targets are constrained within defined bounds to simulate a realistic strike/interaction zone.

---

## IncomingPitchReflection

The `IncomingPitchReflection` script is responsible for:
- Handling mouse input using Unity’s **New Input System**
- Allowing either the pitcher or batter to move their respective target
- Constraining movement within a defined 2D area
- Acting as a shared reference for pitch end-position sampling

### Key Design Decisions
- Movement is **local-space constrained**, ensuring predictable behavior relative to the target plane.
- Input responsibility is gated using boolean flags (`isPitcherAllowedToSetPos`, `isBatterAllowedToSetPos`) to prevent conflicting control.
- The system is input-agnostic and can later be extended to controller or AI input.

### Code Written

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class IncomingPitchReflection : MonoBehaviour
{
    public static IncomingPitchReflection Instance;

    private Vector2 xLimits = new Vector3(-.5f, .5f);
    private Vector2 yLimits = new Vector3(-.5f, .5f);

    [SerializeField] private Transform positionByPitcher;
    [SerializeField] private Transform positionByBatter;
    [SerializeField] private float mouseSensitivity = .01f;

    public bool isPitcherAllowedToSetPos = false;
    public bool isBatterAllowedToSetPos = false;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isPitcherAllowedToSetPos && Mouse.current != null)
        {
            Vector3 pos = positionByPitcher.localPosition;
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            pos.x += mouseDelta.x * mouseSensitivity;
            pos.y += mouseDelta.y * mouseSensitivity;

            pos.x = Mathf.Clamp(pos.x, xLimits.x, xLimits.y);
            pos.y = Mathf.Clamp(pos.y, yLimits.x, yLimits.y);
            pos.z = 0;

            positionByPitcher.localPosition = pos;
        }

        if (isBatterAllowedToSetPos && Mouse.current != null)
        {
            Vector3 pos = positionByBatter.localPosition;
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            pos.x += mouseDelta.x * mouseSensitivity;
            pos.y += mouseDelta.y * mouseSensitivity;

            pos.x = Mathf.Clamp(pos.x, xLimits.x, xLimits.y);
            pos.y = Mathf.Clamp(pos.y, yLimits.x, yLimits.y);
            pos.z = 0;

            positionByBatter.localPosition = pos;
        }
    }

    public Vector3 GetPositionByBatter()
    {
        return positionByBatter.position;
    }
}
```
## Game Flow Control

### GameManager

To manage turn order and future state transitions, I introduced a lightweight `GameManager`.  
At this stage, it tracks **who currently has control** (Pitcher or Batter), laying the groundwork for a turn-based pitch → swing → outcome loop.

```
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerStatus currentPlayerStatus;

    public void SetPlayerStatus(PlayerStatus playerStatus)
    {
        currentPlayerStatus = playerStatus;
    }

    public PlayerStatus GetPlayerStatus()
    {
        return currentPlayerStatus;
    }

    void Start()
    {
        currentPlayerStatus = PlayerStatus.Pitcher;
    }
}

```

This system will later coordinate:

-   Pitcher aim phase
    
-   Batter anticipation phase
    
-   Pitch execution
    
-   Bat–ball interaction
    
-   Result evaluation
    

----------

## Hybrid Curve + Physics Transition

Continuing the design philosophy established on Day 1, I extended the **hybrid motion approach**:

-   The ball follows a **precomputed cubic Bezier trajectory** for readability, intent, and timing control.
    
-   Once the ball reaches the end of the curve, it is **released into physics simulation**.
    

This mirrors my previous sports projects:

-   **Golf** – Designer-driven shot arcs
    
-   **Badminton** – Curve-defined shuttle flight
    
-   **Cricket** – Physics-dominant post-contact motion
    

For baseball, this hybrid approach ensures:

-   Predictable pitch behavior up to the plate
    
-   Natural physics response after contact or miss
    
-   Clean separation between “pitch intent” and “physical consequence”
    

----------

## Reflection

Day 2 transformed the pitch system from a purely technical prototype into an **interactive system with player intent**. The addition of mouse-driven targeting, turn-based control logic, and curve-to-physics transition brings the project closer to real gameplay while still preserving strong designer control.

Watching _Moneyball_ helped reinforce the importance of intent, positioning, and decision-making—principles that directly informed today’s work.



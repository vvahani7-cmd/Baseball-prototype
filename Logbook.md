
# Day 1 – Initial Setup & Pitch Trajectory System

## Overview

On Day 1, I set up the foundation for a baseball pitch trajectory system in Unity. I began by placing a basic batter FBX in the scene to establish scale, orientation, and spatial context. The main focus of the day was designing a controllable and visualizable pitch motion system rather than implementing full gameplay.

----------

## Scripts Created

### PitchData

`PitchData` is a serializable data class that defines all parameters required to construct a pitch trajectory. Each cubic Bezier control point has independent vertical, horizontal, and depth offsets, allowing fine-grained control over pitch shape.
```
using UnityEngine;

[System.Serializable] public  class  PitchData { 
	public Vector3 startPosition; 
	public Vector3 endPosition; 
	public  float timeOfFlight;

    [Header("Control Point 1 (Early)")] 
    public  float cp1Height; 
    public  float cp1Horizontal; public  float cp1Depth;

    [Header("Control Point 2 (Late)")] 
    public  float cp2Height; 
    public  float cp2Horizontal; public  float cp2Depth;
}
```
This structure keeps the trajectory data-driven, easy to tune, and suitable for future difficulty scaling and experimentation.

----------

### BezierCurveUtility

`BezierCurveUtility` is a static helper class responsible for generating cubic Bezier curves and building an arc-length lookup table. The arc-length table enables constant-speed traversal of the curve regardless of its shape.
```
Vector3[] curve = BezierCurveUtility.GenerateCubicBezier(
    p0, p1, p2, p3, resolution
); 
float[] arcLengths = BezierCurveUtility.BuildArcLengthTable(curve);
```
This approach avoids per-frame Bezier evaluation and ensures deterministic motion.

----------

### BallController

`BallController` handles ball movement along the precomputed curve using distance-based motion. The ball advances along the curve based on traveled distance rather than normalized time (`t`), ensuring consistent speed.
```
travelledDistance += speed * Time.deltaTime;
transform.position = GetPositionAtDistance(travelledDistance);
```

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
    
```
Vector3 control1 = start + forward * cp1Depth + up * cp1Height + right * cp1Horizontal;
Vector3 control2 = end   - forward * cp2Depth + up * cp2Height + right * cp2Horizontal; 
```

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

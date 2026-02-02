using UnityEngine;

[System.Serializable]
public class PitchData
{
    public Vector3 startPosition;
    public Vector3 endPosition;

    public float timeOfFlight;

    [Header("Control Point 1 Offsets (Early)")]
    public float cp1Height;     // up / down
    public float cp1Horizontal; // left / right
    public float cp1Depth;      // forward / backward

    [Header("Control Point 2 Offsets (Late)")]
    public float cp2Height;
    public float cp2Horizontal;
    public float cp2Depth;
}

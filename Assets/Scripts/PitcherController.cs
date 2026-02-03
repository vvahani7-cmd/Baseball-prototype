using UnityEngine;
using UnityEngine.InputSystem;

public class PitcherController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallController ball;
    [SerializeField] private PitchData pitchData;

    [Header("Pitch Transforms")]
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform endTransform;

    [Header("Curve Settings")]
    [SerializeField] private int resolution = 60;

    Vector3 p1, p2;

    public float ballRadius;

    public void ThrowPitch()
    {
        pitchData.startPosition = startTransform.position;
        pitchData.endPosition = endTransform.position;

        Vector3 start = pitchData.startPosition;
        Vector3 end = pitchData.endPosition;

        Vector3 forward = (end - start).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        Vector3 up = Vector3.up;

        // Control Point 1 (early)
        p1 =
            start +
            forward * pitchData.cp1Depth +
            up * pitchData.cp1Height +
            right * pitchData.cp1Horizontal;

        // Control Point 2 (late)
        p2 =
            end -
            forward * pitchData.cp2Depth +
            up * pitchData.cp2Height +
            right * pitchData.cp2Horizontal;



        Vector3[] curve =
            BezierCurveUtility.GenerateCubicBezier(
                start, p1, p2, end, resolution
            );

        float[] arc =
            BezierCurveUtility.BuildArcLengthTable(curve);

        float totalLength = arc[arc.Length - 1];
        float speed = totalLength / pitchData.timeOfFlight;

        ball.SetPath(curve, arc, speed);
    }

    private bool isDragging;
    private bool dragDetected;
    private Vector2 pressPosition;
    [SerializeField] private float dragThreshold = 8f;
    private void Update()
    {
        if (Mouse.current == null) return;

        // Mouse down
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            pressPosition = Mouse.current.position.ReadValue();
            dragDetected = false;
            isDragging = true;

            IncomingPitchReflection.Instance.isBatterAllowedToSetPos = true;
        }

        // Mouse held
        if (Mouse.current.leftButton.isPressed && isDragging)
        {
            Vector2 currentPos = Mouse.current.position.ReadValue();

            if (!dragDetected &&
                Vector2.Distance(currentPos, pressPosition) > dragThreshold)
            {
                dragDetected = true;
            }

            if (dragDetected)
            {
                OnDrag();
            }
        }

        // Mouse up
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            IncomingPitchReflection.Instance.isBatterAllowedToSetPos = false;
            isDragging = false;

            // CLICK = throw
            if (!dragDetected)
            {
                ThrowPitch();
            }
        }

    }

    void OnDrag()
    {
        endTransform.position = IncomingPitchReflection.Instance.GetPositionByBatter();
    }


    // =======================
    // GIZMOS VISUALIZATION
    // =======================
    private void OnDrawGizmos()
    {
        if (startTransform == null || endTransform == null || pitchData == null)
            return;

        Vector3 start = startTransform.position;
        Vector3 end = endTransform.position;

        Vector3 forward = (end - start).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        Vector3 up = Vector3.up;

        // Control Point 1 (Early)
        Vector3 control1 =
            start +
            forward * pitchData.cp1Depth +
            up * pitchData.cp1Height +
            right * pitchData.cp1Horizontal;

        // Control Point 2 (Late)
        Vector3 control2 =
            end -
            forward * pitchData.cp2Depth +
            up * pitchData.cp2Height +
            right * pitchData.cp2Horizontal;

        // Draw start & end
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(start, ballRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(end, ballRadius);

        // Draw control points
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(control1, ballRadius);
        Gizmos.DrawSphere(control2, ballRadius);

        // Draw helper lines
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(start, control1);
        Gizmos.DrawLine(control1, control2);
        Gizmos.DrawLine(control2, end);

        // Draw Bezier curve
        Gizmos.color = Color.cyan;
        Vector3 prev = start;

        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 point =
                BezierCurveUtility.EvaluateCubicBezier(
                    start, control1, control2, end, t
                );

            Gizmos.DrawLine(prev, point);
            prev = point;
        }
    }

}

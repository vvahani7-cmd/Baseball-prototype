using UnityEngine;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThrowPitch();
        }
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
        Gizmos.DrawSphere(start, 0.05f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(end, 0.05f);

        // Draw control points
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(control1, 0.04f);
        Gizmos.DrawSphere(control2, 0.04f);

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

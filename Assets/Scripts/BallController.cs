using UnityEngine;

public class BallController : MonoBehaviour
{
    Vector3[] curvePoints;
    float[] arcLengths;

    float speed;
    float travelledDistance;
    float totalLength;

    bool isMoving;

    Rigidbody rb;
    Vector3 lastPosition;
    Vector3 currentVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }


    public void SetPath(Vector3[] curve, float[] arc, float ballSpeed)
    {
        curvePoints = curve;
        arcLengths = arc;

        speed = ballSpeed;
        travelledDistance = 0f;
        totalLength = arcLengths[arcLengths.Length - 1];

        isMoving = true;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;


        transform.position = curvePoints[0];
        lastPosition = transform.position;
    }


    void Update()
    {
        if (!isMoving) return;

        travelledDistance += speed * Time.deltaTime;

        if (travelledDistance >= totalLength)
        {
            transform.position = curvePoints[curvePoints.Length - 1];
            EnablePhysics();
            return;
        }

        Vector3 newPos = GetPositionAtDistance(travelledDistance);
        currentVelocity = (newPos - lastPosition) / Time.deltaTime;

        transform.position = newPos;
        lastPosition = newPos;
    }

    void EnablePhysics()
    {
        isMoving = false;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = currentVelocity;
    }


    Vector3 GetPositionAtDistance(float distance)
    {
        for (int i = 1; i < arcLengths.Length; i++)
        {
            if (arcLengths[i] >= distance)
            {
                float d0 = arcLengths[i - 1];
                float d1 = arcLengths[i];

                float t = (distance - d0) / (d1 - d0);
                return Vector3.Lerp(curvePoints[i - 1], curvePoints[i], t);
            }
        }

        return curvePoints[curvePoints.Length - 1];
    }
}

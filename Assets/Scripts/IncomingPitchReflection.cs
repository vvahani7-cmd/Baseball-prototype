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

    void Start()
    {

    }

    void Update()
    {
        if (isPitcherAllowedToSetPos)
        {
            if (Mouse.current != null)
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


        }

        if (isBatterAllowedToSetPos)
        {
            if (Mouse.current != null)
            {
                Vector3 pos = positionByBatter.transform.localPosition;
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                pos.x += mouseDelta.x * mouseSensitivity;
                pos.y += mouseDelta.y * mouseSensitivity;

                pos.x = Mathf.Clamp(pos.x, xLimits.x, xLimits.y);
                pos.y = Mathf.Clamp(pos.y, yLimits.x, yLimits.y);
                pos.z = 0;

                positionByBatter.localPosition = pos;

            }
        }
    }

    public Vector3 GetPositionByBatter()
    {
        return positionByBatter.position;
    }
}

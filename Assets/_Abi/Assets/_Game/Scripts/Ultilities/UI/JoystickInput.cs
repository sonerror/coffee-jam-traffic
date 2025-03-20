using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class JoystickInput : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public static JoystickInput ins;

    private bool isClickable;
    private Vector2 mouseDownPos;
    private Vector2 mouseUpPos;
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;
    public RectTransform joystickPanel;
    public GameObject joystick;
    [SerializeField] private float joystickMaxDistance = 150f;

    // public Vector3 MoveDir { get; private set; } // not use this to use ve3 .set
    [HideInInspector] public Vector3 moveDir;

    private void Awake()
    {
        if (ins != null) DestroyImmediate(ins.gameObject);
        ins = this;

        isClickable = true;
    }

    private void OnEnable()
    {
        joystick.SetActive(false);
    }

    private void OnDisable()
    {
        SetBackState();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isClickable) return;


        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position,
                eventData.pressEventCamera, out var pos))
        {
            moveDir.Set(pos.x, 0, pos.y);
            moveDir.Normalize();
            joystickHandle.anchoredPosition = Vector3.ClampMagnitude(pos, joystickMaxDistance);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isClickable) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickPanel, eventData.position,
                eventData.pressEventCamera, out var posD))
        {
            joystickBackground.anchoredPosition = posD;
            joystick.SetActive(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetBackState();
    }

    public void SetBackState()
    {
        moveDir.Set(0, 0, 0);
        joystickHandle.anchoredPosition = default(Vector3);
        joystick.SetActive(false);
    }

    public void SetLockState(bool isLock)
    {
        if (isLock)
        {
            SetBackState();
        }

        isClickable = !isLock;
    }
}
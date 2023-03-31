using UnityEngine;

public class StatusPanelLayout : MonoBehaviour
{
    [SerializeField]
    private Vector2 screenPos;

    [SerializeField]
    private float yOffset;

    [SerializeField]
    private float xOffset;

    private RectTransform rect;

    [SerializeField]
    private Camera uiCamera;

    private Camera mainCamera;

    [SerializeField]
    private Transform StatusPanelEx;

    [SerializeField]
    private RectTransform parentRect;

    private StatusPanelUI statusPanel;

    private void Awake()
    {
        mainCamera = CameraManager.Instance.MainCamera;
        uiCamera = CameraManager.Instance.UICamera;
        rect = GetComponent<RectTransform>();
        statusPanel = GetComponent<StatusPanelUI>();
        parentRect = transform.parent.GetComponent<RectTransform>();
    }

    private void Start()
    {
        StatusPanelEx = statusPanel.Unit.transform.Find("StatusPanelEx");
    }

    // Start is called before the first frame update
    private void LateUpdate()
    {
        screenPos = WorldPointToScreenPoint(StatusPanelEx.position);
        Vector2 resultPos = ScreenPointToUIPoint(parentRect, screenPos);
        rect.localPosition = resultPos + new Vector2(xOffset, yOffset);
    }

    public Vector2 ScreenPointToUIPoint(RectTransform rt, Vector2 screenPoint)
    {
        Vector2 globalMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPoint, uiCamera, out globalMousePos);
        return globalMousePos;
    }

    public Vector2 WorldPointToScreenPoint(Vector3 worldPoint)
    {
        Vector2 screenPoint = mainCamera.WorldToScreenPoint(worldPoint);
        return screenPoint;
    }
}
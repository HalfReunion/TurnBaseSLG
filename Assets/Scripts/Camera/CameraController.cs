using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 moveDir;

    [SerializeField]
    private Vector3 quDir;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float rotateSpeed;

    [SerializeField]
    private bool isRever;

    [SerializeField]
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private Vector3 followOffset;
    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 12f;
    private CinemachineTransposer cinemachineTransposer;

    private int[] rotates = new int[4] { -135, 135, 45, -45 };

    [SerializeField]
    private int index = 0;

    private bool isRotating = false;

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        followOffset = cinemachineTransposer.m_FollowOffset;
        transform.eulerAngles = new Vector3(0, rotates[index], 0);
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        moveDir = transform.rotation * new Vector3(horizontal, 0, vertical);
        quDir = new Vector3(0, 10, 0);
        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (!isRotating)
                {
                    index--;
                    if (index < 0)
                    {
                        index = 3;
                    }
                    isRotating = true;
                }
            }
        }
        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //quDir.y -= 90f;
                index = ((index + 1) % rotates.Length);
                isRotating = true;
            }
        }

        //transform.eulerAngles += quDir * (isRever?-1f:1f) * rotateSpeed * Time.deltaTime;

        //transform.eulerAngles += quDir * (isRever ? -1f : 1f) * rotateSpeed * Time.deltaTime;
        if (isRotating)
        {
            transform.DORotate(new Vector3(0, rotates[index], 0), 1f).OnComplete(() => { isRotating = false; });
        }
        transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;

        float zoomAmount = 1f;
        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset.y -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            followOffset.y += zoomAmount;
        }

        followOffset.y = Mathf.Clamp(followOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        cinemachineTransposer.m_FollowOffset =
            Vector3.Lerp(cinemachineTransposer.m_FollowOffset,
            followOffset,
            Time.deltaTime * 10f);
    }
}
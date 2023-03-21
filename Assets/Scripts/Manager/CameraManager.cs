using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject;
    [SerializeField] private float cameraCharacterHeights;

    public Camera MainCamera;
    public Camera UICamera;

    private static CameraManager instance;
    public static CameraManager Instance => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;

        MainCamera = Camera.main;
        UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
    }

    private void Start()
    {
        BaseAction.OnBeforeConfirm += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        UnitActionSystem.Instance.OnSelectedActionChanged += BaseAction_OnAnyActionCompleted;
        HideActionCamera();
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        bool isActive = actionCameraGameObject.activeSelf;
        switch (sender)
        {
            case ShootAction shootAction:
                if (isActive) HideActionCamera();
                break;

            case MoveAction moveAction:
                if (isActive) HideActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.BelongUnit;
                Unit targetUnit = shootAction.TargetUnit;
                Vector3 cameraCharacterHeight = Vector3.up * cameraCharacterHeights;
                Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;
                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;
                Vector3 dir = shooterUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + (shootDir * -1);
                actionCameraGameObject.transform.position = dir;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);
                ShowActionCamera();
                break;
        }
    }

    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    }
}
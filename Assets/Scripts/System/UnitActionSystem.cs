using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ���ڿ��ƽ�ɫ�ж�����
/// </summary>
public class UnitActionSystem : MonoBehaviour
{
    [SerializeField] private Unit selectUnit;
    private Camera cam;

    [SerializeField]
    private LayerMask unitLayMask;

    [ShowInInspector]
    private bool isBusy = false;

    public bool IsBusy => isBusy;
    public static UnitActionSystem instance;

    public static UnitActionSystem Instance => instance;

    /// <summary>
    /// ��ѡ�񵽽�ɫ��ʱ��
    /// </summary>
    public event EventHandler OnSelectedUnitChanged;

    public event EventHandler OnSelectedActionChanged;

    public event Action<bool> OnBusyActionChanged;

    public event Action<int> OnActionPointsChanged;

    public Unit SelectUnit => selectUnit;
    public BaseAction SelectedAction => selectedAction;

    private BaseAction selectedAction;

    private void Awake()
    {
        cam = Camera.main;

        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        //TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
        EventDispatcher.Instance.SubScribe<object, EventArgs>(GameEventType.OnTurnChanged,
            OnTurnChanged);
        BaseAction.OnAnyActionCompleted += UpdateActionsButtons;
    }

    private void OnTurnChanged(object sender, EventArgs msg)
    {
        SetSelectedUnit(null);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isBusy)
        {
            return;
        }

        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        //����UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        HandleSelectedAction();

        if (!HandleUnitSelection())
        {
            //�ص�ʲô��û��ѡ��״̬ ����ActionPoint�������ѡ��Ȧ

            return;
        }
    }

    public void HandleSelectedAction()
    {
        if (selectUnit == null) return;

        if (Input.GetMouseButton(0))
        {
            //��ѡĿ��
            GridPos mouseGridPosition = LevelGrid.Instance.GetGridPosition(WorldMouse.Instance.GetPosition());
            if (selectedAction == null || !selectedAction.IsValidActionGridPos(mouseGridPosition)) return;

            ComfirmAction(mouseGridPosition);
        }
    }

    /// <summary>
    /// ִ��Action
    /// </summary>
    /// <param name="mouseGridPosition"></param>
    public void ComfirmAction(GridPos mouseGridPosition)
    {
        //�������Ϳ۳�������true
        if (!selectUnit.IsEnoughActionPointsToTakeAction(selectedAction)) return;

        Debug.Log("ִ��Action");
        //���Ŀ��,����ȷ�Ͻ׶�
        selectedAction.ConfirmAction(mouseGridPosition, SetBusy, ActionComplete);
    }

    private void ActionComplete()
    {
        ClearBusy();
        selectUnit.TrySpendActionPointsToTakeAction(selectedAction, OnActionPointsChanged);
    }

    /// <summary>
    /// ѡ���ɫ
    /// </summary>
    private bool HandleUnitSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //����ѡ��Ľ�ɫ
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, unitLayMask))
            {
                if (hitInfo.collider.gameObject.TryGetComponent<Unit>(out Unit unit))
                {
                    if (!unit.CanControl()) return false;
                    if (unit == this.selectUnit) return false;
                    SetSelectedUnit(unit);
                    return true;
                }
            }
            //HACK: ��bug ��Ҫ��ѡunit&&û��ѡAction ���� ��ѡ��ȡ���ж�
            if (selectUnit != null && selectedAction == null)
            {
                SetSelectedUnit(null);
            }
        }

        return false;
    }

    private void SetBusy()
    {
        Debug.Log("SetBusy");
        isBusy = true;
        OnBusyActionChanged(isBusy);
    }

    private void ClearBusy()
    {
        Debug.Log("ClearBusy");
        isBusy = false;
        OnBusyActionChanged(isBusy);
    }

    /// <summary>
    /// ���õ�ǰѡ��Ľ�ɫ
    /// </summary>
    /// <param name="unit"></param>
    private void SetSelectedUnit(Unit unit)
    {
        if (unit == null)
        {
            selectUnit = null;
            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
            GridSystemVisual.Instance.HideAllGridPos();
            return;
        }

        if (selectUnit != unit)
        {
            selectUnit = unit;

            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
            GridSystemVisual.Instance.HideAllGridPos();

            UpdateActionsButtons(this, EventArgs.Empty);

            //��ʾ��ɫ��action��Χ

            // GridSystemVisual.Instance.UpdateGridVisual();
            GridSystemVisual.Instance.UpdateCombineGridVisual();
            return;
        }
    }

    public void UnSetSelectedUnit()
    {
        selectUnit = null;
        GridSystemVisual.Instance.HideAllGridPos();
    }

    private void UpdateActionsButtons(object o, EventArgs eventArgs)
    {
        if (selectUnit == null) return;
        //��ý�ɫ�󶨵�����Action
        BaseAction[] baseActions = selectUnit.BaseActionArray;
        for (int i = 0; i < baseActions.Length; i++)
        {
            if (!baseActions[i].HasTaken)
            {
                SetUnitAction(baseActions[i]);
                return;
            }
        }
        SetUnitAction(null);
    }

    /// <summary>
    /// �����ť
    /// </summary>
    /// <param name="baseAction"></param>
    public void SetUnitAction(BaseAction baseAction)
    {
        if (baseAction != this.selectedAction)
        {
            this.selectedAction = baseAction;
            OnSelectedActionChanged?.Invoke(this.selectedAction, EventArgs.Empty);
        }
    }

    public Unit GetSelectedUnit()
    {
        return selectUnit;
    }
}
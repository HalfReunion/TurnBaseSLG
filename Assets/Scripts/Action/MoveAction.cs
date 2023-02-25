using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface TakeCombineGridPos
{
    public void GetCombineGridPos(out List<GridPos> gridPos1, out List<GridPos> gridPos2);
}

public class MoveAction : BaseAction, TakeCombineGridPos
{
    public event EventHandler OnStartMoving;

    public event EventHandler OnStopMoving;

    private Vector3 targetPosition;

    private GridPos targetGridPos;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float rotateSpeed;

    [SerializeField]
    private int moveLimit;

    [SerializeField]
    private int maxMoveLimit;

    [ShowInInspector, LabelText("普通范围")]
    private List<GridPos> moveGridPosList;

    [ShowInInspector, LabelText("冲刺范围")]
    private List<GridPos> maxMoveGridPosList;

    public override void TakeAction(GridPos targetGridPos)
    {
        //GridPos gridPos = unit.Vector2GridPos(targetPosition);
        if (!maxMoveGridPosList.Contains(targetGridPos)) return;
        if (IsDushMove(targetGridPos))
        {
            Debug.Log("冲刺！");
            actionPoint = 3;
        }
        //this.targetPosition = targetPosition;
        ActionStart();
        this.targetGridPos = targetGridPos;
        return;
    }

    //判断是否是冲刺范围，如果是则扣光行动点
    private bool IsDushMove(GridPos pos)
    {
        if (moveGridPosList.Contains(pos))
        {
            Debug.Log("在普通范围");
            return false;
        }
        Debug.Log("在冲刺范围");
        return true;
    }

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
        moveGridPosList = new List<GridPos>();
        maxMoveGridPosList = new List<GridPos>();
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPos);
        Vector3 pos = (targetPosition - transform.position).normalized;
        if (Vector3.Magnitude((targetPosition - transform.position)) > .1f)
        {
            //anim.SetBool("IsWalking", true);
            transform.position += pos * Time.deltaTime * moveSpeed;
            //aN.MoveAction_OnStartMoving(this, EventArgs.Empty);
            OnStartMoving?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            //anim.SetBool("IsWalking", false);
            OnStopMoving?.Invoke(this, EventArgs.Empty);
            ActionComplete();
        }

        if (pos != Vector3.zero)
        {
            Quaternion target = Quaternion.LookRotation(pos);
            transform.rotation = Quaternion.RotateTowards(
            transform.rotation, target, rotateSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 构建可移动范围
    /// 移动范围分为两种：1.冲刺范围,比普通移动范围要大,移动完无法使用其他指令
    /// 2.普通范围,移动完可以使用其他指令
    /// </summary>
    /// <returns></returns>
    public override List<GridPos> GetVaildActionGridPositionList()
    {
        //应该可以优化一下，不需要构建这么多次。
        //Debug.Log("构建范围");
        maxMoveGridPosList.Clear();
        moveGridPosList.Clear();
        //获得当前的格子
        GridPos currentUnitGridPos = unit.GetGridPosition();

        //根据移动力获得可移动范围
        //因为包括前面和后面，所以要从负数开始算
        for (int w = -maxMoveLimit; w <= maxMoveLimit; w++)
        {
            for (int h = -maxMoveLimit; h <= maxMoveLimit; h++)
            {
                //相对于目前正在踩着的位置
                GridPos offset = new GridPos(w, h);
                GridPos canGridPos = currentUnitGridPos + offset;
                //超出整个棋盘边界||角色原点||格子被占
                if (!unit.IsVaildGridPos(canGridPos)
                    || canGridPos == currentUnitGridPos
                    || unit.IsUnitOnGridPos(canGridPos)) continue;
                //Debug.Log(canGridPos);
                maxMoveGridPosList.Add(canGridPos);
            }
        }

        for (int w = -moveLimit; w <= moveLimit; w++)
        {
            for (int h = -moveLimit; h <= moveLimit; h++)
            {
                //相对于目前正在踩着的位置
                GridPos offset = new GridPos(w, h);
                GridPos canGridPos = currentUnitGridPos + offset;
                //超出整个棋盘边界||角色原点||格子被占
                if (!unit.IsVaildGridPos(canGridPos)
                    || canGridPos == currentUnitGridPos
                    || unit.IsUnitOnGridPos(canGridPos)) continue;
                //Debug.Log(canGridPos);
                moveGridPosList.Add(canGridPos);
            }
        }

        return maxMoveGridPosList;
    }

    public override string GetActionName()
    {
        return "MoveAction";
    }

    /// <summary>
    /// 1蓝色 2白色
    /// </summary>
    /// <param name="gridPos1"></param>
    /// <param name="gridPos2"></param>
    public void GetCombineGridPos(out List<GridPos> gridPos1, out List<GridPos> gridPos2)
    {
        List<GridPos> maxGridPos = new List<GridPos>();
        for (int j = 0; j < maxMoveGridPosList.Count; j++)
        {
            GridPos temp = maxMoveGridPosList[j];
            if (!moveGridPosList.Contains(temp))
            {
                maxGridPos.Add(temp);
            }
        }
        gridPos1 = maxGridPos;
        gridPos2 = moveGridPosList;
    }
}
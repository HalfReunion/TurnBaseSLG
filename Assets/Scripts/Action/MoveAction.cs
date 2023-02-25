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

    [ShowInInspector, LabelText("��ͨ��Χ")]
    private List<GridPos> moveGridPosList;

    [ShowInInspector, LabelText("��̷�Χ")]
    private List<GridPos> maxMoveGridPosList;

    public override void TakeAction(GridPos targetGridPos)
    {
        //GridPos gridPos = unit.Vector2GridPos(targetPosition);
        if (!maxMoveGridPosList.Contains(targetGridPos)) return;
        if (IsDushMove(targetGridPos))
        {
            Debug.Log("��̣�");
            actionPoint = 3;
        }
        //this.targetPosition = targetPosition;
        ActionStart();
        this.targetGridPos = targetGridPos;
        return;
    }

    //�ж��Ƿ��ǳ�̷�Χ���������۹��ж���
    private bool IsDushMove(GridPos pos)
    {
        if (moveGridPosList.Contains(pos))
        {
            Debug.Log("����ͨ��Χ");
            return false;
        }
        Debug.Log("�ڳ�̷�Χ");
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
    /// �������ƶ���Χ
    /// �ƶ���Χ��Ϊ���֣�1.��̷�Χ,����ͨ�ƶ���ΧҪ��,�ƶ����޷�ʹ������ָ��
    /// 2.��ͨ��Χ,�ƶ������ʹ������ָ��
    /// </summary>
    /// <returns></returns>
    public override List<GridPos> GetVaildActionGridPositionList()
    {
        //Ӧ�ÿ����Ż�һ�£�����Ҫ������ô��Ρ�
        //Debug.Log("������Χ");
        maxMoveGridPosList.Clear();
        moveGridPosList.Clear();
        //��õ�ǰ�ĸ���
        GridPos currentUnitGridPos = unit.GetGridPosition();

        //�����ƶ�����ÿ��ƶ���Χ
        //��Ϊ����ǰ��ͺ��棬����Ҫ�Ӹ�����ʼ��
        for (int w = -maxMoveLimit; w <= maxMoveLimit; w++)
        {
            for (int h = -maxMoveLimit; h <= maxMoveLimit; h++)
            {
                //�����Ŀǰ���ڲ��ŵ�λ��
                GridPos offset = new GridPos(w, h);
                GridPos canGridPos = currentUnitGridPos + offset;
                //�����������̱߽�||��ɫԭ��||���ӱ�ռ
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
                //�����Ŀǰ���ڲ��ŵ�λ��
                GridPos offset = new GridPos(w, h);
                GridPos canGridPos = currentUnitGridPos + offset;
                //�����������̱߽�||��ɫԭ��||���ӱ�ռ
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
    /// 1��ɫ 2��ɫ
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
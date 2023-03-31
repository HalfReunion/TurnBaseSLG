using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float totalSpinAmount;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);

        totalSpinAmount += spinAddAmount;

        if (totalSpinAmount >= 360f)
        {
            isActive = false;
            ActionComplete();
        }
    }

    public override void TakeAction(GridPos gridPos)
    {
        totalSpinAmount = 0f;
        Debug.Log("ִ����ת");
    }

    public override string GetActionName()
    {
        return "SpinAction";
    }

    public override List<GridPos> GetVaildActionGridPositionList()
    {
        //��õ�ǰ���ӵ���Ϣ
        GridPos unitGridPosition = unit.GetGridPosition();

        return new List<GridPos> { unitGridPosition };
    }

    public override int GetActionPointsCost()
    {
        return 2;
    }
}
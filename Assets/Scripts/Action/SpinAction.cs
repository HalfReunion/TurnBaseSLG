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
        Debug.Log("执行旋转");
    }

    public override string GetActionName()
    {
        return "SpinAction";
    }

    public override List<GridPos> GetVaildActionGridPositionList()
    {
        //获得当前格子的信息
        GridPos unitGridPosition = unit.GetGridPosition();

        return new List<GridPos> { unitGridPosition };
    }

    public override int GetActionPointsCost()
    {
        return 2;
    }
}
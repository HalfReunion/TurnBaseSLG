using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelGrid : MonoBehaviour
{
  
    [SerializeField]
    private Transform gridDebugObj;

    [SerializeField]
    private Transform parent;

    GridSystem gridSystem;

    private static LevelGrid instance;

    public static LevelGrid Instance => instance;

    public event EventHandler OnAnyUnitMovedGridPosition;

    public GridPos GetGridPosition(Vector3 worldPos) => gridSystem.GetGridPos(worldPos);

    public Vector3 GetWorldPosition(GridPos gridPos) => gridSystem.GetWorldPos(gridPos.x,gridPos.y);

    /// <summary>
    /// �ж��Ƿ���������
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    public bool IsVaildGridPos(GridPos gridPos) => gridSystem.IsVaildGridPos(gridPos); 

    public int Width => gridSystem.Width;
    public int Height => gridSystem.Height;
    private void Awake()
    {
        instance = this;
        gridSystem = new GridSystem(10,10,2f);
        //���ԣ����ڴ������ӻ�������
        gridSystem.CreateDebugObjects(gridDebugObj,parent);
        
    }

    /// <summary>
    /// �ڸ�������ӽ�ɫ
    /// </summary>
    /// <param name="gridPos"></param>
    /// <param name="unit"></param>
    public void AddUnitByGridPosition(GridPos gridPos, Unit unit)
    {
        Grid grid = gridSystem.GetGridByGridPosition(gridPos);
        grid.AddUnit(unit);
    }

    public Grid GetGridByGridPosition(GridPos gridPos)
    {
        Grid grid = gridSystem.GetGridByGridPosition(gridPos);
        return grid;
    }

    //��ò��ڸ����ϵ�����
    public List<Unit> GetUnitsByGridPosition(GridPos gridPos)
    {
        Grid grid = gridSystem.GetGridByGridPosition(gridPos);
        return grid.GetUnitList() ;
    }

    /// <summary>
    /// ��ø��ӵĵ�һ��Unit
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    public Unit GetUnitByGridPosition(GridPos gridPos)
    {
        Grid grid = gridSystem.GetGridByGridPosition(gridPos);
        return grid.GetUnitFirst(); 
    }

    //�ڸ������Ƴ���ɫ
    public void RemoveUnitByGridPosition(GridPos gridPos,Unit unit)
    {
        Grid grid = gridSystem.GetGridByGridPosition(gridPos);
        grid.RemoveUnit(unit);
    }

    public void UnitMoveToGridPosition(Unit unit,GridPos from,GridPos to) {
        RemoveUnitByGridPosition(from,unit);
        AddUnitByGridPosition(to,unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty); 
    }

    /// <summary>
    /// �ж��Ƿ���������ڸ�����
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    public bool IsUnitInGrid(GridPos gridPos) {
        return GetUnitsByGridPosition(gridPos).Count > 0;
    }
}

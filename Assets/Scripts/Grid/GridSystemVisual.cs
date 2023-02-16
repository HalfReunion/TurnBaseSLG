using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 处理角色的格子
/// </summary>
public class GridSystemVisual : MonoBehaviour
{

    public enum GridVisualType { 
        White,
        Blue,
        Red,
        Yellow 
    }

    [Serializable]
    public struct GridVisualTypeMaterial {
        public GridVisualType gridVisualType;
        public Material material;
    }

    [SerializeField] private Transform gridSystemVistualItem;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;


    GridSystemVisualItem[,] gridSystemVistualItems;

    private static GridSystemVisual instance;

    public static GridSystemVisual Instance => instance;

    private void Awake()
    {
        if (instance != null
            && instance != this)
        {
            Destroy(gameObject);
        }

        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        SetMoveGrid();
        //TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
        EventDispatcher.Instance.SubScribe<object, EventArgs>(GameEventType.OnTurnChanged,
            OnTurnChanged);
    }

    private void SetMoveGrid()
    {
        int width = LevelGrid.Instance.Width;
        int height = LevelGrid.Instance.Height;
        gridSystemVistualItems = new GridSystemVisualItem[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 pos = LevelGrid.Instance.GetWorldPosition(new GridPos(i, j));

                Transform itemTran = Instantiate(gridSystemVistualItem, pos, Quaternion.identity);

                gridSystemVistualItems[i, j] = itemTran.GetComponent<GridSystemVisualItem>();
                gridSystemVistualItems[i, j].Hide();
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;

        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {

    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    public void ShowGridPositionList(List<GridPos> list,GridVisualType type)
    {
        Material material = GetGridVisualTypeMaterial(type);
            foreach (var pos in list)
            {   
                gridSystemVistualItems[pos.x, pos.y].Show(material);
            } 
    }

    private void OnTurnChanged(object sender, EventArgs msg)
    {
        HideAllGridPos();
    }

    public void HideAllGridPos()
    {
        int row = gridSystemVistualItems.GetLength(0);
        int col = gridSystemVistualItems.GetLength(1);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                gridSystemVistualItems[i, j].Hide();
            }
        }
    }

    /// <summary>
    /// 刷新Action的Range
    /// </summary>
    public void UpdateGridVisual()
    {
        HideAllGridPos();
        GridVisualType type;
        BaseAction baseAction = UnitActionSystem.Instance.SelectedAction;
        if (baseAction == null) return;
        switch (baseAction) {
            default:
            case MoveAction moveAction:
                type = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                type = GridVisualType.Red;
                break; 
        }
            

        List<GridPos> range = baseAction.GetVaildActionGridPositionList();
        ShowGridPositionList(range, type);
    }

    public void UpdateCombineGridVisual()
    {
        HideAllGridPos();
        BaseAction baseAction = UnitActionSystem.Instance.SelectedAction;
        if (baseAction == null||baseAction is not TakeCombineGridPos) return;
         
        List<GridPos> range;
        List<GridPos> otherRange;
        ((TakeCombineGridPos)baseAction).GetCombineGridPos(out range,out otherRange);
        if (range.Count > 0 && otherRange.Count > 0) {
            ShowGridPositionList(range, GridVisualType.White);
            ShowGridPositionList(otherRange,GridVisualType.Blue);
        }
    }


    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType) {
        foreach (var gridVisualTypeMaterial in gridVisualTypeMaterialList) {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) {
                return gridVisualTypeMaterial.material;
            }
        }

        return null;
    }
}
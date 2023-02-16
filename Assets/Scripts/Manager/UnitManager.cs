using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitManager : MonoBehaviour
{
    //测试 先放开 后期用程序加
    [SerializeField]
    private List<Unit> unitList = new List<Unit>(); 
    [SerializeField]
    private List<Unit> enemyUnitList = new List<Unit>();
    
    public List<Unit> UnitList => unitList;

    private List<Unit> friendlyUnitList = new List<Unit>();

    private static UnitManager instance;
    public static UnitManager Instance => instance;

    /// <summary>
    /// 状态栏UI存储点
    /// </summary>
    private Dictionary<Unit, StatusPanelUI> statusDict = new Dictionary<Unit, StatusPanelUI>();


    #region 资源,以后使用资源加载器加载
    [SerializeField]
    private GameObject StatusItem;

    [SerializeField]
    private Transform StatusPanelLayout;

    [SerializeField]
    private Transform CharacterPanelLayout;
    #endregion

    private void Awake()
    {
        if (instance != null) {
            Destroy(gameObject);
        }
        instance = this; 
        DontDestroyOnLoad(gameObject);
        //unitList = new List<Unit>();
        //friendlyUnitList = new List<Unit>();
        //enemyUnitList = new List<Unit>(); 
    }

    /// <summary>
    /// 装载
    /// </summary>
    public void SetUpUnits() { 
        
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead; 
    }

    public void RenderStatus() {
        //测试加载,加载的是状态栏（血条,buff等的UI容器）
        addStatusUIByUnits(unitList);
        addStatusUIByUnits(enemyUnitList);
    }

    private void addStatusUIByUnits(List<Unit> units) {
        for (int i = 0; i < units.Count; i++) {
            StatusPanelUI statusUI = Instantiate(StatusItem, StatusPanelLayout).GetComponent<StatusPanelUI>();
            statusUI.Init(units[i]);
            statusDict.Add(units[i], statusUI);
        }
    }

    private void addCharSelectUIByUnits(List<Unit> units)
    {
        for (int i = 0; i < units.Count; i++)
        {
            StatusPanelUI statusUI = Instantiate(StatusItem, StatusPanelLayout).GetComponent<StatusPanelUI>();
            statusUI.Init(units[i]);
            statusDict.Add(units[i], statusUI);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        unitList.Remove(unit);
        Destroy(statusDict[unit].gameObject);
        statusDict.Remove(unit);
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
         
    }

    public List<Unit> GetUnitListByTurn(Camp camp) {
        switch (camp) {
            case Camp.Player:
                return unitList;
            case Camp.Enemy:
                return enemyUnitList;
            case Camp.Teammate:
                return friendlyUnitList; 
        }
        return null;
    }

}

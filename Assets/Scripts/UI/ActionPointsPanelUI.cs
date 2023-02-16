using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ActionPointsPanelUI : MonoBehaviour
{
    [SerializeField] ActionPoint actionPoints;

    List<ActionPoint> listPoints;

    CanvasGroup canvasGroup;

    bool isSelectedUnit = true;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        listPoints = new List<ActionPoint>();
        UnitActionSystem.Instance.OnSelectedUnitChanged += OnTurnChanged;
        UnitActionSystem.Instance.OnActionPointsChanged += CurrentUpdateActionPoints;
        //TurnSystem.Instance.OnTurnChanged += Init;
        //TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
        EventDispatcher.Instance.SubScribe<object, EventArgs>(GameEventType.OnTurnChanged,
            OnTurnChanged);
        
    }


    /// <summary>
    /// ActionPoint�ĳ�ʼ��
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="eventArgs"></param>
    public void Init()
    {
        //TODO:�������--> 1:Unitԭ����10������,������5��������5������Ϊ��ɫ��5����ɫ
        //2:Unitֻ��5��������
        Unit selected = UnitActionSystem.Instance.SelectUnit;
        if (selected == null) { isSelectedUnit = false; return; }
        isSelectedUnit = true;
        int points = selected.ActionPoints;
        int currentPoints = selected.CurrentActionPoints;

        if (listPoints.Count == 0)
        {
            for (int i = 0; i < points; i++)
            {
                ActionPoint point = Instantiate(actionPoints, transform);
                point.Resume();
                listPoints.Add(point);
            }
            return;
        }

        if (points <= listPoints.Count)
        {
            for (int i = 0; i < listPoints.Count; i++)
            {
                if (i > points - 1) { listPoints[i].gameObject.SetActive(false); continue; };

                listPoints[i].gameObject.SetActive(true);
            }
        }

        if (points > listPoints.Count)
        {
            int cnt = points - listPoints.Count;
            for (int i = 0; i < cnt; i++)
            {
                ActionPoint point = Instantiate(actionPoints, transform);
                listPoints.Add(point);
            }
        }

        CurrentUpdateActionPoints(currentPoints);
    }

    /// <summary>
    /// ��ǰ��ɫ�ж�ʱ��ActionPoint�䶯
    /// </summary>
    /// <param name="current"></param>
    private void CurrentUpdateActionPoints(int current) {
        for (int i = 0; i < listPoints.Count; i++) {
            if (i > current - 1) {
                listPoints[i].Spent();
                continue;
            }
            listPoints[i].Resume();
        }
    }

    public void OnTurnChanged(object sender,EventArgs e) {
        Init();
        ShowOrHide();
    }

    public void ShowOrHide() {
        bool isShow = TurnSystem.Instance.IsPlayerTurn() && isSelectedUnit;
        if (isShow) {
            canvasGroup.alpha = 1;
            return;
        }
        canvasGroup.alpha = 0;
    }
    


}

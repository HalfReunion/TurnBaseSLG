using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class ConfirmActionBtnUI : MonoBehaviour
{
    private Button btn;
    GridPos pos;
    BaseAction baseAction;
    private RectTransform rect;

    [SerializeField]
    private bool isShow;
    private void Awake()
    {
        isShow = false;
        btn = GetComponent<Button>();
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        BaseAction.OnConfirmAction += BaseAction_ShowPanel;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnHide;
        btn.onClick.AddListener(() =>
        {
            baseAction.TakeAction(pos);
            OnHide();
        });
        //OnHide();
    }

    private void UnitActionSystem_OnHide(object sender,EventArgs e) {
        if (isShow) { 
            OnHide();
        }
    }

    private void BaseAction_ShowPanel(object sender, GridPos gridPos)
    {
        Debug.Log("°ó¶¨");
        OnShow();
        this.pos = gridPos;
        this.baseAction = (BaseAction)sender;
    }
    private void OnShow()
    {
        //¶¯»­
        if (!isShow)
        { 
            //btn.enabled = false;
            rect.DOAnchorPos(new Vector2(0 , rect.anchoredPosition.y), 0.1f).OnComplete(
                () =>
                {
                    isShow = true;
                    //btn.enabled = true;
                });

        }
    }
    private void OnHide()
    {
        if (isShow)
        {
            //btn.enabled = false;
            //¶¯»­
            rect.DOAnchorPos(new Vector2(-rect.rect.width, rect.anchoredPosition.y), 0.1f).OnComplete(
                () => {
                    isShow = false;
                    //btn.enabled = true;
                });
        }
    }
}

using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
 
using UnityEngine;


/// <summary>
/// 只用于管理布局
/// </summary>
public class BuffPanelLayout : MonoBehaviour
{
    private enum BuffPanelMode
    {
        NormalMode,
        FocusMode
    }

    RectTransform parent;

    [SerializeField]
    private float itemWidth;
    [SerializeField]
    private float itemHeight;

    [SerializeField]
    private float itemTouchWidth;
    [SerializeField]
    private float itemTouchHeight;

    private BuffPanelUI buffUI;

    [SerializeField]
    int row, column;

    private bool isDirty;

    List<BuffItemUI> buffItemUIs;

    private BuffPanelMode LayoutMode;
    

    // Start is called before the first frame update
    void Awake()
    {
        parent = GetComponent<RectTransform>(); 
        LayoutMode = BuffPanelMode.NormalMode;
        buffUI = GetComponent<BuffPanelUI>();
    }
    private void Start()
    {
        buffUI.BuffPanelLayoutDirty += setDirty;
    }

    private void setDirty(List<BuffItemUI> buffItemUIs) {
        if (buffItemUIs == null||buffItemUIs.Count<=0) return;
        this.buffItemUIs = buffItemUIs;
        isDirty = true;
    }

    private void setNormalPanelRowColumn()
    {
        parent = GetComponent<RectTransform>();
        float panelWidth = parent.rect.width;
        float panelheight = parent.rect.height;
        column = (int)(panelWidth / itemWidth);
        row = (int)(panelheight / itemHeight);
    }

   

    private void ChangeLayoutMode() { 
        isDirty = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (LayoutMode)
        {
            case BuffPanelMode.NormalMode:
                {
                    if (isDirty) {
                        setNormalPanelRowColumn();
                        resetNormalLayoutMode();
                        isDirty = false;
                    } 
                    break;
                }
        }
    }

    private void resetNormalLayoutMode()
    {
        for (int i = 0; i < buffItemUIs.Count; i++)
        {
            RectTransform rect = buffItemUIs[i].GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(itemWidth, itemWidth);

            int nRow = (int)(itemHeight * (i / column));
            int nColumn = (int)(itemHeight * (i % column));
            rect.anchoredPosition = new Vector2(nColumn, -nRow);
        }
    }
}

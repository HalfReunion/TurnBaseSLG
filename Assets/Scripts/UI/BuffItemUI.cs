using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuffItemUI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public static EventHandler onMouseOverItem;
    public static EventHandler onMouseNotOverItem;
    private BuffPanelLayout layout;
    private BaseBuff baseBuff;


    private void Awake()
    {
        layout = GetComponent<BuffPanelLayout>();
    }

   

    public void OnPointerExit(PointerEventData eventData)
    {
        //onMouseNotOverItem(this, EventArgs.Empty);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //onMouseOverItem(this, EventArgs.Empty);
    }

    /// <summary>
    /// 初始化BuffItem的图标
    /// </summary>
    /// <param name="baseBuff"></param>
    public void SetBuffItem(BaseBuff baseBuff) { 
        
    }
   
}

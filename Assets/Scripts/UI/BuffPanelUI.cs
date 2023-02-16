using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffPanelUI : MonoBehaviour
{
    StatusPanelUI parentUI;

    [SerializeField, LabelText("≤‚ ‘Item")]
    private BuffItemUI TestItem;
    
    List<BuffItemUI> buffItemUIs;

    public event Action<List<BuffItemUI>> BuffPanelLayoutDirty;

    private void Awake()
    {
        parentUI = GetComponentInParent<StatusPanelUI>();
        buffItemUIs = new List<BuffItemUI>();
    }
      

    
    // Start is called before the first frame update
    private void Start()
    {
        BuffItemUI.onMouseOverItem += FocusBuffItem;
        BuffManager.Instance.BroadBuffChangedUI += RefreshBuffUI;
    }

    private void RefreshBuffUI(object obj, Dictionary<Unit, List<BaseBuff>> e) {
        if (e.ContainsKey(parentUI.Unit)) {
            ClearAllBuffItem();
            List<BaseBuff> list = e[parentUI.Unit]; 
            for (int i = 0; i < list.Count; i++) {
                BuffItemUI item = Instantiate(TestItem,transform);
                item.SetBuffItem(list[i]);
                buffItemUIs.Add(item);
            }
        }

        BuffPanelLayoutDirty(buffItemUIs);
    }

    private void ClearAllBuffItem() {
        buffItemUIs.Clear();
    }

    private void DestroyBuffItem() { 
        
    }

    private void FocusBuffItem(object obj, EventArgs e)
    {
        Debug.Log((obj as MonoBehaviour).name);
    }

    private void RestoreBuffItem(object obj, EventArgs e)
    {

    }
    public void AddBuffItem(BaseBuff baseBuff)
    {

    }

    public void RefrshBuffItem(BaseBuff baseBuff)
    {

    }

}

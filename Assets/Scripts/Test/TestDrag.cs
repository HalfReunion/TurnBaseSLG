using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestDrag : MonoBehaviour,IDragHandler,IEndDragHandler,IBeginDragHandler
{
    
    public TestDragPanel manager;
    private Vector2 startPos;
    private RectTransform rect;
    private RectTransform parentRect;

    public int offset;
    private Camera mainCamera;
    public Transform parent; 

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        parentRect = parent.GetComponent<RectTransform>();
       
        mainCamera = Camera.main;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = rect.anchoredPosition;
        rect.GetComponent<Image>().raycastTarget = false;
        manager.SetDragging(this);
    }

    public void OnDrag(PointerEventData eventData)
    { 
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, mainCamera, out Vector2 v2);
        rect.anchoredPosition = v2;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        manager.UnsetDragging();
        rect.GetComponent<Image>().raycastTarget = true;
        rect.anchoredPosition = startPos;
        GameObject obj = eventData.pointerCurrentRaycast.gameObject;
        Debug.Log(obj.name);
        if (obj == gameObject || parent.gameObject == obj) return;

        int end = obj.GetComponent<TestDrag>().offset;
        manager.Swap(offset, end); 
    }

    public void SwapChild(Vector2 v) {
        parentRect.anchoredPosition = v;
    }

    public Vector2 GetAnRect() {
        return parentRect.anchoredPosition;
    }
  
}

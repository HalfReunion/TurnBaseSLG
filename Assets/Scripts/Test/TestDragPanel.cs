using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDragPanel : MonoBehaviour
{
    [SerializeField]
    List<TestDrag> list;
    private TestDrag dragging;
    // Start is called before the first frame update
    public void SetDragging(TestDrag dragging) {
        this.dragging = dragging;
    }

    public void UnsetDragging()
    {
        this.dragging = null;
    }

    public void Swap(int start,int end) {
        Vector2 startV = list[start].GetAnRect();
        Vector2 endV = list[end].GetAnRect();

        list[start].SwapChild(endV);
        list[end].SwapChild(startV);
        list[start].offset = end;
        list[end].offset = start; 
    }
}

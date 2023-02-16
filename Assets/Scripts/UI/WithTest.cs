using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithTest : MonoBehaviour
{
    public Transform test;
    public Camera camera;
    public Camera uiCamera;
    public RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 v = RectTransformUtility.WorldToScreenPoint(camera, test.position);
        Vector3 res= ScreenTool.ScreenPointToUIPoint(rect,uiCamera, v);
        transform.position = res;
    }

    
}

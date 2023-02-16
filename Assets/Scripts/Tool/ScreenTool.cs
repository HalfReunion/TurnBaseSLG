using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
 
    public class ScreenTool
    {
        public static Vector3 ScreenPointToUIPoint(RectTransform rt, Camera uiCamera ,Vector2 screenPoint)
        {
            Vector3 globalMousePos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, screenPoint, uiCamera, out globalMousePos);
            return globalMousePos;
        }
}
 

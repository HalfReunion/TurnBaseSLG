using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{ 
    // Start is called before the first frame update
     
    private void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            GridSystemVisual.Instance.HideAllGridPos();
            List<GridPos> list = UnitActionSystem.Instance.SelectUnit.gameObject.GetComponent<MoveAction>().GetVaildActionGridPositionList();

            //GridSystemVisual.Instance.ShowGridPositionList(list);
        }    
    
    }

}

using TMPro;
using UnityEngine;

public class GridDebugObj : MonoBehaviour
{
    private Grid grid;

    [SerializeField]
    private TextMeshPro txt;

    public void SetGrid(Grid grid)
    {
        this.grid = grid;
    }

    private void Update()
    {
        //if (grid.CurrentUnit != null) {
        //    Debug.Log($"人物：{grid.CurrentUnit.name}在{this.name}上");
        //}
        txt.text = grid.ToString();
    }
}
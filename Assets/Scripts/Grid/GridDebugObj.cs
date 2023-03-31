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
        //    Debug.Log($"���{grid.CurrentUnit.name}��{this.name}��");
        //}
        txt.text = grid.ToString();
    }
}
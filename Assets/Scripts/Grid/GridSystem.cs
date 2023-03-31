using UnityEngine;

public class GridSystem
{
    //����ս���Ŀ��
    private int width;

    private int height;
    private float cellSize;

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;
    private Grid[,] grids;

    public GridSystem(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        grids = new Grid[width, height];
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                GridPos gridPos = new GridPos(w, h);
                grids[w, h] = new Grid(this, gridPos);
            }
        }
    }

    public Vector3 GetWorldPos(int w, int h)
    {
        Vector3 v = new Vector3(w, 0, h) * this.cellSize;
        return v;
    }

    /// <summary>
    /// �������������ȡ���ӵ�λ����Ϣ,�������һ����Input.mousePosition
    /// </summary>
    /// <param name="wPos"></param>
    /// <returns></returns>
    public GridPos GetGridPos(Vector3 wPos)
    {
        return new GridPos(
            Mathf.RoundToInt(wPos.x / cellSize),
            Mathf.RoundToInt(wPos.z / cellSize)
            );
    }

    /// <summary>
    /// ���ڴ������ӻ�������
    /// </summary>
    /// <param name="debugPrefab"></param>
    /// <param name="parent"></param>
    public void CreateDebugObjects(Transform debugPrefab, Transform parent)
    {
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                Transform tr = GameObject.Instantiate(debugPrefab, GetWorldPos(w, h), Quaternion.identity, parent);
                tr.gameObject.name = $"({w},{h})";
                GridDebugObj debugObj = tr.GetComponent<GridDebugObj>();
                debugObj.SetGrid(grids[w, h]);
            }
        }
    }

    /// <summary>
    /// ���ݸ��ӷ�λ��ø���
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Grid GetGridByGridPosition(GridPos pos)
    {
        if (grids[pos.x, pos.y] != null)
        {
            return grids[pos.x, pos.y];
        }
        return null;
    }

    /// <summary>
    /// �ж��Ƿ���������
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    public bool IsVaildGridPos(GridPos gridPos)
    {
        return gridPos.x >= 0 &&
            gridPos.y >= 0 &&
            gridPos.x < width &&
            gridPos.y < height;
    }
}
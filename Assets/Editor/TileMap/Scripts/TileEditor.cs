using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TileEditor : EditorWindow
{
    //用于放置生成的场景物体
    private static GameObject objInstance;

    // 为让编辑器可以滚动
    private Vector2 scrollPos;

    private float windowWidth; // 窗口宽度
    private string tempPath = "";  // 记录使用者最后选取的资料夹

    #region 地图方块部分

    private string mapInfoFilePath = "Assets/MapCreator/TileMap/TileMapItems.json";
    private static List<GameObject> mapCubeItemPrefabs = new List<GameObject>(); //map cube list
    private static List<string> mapCubeItemNames = new List<string>();  //map cube list name
    private static List<Texture> mapCubeItemIcon = new List<Texture>(); //map cube icon
    private static int selNum = 0;      // selected item num
    private static float iconSize = 80; // icon size

    #endregion 地图方块部分

    #region 图层区变量

    private static List<GameObject> layerObjs = new List<GameObject>(); // 通过不同的GameObject来划分图层
    private static List<string> layerNames = new List<string>();        // 图层名称
    private static bool newLayer = false;
    private static bool editLayer = false;
    private static string layerName = "";
    private static int layerHeight = 0;
    private static int selLayer = 0;  // 所选取的图层下标

    #endregion 图层区变量

    #region 地图绘制区变量

    private static bool mouseDown = false; //监控鼠标是否被点击
    private static int mapUnitSize = 1; // 地图单位大小
    private static Color cursorColor = Color.yellow; //参考线颜色
    private static bool showGrid = true;// 是否显示参考线
    private static int gridSize = 9;    // 参考线数量
    private static bool clearOver = false;  // 自动清除不同图层的重叠方块
    private static bool replaceItem = true; // 若为true 则当该地已有方块时以新取代旧
    private static List<Dictionary<Vector3Int, GameObject>> mapDics = new List<Dictionary<Vector3Int, GameObject>>();   // 地图工厂
    private static bool OnPainting = false; // 是否开始绘制地图

    #endregion 地图绘制区变量

    #region 存档区变量
    private static string tempFileName = "";
    #endregion  存档区变量


    public void OnGUI()
    {
        Input.imeCompositionMode = IMECompositionMode.On;  //让输入文字支持中文

        Texture2D texture = new Texture2D(1, 1);         //加这行才能显示格子线
        windowWidth = position.width - 20;              //滚轮的宽度

        GUIStyle boxStyle = new GUIStyle(GUI.skin.box); // set a box style
        boxStyle.normal.textColor = Color.white;        //set the style's text color

        boxStyle.fixedWidth = windowWidth;

        GUILayout.Label("Tile Map Editor");
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true,       //展开滚动窗
            GUILayout.Height(position.height - 20));

        {
            #region 显示地图组件

            GUILayout.BeginVertical(GUILayout.Width(windowWidth));
            {
                GUILayout.Space(10);
                GUILayout.Box("地图组件", boxStyle);

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("加入地图组件"))
                    {
                        addMapCubeItem();
                    }

                    if (GUILayout.Button("删除地图组件"))
                    {
                        removeMapItem();
                    }

                    if (GUILayout.Button("清理全部地图组件"))
                    {
                        removeAllMapItem();
                    }

                    //显示尺寸的Slider
                    iconSize = GUILayout.HorizontalSlider(iconSize, 40, 120, GUILayout.Width(60));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            displayMapItems();

            //显示地图Item列表

            #endregion 显示地图组件
        }
        EditorGUILayout.EndScrollView();
    }

    #region 地图组件方法

    //用于序列化的数据
    private class MapItem
    {
        public string[] names;

        public MapItem(string[] names)
        {
            this.names = new string[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = this.names[i];
            }
        }
    }

    private void saveMapInfoData()
    {
        // 写一个序列化保存的方法
        LocalFileTool.SaveFileInEditor(mapCubeItemNames.ToArray(), mapInfoFilePath);
    }

    private void buildMapInfoData()
    {
        clear();
        // 反序列化数据,构建地图信息
        selNum = 0;

        string res = LocalFileTool.LoadJsonFile(mapInfoFilePath);
        if (res != null)
        {
            MapItem mapItem = SerializeTool.Deserialize<MapItem>(res);
            foreach (var i in mapItem.names)
            {
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(i, typeof(GameObject));   // 编辑器专用
                mapCubeItemPrefabs.Add(obj);
                mapCubeItemNames.Add(i);
                mapCubeItemIcon.Add(AssetPreview.GetAssetPreview(obj));             // 获得资源省略图
            }
        }
    }

    private void addMapCubeItem()
    {
        string defaultPath = tempPath;
        if (defaultPath == "") defaultPath = Application.dataPath;

        // 开启一个文件选择框
        string prefabFull = EditorUtility.OpenFilePanel("选取地图元件", defaultPath, "prefab,fbx");
        if (string.IsNullOrEmpty(prefabFull)) return;
        //记录下当前的文件夹
        tempPath = Path.GetDirectoryName(prefabFull);

        string prefabShort = $"Assets{prefabFull.Substring(Application.dataPath.Length)}";
        GameObject bk = (GameObject)AssetDatabase.LoadAssetAtPath(prefabShort, typeof(GameObject));

        mapCubeItemPrefabs.Add(bk);
        mapCubeItemNames.Add(prefabShort);
        // huode
        mapCubeItemIcon.Add(AssetPreview.GetAssetPreview(bk));
        saveMapInfoData();
    }

    private void removeMapItem()
    {
        if (mapCubeItemPrefabs.Count == 0) return;

        if (!EditorUtility.DisplayDialog("删除地图组件", $"确定删除组件:{mapCubeItemNames[selNum]}?", "确定", "取消")) return;

        mapCubeItemIcon.Remove(mapCubeItemIcon[selNum]);
        mapCubeItemNames.Remove(mapCubeItemNames[selNum]);
        mapCubeItemPrefabs.Remove(mapCubeItemPrefabs[selNum]);
        selNum = 0;
        saveMapInfoData();
    }

    private void removeAllMapItem()
    {
        if (!EditorUtility.DisplayDialog("删除所有地图组件", $"删除所有地图组件?", "确定", "取消")) return;
        mapCubeItemIcon.Clear();
        mapCubeItemNames.Clear();
        mapCubeItemPrefabs.Clear();
        selNum = 0;
        saveMapInfoData();
    }

    /// <summary>
    /// 显示缩略图等
    /// </summary>
    private void displayMapItems()
    {
        if (mapCubeItemNames.Count > 0)
        {
            if (iconSize > 40.1f)        // 若要显示缩略图，根据Slider的iconSize
            {
                int xCount = (int)((windowWidth - 20) / iconSize);
                int lines = ((mapCubeItemNames.Count - 1) / xCount) + 1;
                // 创建grid 根据IconSize计算行列数，和格子大小
                // 参数 1:点击返回的下标 2:展示的数据的icon图标 3:每行中按钮的数量 4:
                selNum = GUILayout.SelectionGrid(selNum, mapCubeItemIcon.ToArray(),
                    xCount, GUILayout.Width(windowWidth - 20), GUILayout.Height(lines * iconSize));
            }
            else //缩略条小于，则显示详细文字列表
            {
                selNum = GUILayout.SelectionGrid(selNum, mapCubeItemNames.ToArray(), 1, GUILayout.Width(windowWidth - 20));
            }
        }
    }

    private void clear()
    {
        mapCubeItemPrefabs.Clear();
        mapCubeItemIcon.Clear();
        mapCubeItemNames.Clear();
    }

    #endregion 地图组件方法

    #region 系统函数

    private void OnFocus()  //重新获得焦点时调用
    {
        buildMapInfoData();
    }

    private void OnEnable()
    {
        checkInstance();
    }

    private void OnDisable()
    {
    }

    // 检查objInstance
    private void checkInstance()
    {
        if (objInstance == null)
        {
            objInstance = GameObject.Find("EditorObjInstance");
            if (objInstance == null)
            {
                objInstance = new GameObject("EditorObjInstance");
            }
        }
    }

    #endregion 系统函数
}
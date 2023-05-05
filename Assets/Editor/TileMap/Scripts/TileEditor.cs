using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MapTileCreator
{
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
        private string mapDetailFilePath = "Assets/MapCreator/TileMap/TileMapDetailInfo.json";
        

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
        private static Color gridColor = Color.gray;
        private static bool showGrid = true;// 是否显示参考线
        private static int gridSize = 9;    // 参考线数量
        private static bool clearOver = false;  // 自动清除不同图层的重叠方块
        private static bool replaceItem = true; // 若为true 则当该地已有方块时以新取代旧
        private static List<Dictionary<Vector3Int, GameObject>> mapDics = new List<Dictionary<Vector3Int, GameObject>>();   // 地图方块资料
        private static List<Dictionary<Vector3Int, int>> costDics = new List<Dictionary<Vector3Int, int>>();   // 地图代价

        private static bool OnPainting = false; // 是否开始绘制地图

        #endregion 地图绘制区变量

        #region 路径 编辑变量

        private static bool mapTileDetail = false; // 方块详情界面
        private static List<MapCubeTileDetail> mapCubeTileDetails = new List<MapCubeTileDetail>();

        private List<MapTilePathItem> tiles = new List<MapTilePathItem>();
        private MapTilePathItem[,] mapTiles;

        #endregion 路径 编辑变量

        #region 存档区变量

        private static string tempFileName = "";

        #endregion 存档区变量

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

                // 编辑方块的详情
                if (selNum >= 0)
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("方块详情", boxStyle);
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("方块代价", GUILayout.Width(100));
                            mapCubeTileDetails[selNum].cost = EditorGUILayout.IntField(mapCubeTileDetails[selNum].cost, GUILayout.Width(60));
                            GUILayout.Space(10);
                            GUILayout.Label("该方块是否转换层数", GUILayout.Width(100));
                            mapCubeTileDetails[selNum].isTran = EditorGUILayout.Toggle(mapCubeTileDetails[selNum].isTran, GUILayout.Width(60));
                        }
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.EndVertical();
                }

                //显示地图Item列表

                #endregion 显示地图组件

                #region 图层管理

                GUILayout.BeginVertical(GUILayout.Width(windowWidth));
                {
                    GUILayout.Space(10); //10 pixel空格
                    GUILayout.Box("图层", boxStyle);
                    GUILayout.BeginHorizontal();
                    {
                        if (!newLayer && !editLayer) // 不是新增或编辑图层才能显示按钮
                        {
                            if (GUILayout.Button("新增图层"))
                            {
                                addMapLayer();
                            }

                            if (GUILayout.Button("编辑图层"))
                            {
                                modifyMapLayer();
                            }

                            if (GUILayout.Button("删除图层"))
                            {
                                removeMapLayer();
                            }
                        }
                        if (newLayer) // 新增图层状态
                        {
                            addMapLayerImpl();
                        }
                        else if (editLayer) // 编辑图层状态
                        {
                            modifyMapLayerImpl();
                        }
                    }

                    GUILayout.EndHorizontal();

                    displayLayerList();
                }
                GUILayout.EndVertical();

                #endregion 图层管理

                #region 地图制作区

                GUILayout.BeginVertical();
                {
                    GUILayout.Space(10);
                    GUILayout.Box("地图制作", boxStyle);

                    //设定单位尺寸
                    SettingUnitSize();
                    //设定参考线尺寸
                    SettingGrid();
                    GUILayout.BeginHorizontal();
                    {
                        clearOver = GUILayout.Toggle(clearOver, "自动清除不同图层的地图方块");
                        replaceItem = GUILayout.Toggle(replaceItem, "自动替换");
                    }
                    GUILayout.EndHorizontal();

                    if (OnPainting) // 绘制状态中
                    {
                        GUIStyle btnStyle = new GUIStyle(GUI.skin.button); // 取得预设按钮的外观
                        btnStyle.normal.textColor = Color.yellow;           // 把字改成黄字
                        btnStyle.normal.background = makeTex(10, 10, Color.blue); // 按钮底色
                        if (GUILayout.Button("按下停止绘制地图", btnStyle))
                        {
                            OnPainting = false;
                        }
                    }
                    else
                    {
                        GUIStyle btnStyle = new GUIStyle(GUI.skin.button); // 取得预设按钮的外观
                        btnStyle.normal.textColor = Color.black;           // 把字改成黄字
                        btnStyle.normal.background = makeTex(10, 10, Color.white); // 按钮底色
                        if (GUILayout.Button("按下开始绘制", btnStyle))
                        {
                            OnPainting = true;
                        }
                    }

                    GUILayout.Space(10);
                    GUILayout.Box("地图数据", boxStyle);
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("新地图"))
                        {
                            createNewMap();
                        }
                        if (GUILayout.Button("存储"))
                        {
                            saveMapData();
                        }
                        if (GUILayout.Button("加载数据"))
                        {
                            loadMapData();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                #endregion 地图制作区
            }

            GUILayout.BeginVertical();
            {
                GUILayout.Space(10f);
                GUILayout.Box("烘焙路径", boxStyle);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("点击烘焙路径"))
                {
                    bakePath();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 烘焙路径
        /// </summary>
        private void bakePath()
        {
            mapTiles = new MapTilePathItem[gridSize, gridSize];

            //for (int i = 0; i < gridSize; i++)
            //{
            //    for (int j = 0; j < gridSize; j++)
            //    {
            //    }
            //}

            foreach (var io in tiles)
            { 
                    Vector3Int v3 = posChange(io.x, 0,io.z);
                    mapTiles[v3.x, v3.z] = io;
            }

            string str = SerializeTool.SerializeToFile(mapTiles);

            string defaultPath = tempPath;
            if (defaultPath == "") defaultPath = Application.dataPath;

            string fileName = EditorUtility.SaveFilePanel("存储数据", defaultPath, tempFileName, "json");
            LocalFileTool.SaveFileInEditor(str, fileName);
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
            string it = SerializeTool.SerializeToFile(mapCubeTileDetails);
            LocalFileTool.SaveFileInEditor(it, mapDetailFilePath);
        }

        private void buildMapInfoData()
        {
            clear();
            // 反序列化数据,构建地图信息
            selNum = -1;

            string res = LocalFileTool.LoadJsonFile(mapInfoFilePath);
            string details = LocalFileTool.LoadJsonFile(mapDetailFilePath);
            if (res != null)
            {
                string[] names = SerializeTool.Deserialize<string[]>(res);
                foreach (var i in names)
                {
                    GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(i, typeof(GameObject));   // 编辑器专用
                    mapCubeItemPrefabs.Add(obj);
                    mapCubeItemNames.Add(i);
                    mapCubeItemIcon.Add(AssetPreview.GetAssetPreview(obj));             // 获得资源省略图
                }
            }

            if (details != null)
            {
                mapCubeTileDetails = SerializeTool.Deserialize<List<MapCubeTileDetail>>(details);
            }
            Repaint();
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
            addMapCubeDetail();
            saveMapInfoData();
            Repaint();
        }

        /// <summary>
        /// 根据地图组件，加入地图详细数据
        /// </summary>
        private void addMapCubeDetail()
        {
            MapCubeTileDetail cube = new MapCubeTileDetail();
            mapCubeTileDetails.Add(cube);
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
            mapCubeTileDetails.Clear();
            selNum = -1;
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

            //需要修改一下，因为会清空东西
            mapCubeTileDetails.Clear();
        }

        #endregion 地图组件方法

        #region 系统函数

        private void OnFocus()  //重新获得焦点时调用
        {
            checkInstance();
            buildMapInfoData();
            buildMapDic(); //重建地图字典
        }

        private void OnEnable()
        {
            //checkInstance();
            //加入一个SceneView控制的委托
            SceneView.duringSceneGui += this.OnSceneGUI;

            buildMapInfoData();
            checkInstance();
            buildMapDic(); //重建地图字典
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }

        // 检查objInstance，不存在则重建
        private void checkInstance()
        {
            // 不知道要不要销毁GameObject
            layerObjs.Clear();
            layerNames.Clear();
            if (objInstance == null)
            {
                mapDics.Clear();
                objInstance = GameObject.Find("EditorObjInstance");
                if (objInstance == null)
                {
                    objInstance = new GameObject("EditorObjInstance");

                    GameObject defaultLayer = new GameObject("Default");
                    defaultLayer.transform.SetParent(objInstance.transform);
                    layerNames.Add("Default");
                    layerObjs.Add(defaultLayer);
                    mapDics.Add(new Dictionary<Vector3Int, GameObject>()); // 列表下标为层级
                    selLayer = 0;
                }
                return;
            }

            // 若存在ObjInstance ，则构建数据
            // 搜索第一层子物体为图层信息
            Transform[] tr = objInstance.GetComponentsInChildren<Transform>();
            foreach (Transform t in tr)
            {
                if (t.parent == objInstance.transform)
                {
                    layerObjs.Add(t.gameObject);
                    layerNames.Add(t.name);
                    mapDics.Add(new Dictionary<Vector3Int, GameObject>());
                }
            }
            if (layerObjs.Count == 0) // 若没有子物体，则创建一个当预设图层
            {
                GameObject defaultLayer = new GameObject("Default");
                defaultLayer.transform.SetParent(objInstance.transform);
                layerNames.Add("Default");
                layerObjs.Add(defaultLayer);
                mapDics.Add(new Dictionary<Vector3Int, GameObject>()); // 列表下标为层级
                selLayer = 0;
            }
        }

        private bool MouseToWorldPos(Vector3 mousePos, int y, out Vector3 mouseWorldPos)
        {
            // 图层高度
            Vector3Int h = new Vector3Int(0, y, 0);

            Ray mouseRay = HandleUtility.GUIPointToWorldRay(mousePos); //获得射线，有点像Camera.ScreenPointToRay
                                                                       // y向量向下，代表鼠标的点击处在画面内
            if (mouseRay.direction.y <= 0)
            {
                mouseRay.origin -= h;
                float t = -mouseRay.origin.y / mouseRay.direction.y;

                // 取得点击后的世界坐标
                mouseWorldPos = mouseRay.origin + t * mouseRay.direction + h;
                return true;
            }
            mouseWorldPos = Vector3.zero;
            return false;
        }

        // 重建地图
        private void buildMapDic()
        {
            if (objInstance == null)
            {
                Debug.LogWarning("重建字典失败，Instance容器不存在");
                return;
            }

            for (int i = 0; i < layerNames.Count; i++)
            {
                mapDics[i].Clear(); // 清楚原有内存的数据
                Transform[] children = layerObjs[i].GetComponentsInChildren<Transform>(); // 取得所有子物体

                foreach (Transform b in children)
                {
                    if (b.parent == layerObjs[i].transform)  // 只使用第一层子物体
                    {
                        // 获得每个地块在网格的位置
                        Vector3Int posInt = VectorToInt(b.transform.position);
                        Vector3Int dicPos = new Vector3Int(posInt.x, 0, posInt.z);
                        b.transform.position = posInt;  // 把方块对齐
                        if (!mapDics[i].ContainsKey(dicPos))
                        {  // 若字典没有该地块
                            mapDics[i].Add(dicPos, b.gameObject);    // 写入字典
                        }
                    }
                }
            }
        }

        private void saveData(string fileName)
        {
            MapTileData data = new MapTileData();
            checkInstance(); // 重建数据
            buildMapInfoData();
            buildMapDic();

            Transform cam = Camera.main.transform;
            if (cam != null)
            {
                data.camPos = VectorToInt(cam.position);
                data.camRot = cam.eulerAngles;
                data.camFOV = Camera.main.GetComponent<Camera>().fieldOfView;
            }
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                data.playerPos = VectorToInt(player.transform.position);
                data.playerRot = player.transform.eulerAngles;
            }
            data.resBlocks = mapCubeItemNames.ToArray(); // 资源列表
            data.layerDatas = new MapTileLayer[layerObjs.Count()];

            for (int i = 0; i < layerObjs.Count; i++)
            {
                data.layerDatas[i].name = layerObjs[i].name;
                data.layerDatas[i].height = Mathf.RoundToInt(layerObjs[i].transform.position.y);
                data.layerDatas[i].blocks = new List<Block>();

                Transform[] children = layerObjs[i].GetComponentsInChildren<Transform>();

                foreach (var item in children)
                {    // 处理每个地图块
                    if (item.parent == layerObjs[i].transform)
                    {
                        // 1. 取得地板原始的Prefab
                        GameObject prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(item.gameObject);

                        // 2. 取得Prefab在有内硬盘空间存放的位置
                        string assetPath = AssetDatabase.GetAssetPath(prefab);

                        // 3. 找出相同的文档名即为编号
                        int index = 0;  // 资源 编号
                        for (int j = 0; j < data.resBlocks.Length; j++)
                        {
                            if (assetPath.ToLower() == data.resBlocks[j].ToLower())
                            {
                                index = j;  //  找到编号
                                break;      // 若找到编号，就取消
                            }
                        }

                        // 把搜索到的GameObject添加到要存储的block列表中
                        data.layerDatas[i].blocks.Add(new Block(index, VectorToInt(item.position), mapCubeTileDetails[index].cost));
                    }
                }
            }

            string str = SerializeTool.SerializeToFile(data);
            LocalFileTool.SaveFileInEditor(str, fileName);
        }

        private void loadData(string fileName)
        {
            MapTileData data = new MapTileData();
            string json = LocalFileTool.LoadJsonFile(fileName);
            data = SerializeTool.Deserialize<MapTileData>(json);

            Transform cam = Camera.main.transform;
            if (cam != null)
            {
                cam.position = data.camPos;
                cam.eulerAngles = data.camRot;
                Camera.main.GetComponent<Camera>().fieldOfView = data.camFOV;
            }

            // 玩家数据
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = data.playerPos;
                player.transform.eulerAngles = data.playerRot;
            }

            // 清空资源列表
            mapCubeItemPrefabs.Clear();
            mapCubeItemNames.Clear();
            mapCubeItemIcon.Clear();
            mapCubeTileDetails.Clear();
            selNum = -1;

            // 清空地图物件
            layerObjs.Clear();
            layerNames.Clear();
            mapDics.Clear();    // 清空字典
            DestroyImmediate(objInstance);

            // 读取资源
            foreach (var i in data.resBlocks)
            {
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(i, typeof(GameObject));
                mapCubeItemPrefabs.Add(obj);
                mapCubeItemNames.Add(i);
                mapCubeItemIcon.Add(AssetPreview.GetAssetPreview(obj));
            }
            saveMapInfoData(); // 存储列表到内存里

            objInstance = new GameObject(); // 创建新的容器

            objInstance.name = "EditorObjInstance";

            int dictNum = 0;
            foreach (var i in data.layerDatas)
            {
                // 建立图层
                GameObject lObj = new GameObject();
                lObj.name = i.name;
                lObj.transform.SetParent(objInstance.transform);
                lObj.transform.position = new Vector3(0, i.height, 0);
                layerObjs.Add(lObj); // 加入列表
                layerNames.Add(i.name);
                mapDics.Add(new Dictionary<Vector3Int, GameObject>());

                // 加载所有方块
                foreach (Block b in i.blocks)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(mapCubeItemPrefabs[b.index]);
                    obj.transform.position = b.pos;
                    obj.transform.SetParent(lObj.transform);
                    Vector3Int dicPos = new Vector3Int(b.pos.x, 0, b.pos.z);
                    mapDics[dictNum].Add(dicPos, obj); // 把物体加入字典
                }
                dictNum++;
            }
        }

        private Vector3Int VectorToInt(Vector3 origin)
        {  // 把Vector3 转变为Vector3Int
            int x = Mathf.RoundToInt(origin.x);
            int y = Mathf.RoundToInt(origin.y);
            int z = Mathf.RoundToInt(origin.z);
            return new Vector3Int(x, y, z);
        }

        private Texture2D makeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            // 创建一个纹理贴图，
            Texture2D result = new Texture2D(width, height);

            result.SetPixels(pix, 0);
            result.Apply();
            return result;
        }

        #endregion 系统函数

        #region SceneView 绘图用的函数

        [DrawGizmo(GizmoType.NonSelected)]
        private static void RenderGridGizmo(Transform objectTransform, GizmoType gizmoType)
        {
            if (layerObjs.Count == 0 || objInstance == null) return;
            if (showGrid)
            {
                int s = (gridSize + 1) / 2;
                int h = Mathf.RoundToInt(layerObjs[selLayer].transform.position.y);
                Gizmos.color = gridColor;
                float offset = 0.5f * mapUnitSize;
                for (int i = -s; i < s; i++)
                {
                    Gizmos.DrawLine(
                        new Vector3(-s * mapUnitSize + offset, h, i * mapUnitSize + offset),
                        new Vector3(s * mapUnitSize - offset, h, i * mapUnitSize + offset)
                        );

                    Gizmos.DrawLine(
                        new Vector3(i * mapUnitSize + offset, h, -s * mapUnitSize + offset),
                        new Vector3(i * mapUnitSize + offset, h, s * mapUnitSize - offset)
                        );
                }
            }
        }

        // SceneView绘制的实现,绘制场景测试UI
        private void OnSceneGUI(SceneView sceneView)
        {
            //Handles.BeginGUI();
            //{
            //    // 在SceneView里绘制UI
            //}
            //Handles.EndGUI();

            if (!OnPainting) return;

            int controlID = GUIUtility.GetControlID(FocusType.Passive);  // 拦截鼠标按下的信号
            Event e = Event.current; // 取得事件
            if (e.alt) return;      // 若按住[Alt]键，则表示使用者要旋转画面，不要进行绘制

            switch (e.GetTypeForControl(controlID))
            {  // 判断鼠标事件
                case EventType.MouseDown:       // 鼠标按下
                    if (e.button != 0) break; // 若按下的不是左键
                    GUIUtility.hotControl = controlID;  // 拦截信号
                    mouseDown = true;                   // 打开鼠标按下的标识

                    break;

                case EventType.MouseUp:             //  若拦截的是鼠标放开
                    if (e.button != 0) break;
                    GUIUtility.hotControl = 0;
                    mouseDown = false;
                    Event.current.Use();
                    break;
            }
            Vector3 pos = Vector3.zero;
            Vector3Int posInt = Vector3Int.zero;
            int dx = 0, dy = 0, dz = 0;

            bool mouseInWorld = MouseToWorldPos(
                e.mousePosition,
                Mathf.RoundToInt(layerObjs[selLayer].transform.position.y),
                out pos);

            // 绘制游标
            if (mouseInWorld)
            {     // 鼠标在空间里时
                posInt = VectorToInt(pos);
                dy = posInt.y;
                dx = (posInt.x / mapUnitSize) * mapUnitSize;    // 技巧 自动去尾数
                dz = (posInt.z / mapUnitSize) * mapUnitSize;
                posInt = new Vector3Int(dx, dy, dz);
                float cursorOffset = (float)mapUnitSize / 2f;
                // 画方格
                Vector3 p1 = new Vector3(posInt.x - cursorOffset, posInt.y, posInt.z - cursorOffset);
                Vector3 p2 = new Vector3(posInt.x - cursorOffset, posInt.y, posInt.z + cursorOffset);
                Vector3 p3 = new Vector3(posInt.x + cursorOffset, posInt.y, posInt.z + cursorOffset);
                Vector3 p4 = new Vector3(posInt.x + cursorOffset, posInt.y, posInt.z - cursorOffset);
                Color handlesColor = Handles.color;

                Handles.color = cursorColor;
#if UNITY_2019
            Handles.DrawLine(p1,p2);
            Handles.DrawLine(p2,p3);
            Handles.DrawLine(p3,p4);
            Handles.DrawLine(p4,p1);
            sceneView.RepaintAll();
#else
                int thickness = 2;  // 粗细
                Handles.DrawLine(p1, p2, thickness);
                Handles.DrawLine(p2, p3, thickness);
                Handles.DrawLine(p3, p4, thickness);
                Handles.DrawLine(p4, p1, thickness);
                sceneView.Repaint();
#endif
                Handles.color = handlesColor; // color还原
            }

            // 绘制方块部分

            if (mouseDown)
            {
                if (mouseInWorld)
                {  // 是否在绘制区域里
                    checkInstance(); // 检查容器
                    Vector3Int dicPos = new Vector3Int(dx, 0, dz); // 鼠标点击的位置 y为高度，实际高度是随着图层来变动的 这里为0意思是不做关注

                    // 按住Shift 就是删除动作
                    if (e.shift)
                    {
                        Debug.Log($"点击Shift:{dicPos}");
                        if (mapDics[selLayer].ContainsKey(dicPos))
                        {  // 若该图层坐标已有方块，先删除原有的
                            Debug.Log($"存在:{dicPos}");
                            GameObject ori;
                            if (mapDics[selLayer].TryGetValue(dicPos, out ori))
                            {
                                mapDics[selLayer].Remove(dicPos); // 从字典移除
                                DestroyImmediate(ori);            // 从场景移除
                            }
                        }
                    }
                    else
                    {  // 否则则是绘制方块状态
                        if (mapDics[selLayer].ContainsKey(dicPos) && !replaceItem) return;  // 判断是否取代方块且该地是否已有方块
                        if (clearOver) // 若要清除不同图层的重叠方块
                        {
                            for (int i = 0; i < mapDics.Count; i++)
                            {
                                if (mapDics[i].ContainsKey(dicPos)) // 若该坐标已有地图方块 则清除
                                {
                                    GameObject ori;
                                    if (mapDics[i].TryGetValue(dicPos, out ori))
                                    {
                                        mapDics[i].Remove(dicPos); // 从字典中移除
                                        DestroyImmediate(ori);
                                    }
                                }
                            }
                        }

                        if (mapDics[selLayer].ContainsKey(dicPos))
                        {  // 若该图层坐标已有方块，先删除原有的
                            GameObject ori;
                            if (mapDics[selLayer].TryGetValue(dicPos, out ori))
                            {
                                mapDics[selLayer].Remove(dicPos); // 从字典移除
                                DestroyImmediate(ori);            // 从场景移除
                            }
                        }

                        // 绘制方块
                        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(mapCubeItemPrefabs[selNum]); // 动态创建
                        obj.transform.position = posInt;
                        obj.transform.SetParent(layerObjs[selLayer].transform);

                        mapDics[selLayer].Add(dicPos, obj); // 把方块信息加入字典

                        MapTilePathItem mapTilePathItem = new MapTilePathItem(mapCubeTileDetails[selNum]);
                        
                        //转换到左下角是(0,0)的坐标
                        mapTilePathItem.x = dx;
                        mapTilePathItem.z = dz;
                        tiles.Add(mapTilePathItem);
                    }
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }
        }

        #endregion SceneView 绘图用的函数

        private Vector3Int posChange(int x, int y,int z) {
           
            int cx = (int)(x + gridSize / 2);
            int cz = (int)(z + gridSize / 2);
            return new Vector3Int(cx, y, cz);
        }

        // 新增地图图层
        private void addMapLayer()
        {
            newLayer = true;
            layerName = "New Layer";
            layerHeight = 0;
        }

        // 编辑图层
        private void modifyMapLayer()
        {
            editLayer = true;
            layerName = layerNames[selLayer];
            layerHeight = Mathf.RoundToInt(layerObjs[selLayer].transform.position.y);
        }

        private void removeMapLayer()
        {
            if (!EditorUtility.DisplayDialog(
                "删除图层", $"确定要删除图层[{layerNames[selLayer]}]吗？\n 该图层的数据会全部删除",
                "确定",
                "取消"
                )) return;
            DestroyImmediate(layerObjs[selLayer]);  // 删除图层的管理GameObject
            layerObjs.Remove(layerObjs[selLayer]);  // 从List里移除图层物体
            layerNames.Remove(layerNames[selLayer]);
            mapDics.Remove(mapDics[selLayer]);      // 删除该图层的地图方块
            selLayer = 0;
        }

        // 实现新增图层功能
        private void addMapLayerImpl()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("图层名称：", GUILayout.Width(60));
                    layerName = GUILayout.TextField(layerName);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"图层高度:", GUILayout.Width(60));
                    layerHeight = EditorGUILayout.IntField(layerHeight);
                    if (GUILayout.Button("-")) layerHeight -= 1;
                    if (GUILayout.Button("+")) layerHeight += 1;

                    if (GUILayout.Button("确定"))
                    {
                        GameObject lObj = new GameObject();
                        // 设置高度
                        lObj.transform.position = new Vector3(0, layerHeight, 0);
                        lObj.transform.SetParent(objInstance.transform);
                        lObj.name = layerName;
                        layerObjs.Add(lObj);
                        layerNames.Add(layerName);
                        mapDics.Add(new Dictionary<Vector3Int, GameObject>());  // 为图层增加存储的字典
                        newLayer = false; // 增加完成，关闭UI
                        layerName = "";
                    }

                    if (GUILayout.Button("取消"))
                    {
                        newLayer = false;
                        layerName = "";
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        // 实现编辑图层
        private void modifyMapLayerImpl()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"图层名称", GUILayout.Width(60));
                    layerName = GUILayout.TextField(layerName);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("图层高度:", GUILayout.Width(60));
                layerHeight = EditorGUILayout.IntField(layerHeight);

                if (GUILayout.Button("-")) layerHeight -= 1;
                if (GUILayout.Button("+")) layerHeight += 1;

                if (GUILayout.Button("确定"))
                {
                    layerObjs[selLayer].name = layerName;
                    layerObjs[selLayer].transform.position = new Vector3(0, layerHeight, 0);
                    layerNames[selLayer] = layerName;

                    editLayer = false;
                    layerName = "";
                }

                if (GUILayout.Button("取消"))
                {
                    editLayer = false;
                    layerName = "";
                }
            }
            GUILayout.EndHorizontal();
        }

        private void displayLayerList()
        {
            selLayer = GUILayout.SelectionGrid(
                selLayer,
                layerNames.ToArray(),
                1,
                GUILayout.Width(windowWidth - 20)
                );
        }

        #region 绘制地图用的函数

        //设置单位尺寸
        private void SettingUnitSize()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("地图单位尺寸", GUILayout.Width(100));
                mapUnitSize = EditorGUILayout.IntField(mapUnitSize, GUILayout.Width(60));
                if (mapUnitSize < 1) mapUnitSize = 1;
                GUILayout.Space(30);
                GUILayout.Label("游标颜色", GUILayout.Width(60));
                cursorColor = EditorGUILayout.ColorField(cursorColor, GUILayout.Width(60));
            }
            GUILayout.EndHorizontal();
        }

        //设置参考线尺寸
        private void SettingGrid()
        {
            GUILayout.BeginHorizontal();
            {
                showGrid = GUILayout.Toggle(showGrid, "显示参考线", GUILayout.Width(80));
                GUILayout.Space(10);
                GUILayout.Label("参考线范围(单数):", GUILayout.Width(100));
                gridSize = EditorGUILayout.IntField(gridSize, GUILayout.Width(60));
                gridSize = ((gridSize - 1) / 2) * 2 + 1;    // 确保数字一定为单数

                if (GUILayout.Button("-", GUILayout.Width(20))) gridSize -= 2;
                if (GUILayout.Button("+", GUILayout.Width(20))) gridSize += 2;

                GUILayout.Space(20);
                if (gridSize < 1) gridSize = 1;
                GUILayout.Label("参考线颜色:", GUILayout.Width(70));
                gridColor = EditorGUILayout.ColorField(gridColor, GUILayout.Width(60));
            }
            GUILayout.EndHorizontal();
        }

        //新地图
        private void createNewMap()
        {
            if (!EditorUtility.DisplayDialog(
                "新地图", "现有地图资料会被清空,确定新建新地图吗？",
                "确定", "取消")) return;

            layerObjs.Clear();
            layerNames.Clear();
            mapDics.Clear();
            DestroyImmediate(objInstance);
            checkInstance();
            tempFileName = "";
        }

        // 存储地图资料
        private void saveMapData()
        {
            string defaultPath = tempPath; // 取得先前的数据
            if (defaultPath == "") defaultPath = Application.dataPath;
            string fileName = EditorUtility.SaveFilePanel("存储数据", defaultPath, tempFileName, "json");

            if (fileName != "")
            {
                tempPath = Path.GetDirectoryName(fileName); // 记录当前文件夹，跟之前的类似
                tempFileName = Path.GetFileName(fileName);
                saveData(fileName);
            }
        }

        private void loadMapData()
        {
            selNum = -1;
            selLayer = 0;
            string defaultPath = tempPath;
            if (defaultPath == "") defaultPath = Application.dataPath;
            string fileName = EditorUtility.OpenFilePanel("载入数据", defaultPath, "json");
            if (fileName != "")
            {
                tempPath = Path.GetDirectoryName(fileName); // 记录当前文件夹，跟之前的类似
                tempFileName = Path.GetFileName(fileName);
                loadData(fileName);
            }
        }

        #endregion 绘制地图用的函数
    }
}
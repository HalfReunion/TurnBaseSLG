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
        //���ڷ������ɵĳ�������
        private static GameObject objInstance;

        // Ϊ�ñ༭�����Թ���
        private Vector2 scrollPos;

        private float windowWidth; // ���ڿ��
        private string tempPath = "";  // ��¼ʹ�������ѡȡ�����ϼ�

        #region ��ͼ���鲿��

        private string mapInfoFilePath = "Assets/MapCreator/TileMap/TileMapItems.json";
        private string mapDetailFilePath = "Assets/MapCreator/TileMap/TileMapDetailInfo.json";
        

        private static List<GameObject> mapCubeItemPrefabs = new List<GameObject>(); //map cube list
        private static List<string> mapCubeItemNames = new List<string>();  //map cube list name
        private static List<Texture> mapCubeItemIcon = new List<Texture>(); //map cube icon
        private static int selNum = 0;      // selected item num
        private static float iconSize = 80; // icon size

        #endregion ��ͼ���鲿��

        #region ͼ��������

        private static List<GameObject> layerObjs = new List<GameObject>(); // ͨ����ͬ��GameObject������ͼ��
        private static List<string> layerNames = new List<string>();        // ͼ������
        private static bool newLayer = false;
        private static bool editLayer = false;
        private static string layerName = "";
        private static int layerHeight = 0;
        private static int selLayer = 0;  // ��ѡȡ��ͼ���±�

        #endregion ͼ��������

        #region ��ͼ����������

        private static bool mouseDown = false; //�������Ƿ񱻵��
        private static int mapUnitSize = 1; // ��ͼ��λ��С
        private static Color cursorColor = Color.yellow; //�ο�����ɫ
        private static Color gridColor = Color.gray;
        private static bool showGrid = true;// �Ƿ���ʾ�ο���
        private static int gridSize = 9;    // �ο�������
        private static bool clearOver = false;  // �Զ������ͬͼ����ص�����
        private static bool replaceItem = true; // ��Ϊtrue �򵱸õ����з���ʱ����ȡ����
        private static List<Dictionary<Vector3Int, GameObject>> mapDics = new List<Dictionary<Vector3Int, GameObject>>();   // ��ͼ��������
        private static List<Dictionary<Vector3Int, int>> costDics = new List<Dictionary<Vector3Int, int>>();   // ��ͼ����

        private static bool OnPainting = false; // �Ƿ�ʼ���Ƶ�ͼ

        #endregion ��ͼ����������

        #region ·�� �༭����

        private static bool mapTileDetail = false; // �����������
        private static List<MapCubeTileDetail> mapCubeTileDetails = new List<MapCubeTileDetail>();

        private List<MapTilePathItem> tiles = new List<MapTilePathItem>();
        private MapTilePathItem[,] mapTiles;

        #endregion ·�� �༭����

        #region �浵������

        private static string tempFileName = "";

        #endregion �浵������

        public void OnGUI()
        {
            Input.imeCompositionMode = IMECompositionMode.On;  //����������֧������

            Texture2D texture = new Texture2D(1, 1);         //�����в�����ʾ������
            windowWidth = position.width - 20;              //���ֵĿ��

            GUIStyle boxStyle = new GUIStyle(GUI.skin.box); // set a box style
            boxStyle.normal.textColor = Color.white;        //set the style's text color

            boxStyle.fixedWidth = windowWidth;

            GUILayout.Label("Tile Map Editor");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true,       //չ��������
                GUILayout.Height(position.height - 20));
            {
                #region ��ʾ��ͼ���

                GUILayout.BeginVertical(GUILayout.Width(windowWidth));
                {
                    GUILayout.Space(10);
                    GUILayout.Box("��ͼ���", boxStyle);

                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("�����ͼ���"))
                        {
                            addMapCubeItem();
                        }

                        if (GUILayout.Button("ɾ����ͼ���"))
                        {
                            removeMapItem();
                        }

                        if (GUILayout.Button("����ȫ����ͼ���"))
                        {
                            removeAllMapItem();
                        }

                        //��ʾ�ߴ��Slider
                        iconSize = GUILayout.HorizontalSlider(iconSize, 40, 120, GUILayout.Width(60));
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                displayMapItems();

                // �༭���������
                if (selNum >= 0)
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("��������", boxStyle);
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("�������", GUILayout.Width(100));
                            mapCubeTileDetails[selNum].cost = EditorGUILayout.IntField(mapCubeTileDetails[selNum].cost, GUILayout.Width(60));
                            GUILayout.Space(10);
                            GUILayout.Label("�÷����Ƿ�ת������", GUILayout.Width(100));
                            mapCubeTileDetails[selNum].isTran = EditorGUILayout.Toggle(mapCubeTileDetails[selNum].isTran, GUILayout.Width(60));
                        }
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.EndVertical();
                }

                //��ʾ��ͼItem�б�

                #endregion ��ʾ��ͼ���

                #region ͼ�����

                GUILayout.BeginVertical(GUILayout.Width(windowWidth));
                {
                    GUILayout.Space(10); //10 pixel�ո�
                    GUILayout.Box("ͼ��", boxStyle);
                    GUILayout.BeginHorizontal();
                    {
                        if (!newLayer && !editLayer) // ����������༭ͼ�������ʾ��ť
                        {
                            if (GUILayout.Button("����ͼ��"))
                            {
                                addMapLayer();
                            }

                            if (GUILayout.Button("�༭ͼ��"))
                            {
                                modifyMapLayer();
                            }

                            if (GUILayout.Button("ɾ��ͼ��"))
                            {
                                removeMapLayer();
                            }
                        }
                        if (newLayer) // ����ͼ��״̬
                        {
                            addMapLayerImpl();
                        }
                        else if (editLayer) // �༭ͼ��״̬
                        {
                            modifyMapLayerImpl();
                        }
                    }

                    GUILayout.EndHorizontal();

                    displayLayerList();
                }
                GUILayout.EndVertical();

                #endregion ͼ�����

                #region ��ͼ������

                GUILayout.BeginVertical();
                {
                    GUILayout.Space(10);
                    GUILayout.Box("��ͼ����", boxStyle);

                    //�趨��λ�ߴ�
                    SettingUnitSize();
                    //�趨�ο��߳ߴ�
                    SettingGrid();
                    GUILayout.BeginHorizontal();
                    {
                        clearOver = GUILayout.Toggle(clearOver, "�Զ������ͬͼ��ĵ�ͼ����");
                        replaceItem = GUILayout.Toggle(replaceItem, "�Զ��滻");
                    }
                    GUILayout.EndHorizontal();

                    if (OnPainting) // ����״̬��
                    {
                        GUIStyle btnStyle = new GUIStyle(GUI.skin.button); // ȡ��Ԥ�谴ť�����
                        btnStyle.normal.textColor = Color.yellow;           // ���ָĳɻ���
                        btnStyle.normal.background = makeTex(10, 10, Color.blue); // ��ť��ɫ
                        if (GUILayout.Button("����ֹͣ���Ƶ�ͼ", btnStyle))
                        {
                            OnPainting = false;
                        }
                    }
                    else
                    {
                        GUIStyle btnStyle = new GUIStyle(GUI.skin.button); // ȡ��Ԥ�谴ť�����
                        btnStyle.normal.textColor = Color.black;           // ���ָĳɻ���
                        btnStyle.normal.background = makeTex(10, 10, Color.white); // ��ť��ɫ
                        if (GUILayout.Button("���¿�ʼ����", btnStyle))
                        {
                            OnPainting = true;
                        }
                    }

                    GUILayout.Space(10);
                    GUILayout.Box("��ͼ����", boxStyle);
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("�µ�ͼ"))
                        {
                            createNewMap();
                        }
                        if (GUILayout.Button("�洢"))
                        {
                            saveMapData();
                        }
                        if (GUILayout.Button("��������"))
                        {
                            loadMapData();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                #endregion ��ͼ������
            }

            GUILayout.BeginVertical();
            {
                GUILayout.Space(10f);
                GUILayout.Box("�決·��", boxStyle);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("����決·��"))
                {
                    bakePath();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// �決·��
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

            string fileName = EditorUtility.SaveFilePanel("�洢����", defaultPath, tempFileName, "json");
            LocalFileTool.SaveFileInEditor(str, fileName);
        }

        #region ��ͼ�������

        //�������л�������
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
            // дһ�����л�����ķ���
            LocalFileTool.SaveFileInEditor(mapCubeItemNames.ToArray(), mapInfoFilePath);
            string it = SerializeTool.SerializeToFile(mapCubeTileDetails);
            LocalFileTool.SaveFileInEditor(it, mapDetailFilePath);
        }

        private void buildMapInfoData()
        {
            clear();
            // �����л�����,������ͼ��Ϣ
            selNum = -1;

            string res = LocalFileTool.LoadJsonFile(mapInfoFilePath);
            string details = LocalFileTool.LoadJsonFile(mapDetailFilePath);
            if (res != null)
            {
                string[] names = SerializeTool.Deserialize<string[]>(res);
                foreach (var i in names)
                {
                    GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(i, typeof(GameObject));   // �༭��ר��
                    mapCubeItemPrefabs.Add(obj);
                    mapCubeItemNames.Add(i);
                    mapCubeItemIcon.Add(AssetPreview.GetAssetPreview(obj));             // �����Դʡ��ͼ
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

            // ����һ���ļ�ѡ���
            string prefabFull = EditorUtility.OpenFilePanel("ѡȡ��ͼԪ��", defaultPath, "prefab,fbx");
            if (string.IsNullOrEmpty(prefabFull)) return;
            //��¼�µ�ǰ���ļ���
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
        /// ���ݵ�ͼ����������ͼ��ϸ����
        /// </summary>
        private void addMapCubeDetail()
        {
            MapCubeTileDetail cube = new MapCubeTileDetail();
            mapCubeTileDetails.Add(cube);
        }

        private void removeMapItem()
        {
            if (mapCubeItemPrefabs.Count == 0) return;

            if (!EditorUtility.DisplayDialog("ɾ����ͼ���", $"ȷ��ɾ�����:{mapCubeItemNames[selNum]}?", "ȷ��", "ȡ��")) return;

            mapCubeItemIcon.Remove(mapCubeItemIcon[selNum]);
            mapCubeItemNames.Remove(mapCubeItemNames[selNum]);
            mapCubeItemPrefabs.Remove(mapCubeItemPrefabs[selNum]);
            selNum = 0;
            saveMapInfoData();
        }

        private void removeAllMapItem()
        {
            if (!EditorUtility.DisplayDialog("ɾ�����е�ͼ���", $"ɾ�����е�ͼ���?", "ȷ��", "ȡ��")) return;
            mapCubeItemIcon.Clear();
            mapCubeItemNames.Clear();
            mapCubeItemPrefabs.Clear();
            mapCubeTileDetails.Clear();
            selNum = -1;
            saveMapInfoData();
        }

        /// <summary>
        /// ��ʾ����ͼ��
        /// </summary>
        private void displayMapItems()
        {
            if (mapCubeItemNames.Count > 0)
            {
                if (iconSize > 40.1f)        // ��Ҫ��ʾ����ͼ������Slider��iconSize
                {
                    int xCount = (int)((windowWidth - 20) / iconSize);
                    int lines = ((mapCubeItemNames.Count - 1) / xCount) + 1;
                    // ����grid ����IconSize�������������͸��Ӵ�С
                    // ���� 1:������ص��±� 2:չʾ�����ݵ�iconͼ�� 3:ÿ���а�ť������ 4:
                    selNum = GUILayout.SelectionGrid(selNum, mapCubeItemIcon.ToArray(),
                        xCount, GUILayout.Width(windowWidth - 20), GUILayout.Height(lines * iconSize));
                }
                else //������С�ڣ�����ʾ��ϸ�����б�
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

            //��Ҫ�޸�һ�£���Ϊ����ն���
            mapCubeTileDetails.Clear();
        }

        #endregion ��ͼ�������

        #region ϵͳ����

        private void OnFocus()  //���»�ý���ʱ����
        {
            checkInstance();
            buildMapInfoData();
            buildMapDic(); //�ؽ���ͼ�ֵ�
        }

        private void OnEnable()
        {
            //checkInstance();
            //����һ��SceneView���Ƶ�ί��
            SceneView.duringSceneGui += this.OnSceneGUI;

            buildMapInfoData();
            checkInstance();
            buildMapDic(); //�ؽ���ͼ�ֵ�
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }

        // ���objInstance�����������ؽ�
        private void checkInstance()
        {
            // ��֪��Ҫ��Ҫ����GameObject
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
                    mapDics.Add(new Dictionary<Vector3Int, GameObject>()); // �б��±�Ϊ�㼶
                    selLayer = 0;
                }
                return;
            }

            // ������ObjInstance ���򹹽�����
            // ������һ��������Ϊͼ����Ϣ
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
            if (layerObjs.Count == 0) // ��û�������壬�򴴽�һ����Ԥ��ͼ��
            {
                GameObject defaultLayer = new GameObject("Default");
                defaultLayer.transform.SetParent(objInstance.transform);
                layerNames.Add("Default");
                layerObjs.Add(defaultLayer);
                mapDics.Add(new Dictionary<Vector3Int, GameObject>()); // �б��±�Ϊ�㼶
                selLayer = 0;
            }
        }

        private bool MouseToWorldPos(Vector3 mousePos, int y, out Vector3 mouseWorldPos)
        {
            // ͼ��߶�
            Vector3Int h = new Vector3Int(0, y, 0);

            Ray mouseRay = HandleUtility.GUIPointToWorldRay(mousePos); //������ߣ��е���Camera.ScreenPointToRay
                                                                       // y�������£��������ĵ�����ڻ�����
            if (mouseRay.direction.y <= 0)
            {
                mouseRay.origin -= h;
                float t = -mouseRay.origin.y / mouseRay.direction.y;

                // ȡ�õ�������������
                mouseWorldPos = mouseRay.origin + t * mouseRay.direction + h;
                return true;
            }
            mouseWorldPos = Vector3.zero;
            return false;
        }

        // �ؽ���ͼ
        private void buildMapDic()
        {
            if (objInstance == null)
            {
                Debug.LogWarning("�ؽ��ֵ�ʧ�ܣ�Instance����������");
                return;
            }

            for (int i = 0; i < layerNames.Count; i++)
            {
                mapDics[i].Clear(); // ���ԭ���ڴ������
                Transform[] children = layerObjs[i].GetComponentsInChildren<Transform>(); // ȡ������������

                foreach (Transform b in children)
                {
                    if (b.parent == layerObjs[i].transform)  // ֻʹ�õ�һ��������
                    {
                        // ���ÿ���ؿ��������λ��
                        Vector3Int posInt = VectorToInt(b.transform.position);
                        Vector3Int dicPos = new Vector3Int(posInt.x, 0, posInt.z);
                        b.transform.position = posInt;  // �ѷ������
                        if (!mapDics[i].ContainsKey(dicPos))
                        {  // ���ֵ�û�иõؿ�
                            mapDics[i].Add(dicPos, b.gameObject);    // д���ֵ�
                        }
                    }
                }
            }
        }

        private void saveData(string fileName)
        {
            MapTileData data = new MapTileData();
            checkInstance(); // �ؽ�����
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
            data.resBlocks = mapCubeItemNames.ToArray(); // ��Դ�б�
            data.layerDatas = new MapTileLayer[layerObjs.Count()];

            for (int i = 0; i < layerObjs.Count; i++)
            {
                data.layerDatas[i].name = layerObjs[i].name;
                data.layerDatas[i].height = Mathf.RoundToInt(layerObjs[i].transform.position.y);
                data.layerDatas[i].blocks = new List<Block>();

                Transform[] children = layerObjs[i].GetComponentsInChildren<Transform>();

                foreach (var item in children)
                {    // ����ÿ����ͼ��
                    if (item.parent == layerObjs[i].transform)
                    {
                        // 1. ȡ�õذ�ԭʼ��Prefab
                        GameObject prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(item.gameObject);

                        // 2. ȡ��Prefab������Ӳ�̿ռ��ŵ�λ��
                        string assetPath = AssetDatabase.GetAssetPath(prefab);

                        // 3. �ҳ���ͬ���ĵ�����Ϊ���
                        int index = 0;  // ��Դ ���
                        for (int j = 0; j < data.resBlocks.Length; j++)
                        {
                            if (assetPath.ToLower() == data.resBlocks[j].ToLower())
                            {
                                index = j;  //  �ҵ����
                                break;      // ���ҵ���ţ���ȡ��
                            }
                        }

                        // ����������GameObject��ӵ�Ҫ�洢��block�б���
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

            // �������
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = data.playerPos;
                player.transform.eulerAngles = data.playerRot;
            }

            // �����Դ�б�
            mapCubeItemPrefabs.Clear();
            mapCubeItemNames.Clear();
            mapCubeItemIcon.Clear();
            mapCubeTileDetails.Clear();
            selNum = -1;

            // ��յ�ͼ���
            layerObjs.Clear();
            layerNames.Clear();
            mapDics.Clear();    // ����ֵ�
            DestroyImmediate(objInstance);

            // ��ȡ��Դ
            foreach (var i in data.resBlocks)
            {
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(i, typeof(GameObject));
                mapCubeItemPrefabs.Add(obj);
                mapCubeItemNames.Add(i);
                mapCubeItemIcon.Add(AssetPreview.GetAssetPreview(obj));
            }
            saveMapInfoData(); // �洢�б��ڴ���

            objInstance = new GameObject(); // �����µ�����

            objInstance.name = "EditorObjInstance";

            int dictNum = 0;
            foreach (var i in data.layerDatas)
            {
                // ����ͼ��
                GameObject lObj = new GameObject();
                lObj.name = i.name;
                lObj.transform.SetParent(objInstance.transform);
                lObj.transform.position = new Vector3(0, i.height, 0);
                layerObjs.Add(lObj); // �����б�
                layerNames.Add(i.name);
                mapDics.Add(new Dictionary<Vector3Int, GameObject>());

                // �������з���
                foreach (Block b in i.blocks)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(mapCubeItemPrefabs[b.index]);
                    obj.transform.position = b.pos;
                    obj.transform.SetParent(lObj.transform);
                    Vector3Int dicPos = new Vector3Int(b.pos.x, 0, b.pos.z);
                    mapDics[dictNum].Add(dicPos, obj); // ����������ֵ�
                }
                dictNum++;
            }
        }

        private Vector3Int VectorToInt(Vector3 origin)
        {  // ��Vector3 ת��ΪVector3Int
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
            // ����һ��������ͼ��
            Texture2D result = new Texture2D(width, height);

            result.SetPixels(pix, 0);
            result.Apply();
            return result;
        }

        #endregion ϵͳ����

        #region SceneView ��ͼ�õĺ���

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

        // SceneView���Ƶ�ʵ��,���Ƴ�������UI
        private void OnSceneGUI(SceneView sceneView)
        {
            //Handles.BeginGUI();
            //{
            //    // ��SceneView�����UI
            //}
            //Handles.EndGUI();

            if (!OnPainting) return;

            int controlID = GUIUtility.GetControlID(FocusType.Passive);  // ������갴�µ��ź�
            Event e = Event.current; // ȡ���¼�
            if (e.alt) return;      // ����ס[Alt]�������ʾʹ����Ҫ��ת���棬��Ҫ���л���

            switch (e.GetTypeForControl(controlID))
            {  // �ж�����¼�
                case EventType.MouseDown:       // ��갴��
                    if (e.button != 0) break; // �����µĲ������
                    GUIUtility.hotControl = controlID;  // �����ź�
                    mouseDown = true;                   // ����갴�µı�ʶ

                    break;

                case EventType.MouseUp:             //  �����ص������ſ�
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

            // �����α�
            if (mouseInWorld)
            {     // ����ڿռ���ʱ
                posInt = VectorToInt(pos);
                dy = posInt.y;
                dx = (posInt.x / mapUnitSize) * mapUnitSize;    // ���� �Զ�ȥβ��
                dz = (posInt.z / mapUnitSize) * mapUnitSize;
                posInt = new Vector3Int(dx, dy, dz);
                float cursorOffset = (float)mapUnitSize / 2f;
                // ������
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
                int thickness = 2;  // ��ϸ
                Handles.DrawLine(p1, p2, thickness);
                Handles.DrawLine(p2, p3, thickness);
                Handles.DrawLine(p3, p4, thickness);
                Handles.DrawLine(p4, p1, thickness);
                sceneView.Repaint();
#endif
                Handles.color = handlesColor; // color��ԭ
            }

            // ���Ʒ��鲿��

            if (mouseDown)
            {
                if (mouseInWorld)
                {  // �Ƿ��ڻ���������
                    checkInstance(); // �������
                    Vector3Int dicPos = new Vector3Int(dx, 0, dz); // �������λ�� yΪ�߶ȣ�ʵ�ʸ߶�������ͼ�����䶯�� ����Ϊ0��˼�ǲ�����ע

                    // ��סShift ����ɾ������
                    if (e.shift)
                    {
                        Debug.Log($"���Shift:{dicPos}");
                        if (mapDics[selLayer].ContainsKey(dicPos))
                        {  // ����ͼ���������з��飬��ɾ��ԭ�е�
                            Debug.Log($"����:{dicPos}");
                            GameObject ori;
                            if (mapDics[selLayer].TryGetValue(dicPos, out ori))
                            {
                                mapDics[selLayer].Remove(dicPos); // ���ֵ��Ƴ�
                                DestroyImmediate(ori);            // �ӳ����Ƴ�
                            }
                        }
                    }
                    else
                    {  // �������ǻ��Ʒ���״̬
                        if (mapDics[selLayer].ContainsKey(dicPos) && !replaceItem) return;  // �ж��Ƿ�ȡ�������Ҹõ��Ƿ����з���
                        if (clearOver) // ��Ҫ�����ͬͼ����ص�����
                        {
                            for (int i = 0; i < mapDics.Count; i++)
                            {
                                if (mapDics[i].ContainsKey(dicPos)) // �����������е�ͼ���� �����
                                {
                                    GameObject ori;
                                    if (mapDics[i].TryGetValue(dicPos, out ori))
                                    {
                                        mapDics[i].Remove(dicPos); // ���ֵ����Ƴ�
                                        DestroyImmediate(ori);
                                    }
                                }
                            }
                        }

                        if (mapDics[selLayer].ContainsKey(dicPos))
                        {  // ����ͼ���������з��飬��ɾ��ԭ�е�
                            GameObject ori;
                            if (mapDics[selLayer].TryGetValue(dicPos, out ori))
                            {
                                mapDics[selLayer].Remove(dicPos); // ���ֵ��Ƴ�
                                DestroyImmediate(ori);            // �ӳ����Ƴ�
                            }
                        }

                        // ���Ʒ���
                        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(mapCubeItemPrefabs[selNum]); // ��̬����
                        obj.transform.position = posInt;
                        obj.transform.SetParent(layerObjs[selLayer].transform);

                        mapDics[selLayer].Add(dicPos, obj); // �ѷ�����Ϣ�����ֵ�

                        MapTilePathItem mapTilePathItem = new MapTilePathItem(mapCubeTileDetails[selNum]);
                        
                        //ת�������½���(0,0)������
                        mapTilePathItem.x = dx;
                        mapTilePathItem.z = dz;
                        tiles.Add(mapTilePathItem);
                    }
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }
        }

        #endregion SceneView ��ͼ�õĺ���

        private Vector3Int posChange(int x, int y,int z) {
           
            int cx = (int)(x + gridSize / 2);
            int cz = (int)(z + gridSize / 2);
            return new Vector3Int(cx, y, cz);
        }

        // ������ͼͼ��
        private void addMapLayer()
        {
            newLayer = true;
            layerName = "New Layer";
            layerHeight = 0;
        }

        // �༭ͼ��
        private void modifyMapLayer()
        {
            editLayer = true;
            layerName = layerNames[selLayer];
            layerHeight = Mathf.RoundToInt(layerObjs[selLayer].transform.position.y);
        }

        private void removeMapLayer()
        {
            if (!EditorUtility.DisplayDialog(
                "ɾ��ͼ��", $"ȷ��Ҫɾ��ͼ��[{layerNames[selLayer]}]��\n ��ͼ������ݻ�ȫ��ɾ��",
                "ȷ��",
                "ȡ��"
                )) return;
            DestroyImmediate(layerObjs[selLayer]);  // ɾ��ͼ��Ĺ���GameObject
            layerObjs.Remove(layerObjs[selLayer]);  // ��List���Ƴ�ͼ������
            layerNames.Remove(layerNames[selLayer]);
            mapDics.Remove(mapDics[selLayer]);      // ɾ����ͼ��ĵ�ͼ����
            selLayer = 0;
        }

        // ʵ������ͼ�㹦��
        private void addMapLayerImpl()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("ͼ�����ƣ�", GUILayout.Width(60));
                    layerName = GUILayout.TextField(layerName);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"ͼ��߶�:", GUILayout.Width(60));
                    layerHeight = EditorGUILayout.IntField(layerHeight);
                    if (GUILayout.Button("-")) layerHeight -= 1;
                    if (GUILayout.Button("+")) layerHeight += 1;

                    if (GUILayout.Button("ȷ��"))
                    {
                        GameObject lObj = new GameObject();
                        // ���ø߶�
                        lObj.transform.position = new Vector3(0, layerHeight, 0);
                        lObj.transform.SetParent(objInstance.transform);
                        lObj.name = layerName;
                        layerObjs.Add(lObj);
                        layerNames.Add(layerName);
                        mapDics.Add(new Dictionary<Vector3Int, GameObject>());  // Ϊͼ�����Ӵ洢���ֵ�
                        newLayer = false; // ������ɣ��ر�UI
                        layerName = "";
                    }

                    if (GUILayout.Button("ȡ��"))
                    {
                        newLayer = false;
                        layerName = "";
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        // ʵ�ֱ༭ͼ��
        private void modifyMapLayerImpl()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"ͼ������", GUILayout.Width(60));
                    layerName = GUILayout.TextField(layerName);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("ͼ��߶�:", GUILayout.Width(60));
                layerHeight = EditorGUILayout.IntField(layerHeight);

                if (GUILayout.Button("-")) layerHeight -= 1;
                if (GUILayout.Button("+")) layerHeight += 1;

                if (GUILayout.Button("ȷ��"))
                {
                    layerObjs[selLayer].name = layerName;
                    layerObjs[selLayer].transform.position = new Vector3(0, layerHeight, 0);
                    layerNames[selLayer] = layerName;

                    editLayer = false;
                    layerName = "";
                }

                if (GUILayout.Button("ȡ��"))
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

        #region ���Ƶ�ͼ�õĺ���

        //���õ�λ�ߴ�
        private void SettingUnitSize()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("��ͼ��λ�ߴ�", GUILayout.Width(100));
                mapUnitSize = EditorGUILayout.IntField(mapUnitSize, GUILayout.Width(60));
                if (mapUnitSize < 1) mapUnitSize = 1;
                GUILayout.Space(30);
                GUILayout.Label("�α���ɫ", GUILayout.Width(60));
                cursorColor = EditorGUILayout.ColorField(cursorColor, GUILayout.Width(60));
            }
            GUILayout.EndHorizontal();
        }

        //���òο��߳ߴ�
        private void SettingGrid()
        {
            GUILayout.BeginHorizontal();
            {
                showGrid = GUILayout.Toggle(showGrid, "��ʾ�ο���", GUILayout.Width(80));
                GUILayout.Space(10);
                GUILayout.Label("�ο��߷�Χ(����):", GUILayout.Width(100));
                gridSize = EditorGUILayout.IntField(gridSize, GUILayout.Width(60));
                gridSize = ((gridSize - 1) / 2) * 2 + 1;    // ȷ������һ��Ϊ����

                if (GUILayout.Button("-", GUILayout.Width(20))) gridSize -= 2;
                if (GUILayout.Button("+", GUILayout.Width(20))) gridSize += 2;

                GUILayout.Space(20);
                if (gridSize < 1) gridSize = 1;
                GUILayout.Label("�ο�����ɫ:", GUILayout.Width(70));
                gridColor = EditorGUILayout.ColorField(gridColor, GUILayout.Width(60));
            }
            GUILayout.EndHorizontal();
        }

        //�µ�ͼ
        private void createNewMap()
        {
            if (!EditorUtility.DisplayDialog(
                "�µ�ͼ", "���е�ͼ���ϻᱻ���,ȷ���½��µ�ͼ��",
                "ȷ��", "ȡ��")) return;

            layerObjs.Clear();
            layerNames.Clear();
            mapDics.Clear();
            DestroyImmediate(objInstance);
            checkInstance();
            tempFileName = "";
        }

        // �洢��ͼ����
        private void saveMapData()
        {
            string defaultPath = tempPath; // ȡ����ǰ������
            if (defaultPath == "") defaultPath = Application.dataPath;
            string fileName = EditorUtility.SaveFilePanel("�洢����", defaultPath, tempFileName, "json");

            if (fileName != "")
            {
                tempPath = Path.GetDirectoryName(fileName); // ��¼��ǰ�ļ��У���֮ǰ������
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
            string fileName = EditorUtility.OpenFilePanel("��������", defaultPath, "json");
            if (fileName != "")
            {
                tempPath = Path.GetDirectoryName(fileName); // ��¼��ǰ�ļ��У���֮ǰ������
                tempFileName = Path.GetFileName(fileName);
                loadData(fileName);
            }
        }

        #endregion ���Ƶ�ͼ�õĺ���
    }
}
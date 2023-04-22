using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

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
    private static bool showGrid = true;// �Ƿ���ʾ�ο���
    private static int gridSize = 9;    // �ο�������
    private static bool clearOver = false;  // �Զ������ͬͼ����ص�����
    private static bool replaceItem = true; // ��Ϊtrue �򵱸õ����з���ʱ����ȡ����
    private static List<Dictionary<Vector3Int, GameObject>> mapDics = new List<Dictionary<Vector3Int, GameObject>>();   // ��ͼ����
    private static bool OnPainting = false; // �Ƿ�ʼ���Ƶ�ͼ

    #endregion ��ͼ����������

    #region �浵������
    private static string tempFileName = "";
    #endregion  �浵������


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

            //��ʾ��ͼItem�б�

            #endregion ��ʾ��ͼ���
        }
        EditorGUILayout.EndScrollView();
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
    }

    private void buildMapInfoData()
    {
        clear();
        // �����л�����,������ͼ��Ϣ
        selNum = 0;

        string res = LocalFileTool.LoadJsonFile(mapInfoFilePath);
        if (res != null)
        {
            MapItem mapItem = SerializeTool.Deserialize<MapItem>(res);
            foreach (var i in mapItem.names)
            {
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(i, typeof(GameObject));   // �༭��ר��
                mapCubeItemPrefabs.Add(obj);
                mapCubeItemNames.Add(i);
                mapCubeItemIcon.Add(AssetPreview.GetAssetPreview(obj));             // �����Դʡ��ͼ
            }
        }
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
        saveMapInfoData();
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
        selNum = 0;
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
    }

    #endregion ��ͼ�������

    #region ϵͳ����

    private void OnFocus()  //���»�ý���ʱ����
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

    // ���objInstance
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

    #endregion ϵͳ����
}
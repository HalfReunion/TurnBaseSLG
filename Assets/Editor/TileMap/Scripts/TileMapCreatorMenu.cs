using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace MapCreator
{
    public class TileMapCreatorMenu
    {
        [MenuItem("Tools/Map/3D TileMap Creator", false, 0)]
        public static void OpenControlPanel() { 
            EditorWindow.GetWindow(typeof(TileEditor));
        } 
    } 

}
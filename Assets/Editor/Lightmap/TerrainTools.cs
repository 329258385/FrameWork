using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;

namespace BigWorldTool
{
    public class TerrainTools : EditorWindow
    {
        [MenuItem("Menu/Lightmap/BakeToolsWindow")]
        public static void Open()
        {
            TerrainTools current = EditorWindow.GetWindow<TerrainTools>();
            current.titleContent = new GUIContent("烘焙工具");

            //Rect r = current.position;
            //current.position = new Rect(r.x+64, r.y+64, 688, 720);
        }


        //private TerrainSliceCreator terrainCreator;
        //private SceneObjectSlice objectSlice;
        //private Terrain2Mesh terrain2Mesh;
        private BatchLightBaker lightBaker;
        //private QuadTreeTool quadTreeTool;
        private EditorWindowSection[] sectionSelectSections;
        private EditorWindowSection activeSection;


        private string[] sectionSelectTip = new string[] {"批量烘焙"};

        #region 窗口基本功能
        void OnEnable()
        {
            //terrainCreator = new TerrainSliceCreator(this);
            //terrainCreator.OnEnable();
            //objectSlice = new SceneObjectSlice(this);
            //objectSlice.OnEnable();
            //terrain2Mesh = new Terrain2Mesh(this);
            //terrain2Mesh.OnEnable();
            lightBaker = new BatchLightBaker(this);
            lightBaker.OnEnable();
            //quadTreeTool = new QuadTreeTool(this);
            //quadTreeTool.OnEnable();
            sectionSelectSections = new EditorWindowSection[] { lightBaker };
            activeSection = lightBaker;
            //activeSection = terrainCreator;
        }

        void OnDestroy()
        {
            //terrainCreator.OnDestroy();
            //objectSlice.OnDestroy();
            //terrain2Mesh.OnDestroy();
            lightBaker.OnDestroy();
        }

        void OnFocus()
        {
        }

        void OnGUI()
        {
            //try
            //{
            //切分地图

            drawSectionSelect();

            //if (activeSection == terrainCreator)
            //{
            //    terrainCreator.OnGUI();
            //}
            //if (activeSection == objectSlice)
            //{
            //    objectSlice.OnGUI();
            //}
            //if (activeSection == terrain2Mesh)
            //{
            //    terrain2Mesh.OnGUI();
            //}
            if (activeSection == lightBaker)
            {
                lightBaker.OnGUI();
            }
            //if (activeSection == quadTreeTool)
            //{
            //    quadTreeTool.OnGUI();
            //}
            //}
            //catch (Exception e)
            //{
            //    Debug.LogError("Exception Message " + e.Message);
            //    Debug.LogError("Exception Stack" + e.StackTrace);
            //}
        }

        

        void OnInspectorUpdate()
        {
            
        }

        void Update()
        {
            
        }
        #endregion

        private void drawSectionSelect() //总体的区域选择
        {
            int sectionIndex = -1;
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < sectionSelectTip.Length; i++)
            {
                GUIStyle style = GUI.skin.button;
                if (activeSection == sectionSelectSections[i])
                    style = GUI.skin.box;
                if (GUILayout.Button(new GUIContent(sectionSelectTip[i], sectionSelectTip[i]), style, GUILayout.Height(36),GUILayout.Width(80)))
                {
                    sectionIndex = i;
                }
            }
            EditorGUILayout.EndHorizontal();
            if (sectionIndex != -1)
            {
                activeSection = sectionSelectSections[sectionIndex];
                for (int i = 0; i < sectionSelectSections.Length; i++)
                {
                    if (sectionSelectSections[i] != null)
                    {
                        sectionSelectSections[i].isActive = false;
                    }
                }
                    
                if(activeSection != null)
                    activeSection.isActive = true;
            }
        }

        

    }



}

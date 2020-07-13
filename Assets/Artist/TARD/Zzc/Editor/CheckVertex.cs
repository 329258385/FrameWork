#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CheckVertex : EditorWindow
{
    private List<MeshFilter> mfs = new List<MeshFilter>();
    private int page = 0;
    private int maxObjNumber = 40;
    private int maxPageNumberPerLine = 20;
    private Vector2 scrollPos = Vector2.zero;

    CheckVertex()
    {
        this.titleContent = new GUIContent("场景顶点排序"); 
    }

    private void Awake()
    {
        mfs.Clear();
        page = 0;
    }

    private void OnDestroy()
    {
        mfs.Clear();
        page = 0;
    }

    [MenuItem("TARD/场景顶点排序")]
    static void showCheckVertexWindow()
    {        
        CheckVertex cv = (CheckVertex)GetWindow(typeof(CheckVertex));
        cv.autoRepaintOnSceneChange = true;
        
    }

    private void OnGUI()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos,GUILayout.Width(position.width),GUILayout.Height(position.height));

        GUILayout.BeginVertical();
        GUILayout.Space(10);
        if (GUILayout.Button("开始检测", GUILayout.Width(100f)))
        {
            FindAllSceneObject();
            page = 0;
        }

        int pages = Mathf.CeilToInt(mfs.Count / (float)maxObjNumber);
        int rows = Mathf.CeilToInt(pages / (float)maxPageNumberPerLine);

        Show(page);

        GUILayout.EndVertical();


        GUILayout.BeginHorizontal();

        GUILayout.Label("", GUILayout.Width(1400));

        GUILayout.BeginArea(new Rect(600,0,1000,1000));

        GUILayout.Space(20);

        for (int j = 0; j < rows; j++)
        {
            int end=Mathf.Min(j* maxPageNumberPerLine + maxPageNumberPerLine, pages);

            GUILayout.BeginHorizontal();
            for (int i = j * maxPageNumberPerLine; i < end; i++)
            {
                if (GUILayout.Button(i.ToString(), GUILayout.Width(35f)))
                {
                    page = i;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();

        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
    }

    void Show(int page)
    {
        int end =Mathf.Min (page * maxObjNumber + maxObjNumber, mfs.Count-1);

        for (int i = page * maxObjNumber; i < end; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("物件:", GUILayout.Width(30f));
            mfs[i] = (MeshFilter)EditorGUILayout.ObjectField(mfs[i], typeof(MeshFilter), GUILayout.Width(400f));
            GUILayout.Label("顶点:"+mfs[i].sharedMesh.vertexCount.ToString());
            GUILayout.EndHorizontal();
        }
    }


    private void FindAllSceneObject()
    {
        mfs.Clear();
        GameObject[] objs = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].scene.isLoaded)
            {
                if ((objs[i] as GameObject).GetComponent<MeshFilter>())
                {
                    if ((objs[i] as GameObject).GetComponent<MeshFilter>().sharedMesh)
                    {
                        mfs.Add((objs[i] as GameObject).GetComponent<MeshFilter>());
                    }
                }
            }            
        }
        QuickSort(mfs, 0, mfs.Count - 1);
        mfs.Reverse();
    }

    private int Division(List<MeshFilter> list,int left,int right)
    {
        while (left < right)
        {
            MeshFilter num = list[left];
            if (num.sharedMesh.vertexCount > list[left+1].sharedMesh.vertexCount)
            {
                list[left] = list[left + 1];
                list[left + 1] = num;
                left++;
            }
            else
            {
                MeshFilter temp = list[right];
                list[right] = list[left + 1];
                list[left + 1] = temp;
                right--;
            }
        }
        return left;
    }

    private void QuickSort(List<MeshFilter> list,int left,int right)
    {
        if (left<right)
        {
            int i = Division(list, left, right);
            QuickSort(list, i + 1, right);
            QuickSort(list, left, i - 1);
        }
    }  
}
#endif
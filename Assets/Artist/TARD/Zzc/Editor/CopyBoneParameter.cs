using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CopyBoneParameter : EditorWindow {

    private List<DynamicBone> oldOneChildren;
    private List<Transform> newOneChildren;
    private List<DynamicBone> newOneDBs;
    //private List<Transform> oldOneAllChildren;
    private Transform oldOneTransform;
    private Transform newOneTransform;

    private void Awake()
    {
        oldOneChildren = new List<DynamicBone>();
        newOneChildren = new List<Transform>();
        newOneDBs = new List<DynamicBone>();
    }

    private void OnDestroy()
    {
        oldOneChildren.Clear();
        newOneChildren.Clear();
        newOneDBs.Clear();
    }

    CopyBoneParameter()
    {
        titleContent = new GUIContent("复制骨链参数");
    }

    [MenuItem("TARD/复制骨链参数")]
    private static void ShowCopyParameter()
    {
        CopyBoneParameter cbp = (CopyBoneParameter)GetWindow(typeof(CopyBoneParameter));
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        oldOneTransform = (Transform)EditorGUILayout.ObjectField("Old", oldOneTransform, typeof(Transform), true);
        newOneTransform = (Transform)EditorGUILayout.ObjectField("New", newOneTransform, typeof(Transform), true);
        GUILayout.Space(10);
        if (GUILayout.Button("Copy"))
        {
            oldOneChildren.Clear();
            newOneChildren.Clear();
            newOneDBs.Clear();
            if (oldOneTransform && newOneTransform)
            {
                FindBothChildren();
                CopyValue(oldOneChildren, newOneChildren);
            }
            //Debug.Log(oldOneChildren.Count);
            //Debug.Log(newOneDBs.Count);
        }
        GUILayout.EndVertical();
    }


    private void FindBothChildren()
    {
        if (!oldOneTransform||!newOneTransform)
        {
            return;
        }

        GetChildOld(oldOneTransform, oldOneChildren);
        GetChildNew(newOneTransform, newOneChildren);

    }

    private void GetChildOld(Transform t,List<DynamicBone> list)
    {
        if (t.GetComponent<DynamicBone>())
        {
            list.Add(t.GetComponent<DynamicBone>());
        }

        int num = t.childCount;
        if (num>0)
        {
            for (int i = 0; i < num; i++)
            {
                GetChildOld(t.GetChild(i),list);
            }
        }
    }

    private void GetChildNew(Transform t,List<Transform> list)
    {
        list.Add(t);
        //if (t.GetComponent<DynamicBone>())
        //{
        //    newOneDBs.Add(t.GetComponent<DynamicBone>());
        //}

        int num = t.childCount;
        if (num > 0)
        {
            for (int i = 0; i < num; i++)
            {
                GetChildNew(t.GetChild(i), list);
            }
        }
    }

    private void CopyValue(List<DynamicBone> oldOneChildren, List<Transform> newOneChildren)
    {
        for (int i = 0; i < oldOneChildren.Count; i++)
        {
            for (int j = 0; j < newOneChildren.Count; j++)
            {
                if (oldOneChildren[i].name==newOneChildren[j].name)
                {
                    DynamicBone NDB = newOneChildren[j].gameObject.AddComponent<DynamicBone>();

                    if (NDB)
                    {

                        NDB.boneIndex = oldOneChildren[i].boneIndex;
                        NDB.m_Damping = oldOneChildren[i].m_Damping;
                        NDB.m_Elasticity = oldOneChildren[i].m_Elasticity;
                        NDB.m_Stiffness = oldOneChildren[i].m_Stiffness;
                        NDB.m_Inert = oldOneChildren[i].m_Inert;
                        NDB.maxGravity = oldOneChildren[i].maxGravity;
                        NDB.colliderFactor = oldOneChildren[i].colliderFactor;
                        NDB.factor4YDown = oldOneChildren[i].factor4YDown;
                        NDB.factor5YUp = oldOneChildren[i].factor5YUp;
                        NDB.factor6ZFront = oldOneChildren[i].factor6ZFront;
                        NDB.shouldBoneActive = oldOneChildren[i].shouldBoneActive;
                        NDB.enableCollider = oldOneChildren[i].enableCollider;
                        NDB.stiffnessLockInShow = oldOneChildren[i].stiffnessLockInShow;
                        NDB.clampDamp = oldOneChildren[i].clampDamp;
                        NDB.clampElasticity = oldOneChildren[i].clampElasticity;
                        NDB.clampStiffness = oldOneChildren[i].clampStiffness;
                        NDB.clampInert = oldOneChildren[i].clampInert;
                        NDB.clampPerformance = oldOneChildren[i].clampPerformance;
                        NDB.clampIndex = oldOneChildren[i].clampIndex;
                        NDB.clampIndex2 = oldOneChildren[i].clampIndex2;
                        NDB.clampColliderWhenRun = oldOneChildren[i].clampColliderWhenRun;
                        NDB.useDashClampZ = oldOneChildren[i].useDashClampZ;
                        NDB.clampDashZFactor = oldOneChildren[i].clampDashZFactor;

                        NDB.damping1 = oldOneChildren[i].damping1;
                        NDB.elasticity1 = oldOneChildren[i].elasticity1;
                        NDB.stiffness1 = oldOneChildren[i].stiffness1;
                        NDB.inert1 = oldOneChildren[i].inert1;
                        NDB.maxGravity1 = oldOneChildren[i].maxGravity1;
                        NDB.colliderFactor1 = oldOneChildren[i].colliderFactor1;
                        NDB.factor4YDown1 = oldOneChildren[i].factor4YDown1;
                        NDB.factor5YUp1 = oldOneChildren[i].factor5YUp1;
                        NDB.factor6ZFront1 = oldOneChildren[i].factor6ZFront1;
                        NDB.enableCollider1 = oldOneChildren[i].enableCollider1;
                        NDB.stiffnessLockInShow1 = oldOneChildren[i].stiffnessLockInShow1;
                        NDB.clampDamp1 = oldOneChildren[i].clampDamp1;
                        NDB.clampElasticity1 = oldOneChildren[i].clampElasticity1;
                        NDB.clampStiffness1 = oldOneChildren[i].clampStiffness1;
                        NDB.clampInert1 = oldOneChildren[i].clampInert1;
                        NDB.clampPerformance1 = oldOneChildren[i].clampPerformance1;
                        NDB.clampIndex1 = oldOneChildren[i].clampIndex1;
                        NDB.clampIndex21 = oldOneChildren[i].clampIndex21;
                        NDB.clampColliderWhenRun1 = oldOneChildren[i].clampColliderWhenRun1;
                        NDB.useDashClampZ1 = oldOneChildren[i].useDashClampZ1;
                        NDB.clampDashZFactor1 = oldOneChildren[i].clampDashZFactor1;

                        NDB.damping2 = oldOneChildren[i].damping2;
                        NDB.elasticity2 = oldOneChildren[i].elasticity2;
                        NDB.stiffness2 = oldOneChildren[i].stiffness2;
                        NDB.inert2 = oldOneChildren[i].inert2;
                        NDB.maxGravity2 = oldOneChildren[i].maxGravity2;
                        NDB.colliderFactor2 = oldOneChildren[i].colliderFactor2;
                        NDB.factor4YDown2 = oldOneChildren[i].factor4YDown2;
                        NDB.factor5YUp2 = oldOneChildren[i].factor5YUp2;
                        NDB.factor6ZFront2 = oldOneChildren[i].factor6ZFront2;
                        NDB.enableCollider2 = oldOneChildren[i].enableCollider2;
                        NDB.stiffnessLockInShow2 = oldOneChildren[i].stiffnessLockInShow2;
                        NDB.clampDamp2 = oldOneChildren[i].clampDamp2;
                        NDB.clampElasticity2 = oldOneChildren[i].clampElasticity2;
                        NDB.clampStiffness2 = oldOneChildren[i].clampStiffness2;
                        NDB.clampInert2 = oldOneChildren[i].clampInert2;
                        NDB.clampPerformance2 = oldOneChildren[i].clampPerformance2;
                        NDB.clampIndex12 = oldOneChildren[i].clampIndex2;
                        NDB.clampIndex22 = oldOneChildren[i].clampIndex22;
                        NDB.clampColliderWhenRun2 = oldOneChildren[i].clampColliderWhenRun2;
                        NDB.useDashClampZ2 = oldOneChildren[i].useDashClampZ2;
                        NDB.clampDashZFactor2 = oldOneChildren[i].clampDashZFactor2;

                        CopyReference(oldOneChildren[i], NDB);
                    }
                }
            }
        }
    }

    private void CopyReference(DynamicBone oldDB,DynamicBone newDB)
    {
        string oldName = "";
        if (oldDB.rotatePart)
        {
            oldName = oldDB.rotatePart.name;
            for (int i = 0; i < newOneChildren.Count; i++)
            {
                if (oldName== newOneChildren[i].name)
                {
                    newDB.rotatePart = newOneChildren[i];
                }
            }
        }

        if (oldDB.display)
        {
            oldName = oldDB.display.name;
            for (int i = 0; i < newOneChildren.Count; i++)
            {
                if (oldName == newOneChildren[i].name)
                {
                    newDB.display = newOneChildren[i];
                }
            }
        }

        if (oldDB.m_Root)
        {
            oldName = oldDB.m_Root.name;
            for (int i = 0; i < newOneChildren.Count; i++)
            {
                if (oldName == newOneChildren[i].name)
                {
                    newDB.m_Root = newOneChildren[i];
                }
            }
        }
        
        if (oldDB.colliderPos.Length>0&& oldDB.colliderPos[0])
        {
            oldName = oldDB.colliderPos[0].name;
            for (int i = 0; i < newOneChildren.Count; i++)
            {
                if (oldName == newOneChildren[i].name)
                {
                    newDB.colliderPos = new Transform[1];
                    newDB.colliderPos[0] = newOneChildren[i];
                }
            }
        }

        if (oldDB.head)
        {
            oldName = oldDB.head.name;
            for (int i = 0; i < newOneChildren.Count; i++)
            {
                if (oldName == newOneChildren[i].name)
                {
                    newDB.head = newOneChildren[i];
                }
            }
        }

        if (oldDB.root1)
        {
            oldName = oldDB.root1.name;
            for (int i = 0; i < newOneChildren.Count; i++)
            {
                if (oldName == newOneChildren[i].name)
                {
                    newDB.root1 = newOneChildren[i];
                }
            }
        }

        if (oldDB.root2)
        {
            oldName = oldDB.root2.name;
            for (int i = 0; i < newOneChildren.Count; i++)
            {
                if (oldName == newOneChildren[i].name)
                {
                    newDB.root2 = newOneChildren[i];
                }
            }
        }

        if (oldDB.colliderPos1.Length > 0)
        {
            oldName = oldDB.colliderPos1[0].name;
            for (int i = 0; i < newOneChildren.Count; i++)
            {
                if (oldName == newOneChildren[i].name)
                {
                    newDB.colliderPos1 = new Transform[1];
                    newDB.colliderPos1[0] = newOneChildren[i];
                }
            }
        }

        if (oldDB.colliderPos2.Length > 0)
        {
            oldName = oldDB.colliderPos2[0].name;
            for (int i = 0; i < newOneChildren.Count; i++)
            {
                if (oldName == newOneChildren[i].name)
                {
                    newDB.colliderPos2 = new Transform[1];
                    newDB.colliderPos2[0] = newOneChildren[i];
                }
            }
        }

        if (oldDB.head1)
        {
            oldName = oldDB.head1.name;
            for (int i = 0; i < newOneChildren.Count; i++)
            {
                if (oldName == newOneChildren[i].name)
                {
                    newDB.head1 = newOneChildren[i];
                }
            }
        }

        if (oldDB.head2)
        {
            oldName = oldDB.head2.name;
            for (int i = 0; i < newOneChildren.Count; i++)
            {
                if (oldName == newOneChildren[i].name)
                {
                    newDB.head2 = newOneChildren[i];
                }
            }
        }

    }
}

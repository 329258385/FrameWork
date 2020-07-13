using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadtreeManager : MonoBehaviour
{
    [Header("显示四叉树")]
    public bool ShowQuadtree = false;
    [Header("切换剔除与粒子")]
    public bool SwitchGizmos = false;

    private bool mSwitchGizmos = true;

    private QuadTree CurrentPos;
    private QuadTree Tree = new QuadTree();
    private List<QuadTree> EndTrees = new List<QuadTree>();

    private Vector3[] Extends = new Vector3[2];
    private int MaxTransPerTrunk = 10;
    private int MaxTrancks = 100;
    [Range(0, 180)] private float HideAngle = 35;
    private float hideAngle;
    private float tanHideAngle;
    [Range(0, 300)] private float MinDis = 30;

    private Vector3 mPosMemo;
    private Quaternion mRotMemo;

    // 不处理的layer
    protected List<int> mCullLayerList = new List<int>();

    private int mMaxDepth = 9;

    public bool LodAnimation = true;
    private bool bLodAnimation = true;

    private RenderTargetScaler mRTS;

    private void Awake()
    {
        mCullLayerList.Clear();

        mCullLayerList.Add(LayerMask.NameToLayer("UI"));
        mCullLayerList.Add(LayerMask.NameToLayer("Player"));
        mCullLayerList.Add(LayerMask.NameToLayer("Monster"));
        mCullLayerList.Add(LayerMask.NameToLayer("Boss"));
        mCullLayerList.Add(LayerMask.NameToLayer("NPC"));
        mCullLayerList.Add(LayerMask.NameToLayer("Collection"));

        mCullLayerList.Add(LayerMask.NameToLayer("UISpecial"));
        mCullLayerList.Add(LayerMask.NameToLayer("UI_Model"));
        mCullLayerList.Add(LayerMask.NameToLayer("3DUI"));

        mCullLayerList.Add(LayerMask.NameToLayer("Decal"));
        mCullLayerList.Add(LayerMask.NameToLayer("ClientPlayer"));
        mCullLayerList.Add(LayerMask.NameToLayer("Region"));

#if !UNITY_EDITOR
        if(!GlobalGameDefine.mIsForceCloseRTScaler)
        {
            mRTS = gameObject.AddComponent<RenderTargetScaler>();
            //mRTS.enabled = false;
        }
#else
        if (GlobalGameDefine.mIsOpenDynamicResolutionInEditor)
            mRTS = gameObject.AddComponent<RenderTargetScaler>();
#endif
    }

    //[System.Serializable]
    public class QuadTree
    {
        public QuadTree Parent;
        public List<QuadTree> children = new List<QuadTree>();
        internal Vector3[] Extends = new Vector3[2];
        public List<MeshRenderer> objs = new List<MeshRenderer>();
        public List<ParticleSystem> particles = new List<ParticleSystem>();
        public Vector3 center;
        public float hScale;
        public bool End = false;
        internal bool Active = true;
        internal int ParticleState = 0;
        public List<QuadTree> Neighbors;
        //internal string name;

        public void SetActive(bool active)
        {
            if (Active != active)
            {
                Active = active;
                for (int i = 0; i < objs.Count; i++)
                {
                    if (objs[i] != null)
                    {
                        objs[i].enabled = active;
                    }
                }

            }
        }

        public void SwitchParticles(int active)
        {
            if (ParticleState != active)
            {
                ParticleState = active;
                for (int i = 0; i < particles.Count; i++)
                {
                    if (particles[i] != null)
                    {
                        if (active == 0 && !particles[i].isPlaying)
                        {
                            particles[i].Play();
                        }
                        else if (active == 1 && !particles[i].isPaused)
                        {
                            particles[i].Play();
                            particles[i].Pause();
                        }
                        else if (active == 2 && !particles[i].isStopped)
                        {
                            particles[i].Play();
                            particles[i].Stop();
                        }
                    }
                }

            }
        }

        public bool InTrunck(Vector3 pos)
        {
            pos.y = 0;
            return (Vector3.Min(pos, Extends[0]) == Extends[0] && Vector3.Max(pos, Extends[1]) == Extends[1]);
        }

        internal QuadTree FindEndTrunck(Vector3 position)
        {
            QuadTree res = null;
            if (!End)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i].InTrunck(position))
                    {
                        res = children[i].FindEndTrunck(position);
                        if (res != null)
                        {
                            return res;
                        }
                    }
                }
            }
            else
            {
                res = this;
            }
            return res;
        }

        internal void ActiveNeibors()
        {
            if (Neighbors != null)
            {
                for (int i = 0; i < Neighbors.Count; i++)
                {
                    if (Neighbors[i] != null)
                    {
                        Neighbors[i].SetActive(true);
                    }
                }
            }
        }

        internal void Divide(int maxNum, int Depth, int mMaxDepth)
        {
            center = (Extends[0] + Extends[1]) * 0.5f;
            Vector3 size = (Extends[1] - Extends[0]) * 0.5f;
            hScale = Vector3.Magnitude(size);

            QuadTree child0 = new QuadTree();
            child0.Extends[0] = center;
            child0.Extends[1] = center + size;
            children.Add(child0);

            QuadTree child1 = new QuadTree();
            child1.Extends[0] = center - size;
            child1.Extends[1] = center;
            children.Add(child1);

            QuadTree child2 = new QuadTree();
            child2.Extends[0] = center - new Vector3(size.x, 0, 0);
            child2.Extends[1] = center + new Vector3(0, 0, size.z);
            children.Add(child2);

            QuadTree child3 = new QuadTree();
            child3.Extends[0] = center - new Vector3(0, 0, size.z);
            child3.Extends[1] = center + new Vector3(size.x, 0, 0);
            children.Add(child3);

            Vector3 posObj;
            for (int i = 0; i < objs.Count; i++)
            {
                for (int j = 0; j < children.Count; j++)
                {
                    posObj = objs[i].transform.position;
                    posObj.y = 0;
                    if (Vector3.Min(posObj, children[j].Extends[0]) == children[j].Extends[0]
                        && Vector3.Max(posObj, children[j].Extends[1]) == children[j].Extends[1])
                    {
                        children[j].objs.Add(objs[i]);
                    }
                }
            }
            for (int i = 0; i < children.Count; i++)
            {
                children[i].Parent = this;
                //children[i].name = this.name + "/ c" + i;
                if (children[i].objs.Count > maxNum && Depth <= mMaxDepth)
                {
                    children[i].Divide(maxNum, Depth + 1, mMaxDepth);
                }
                else
                {
                    children[i].End = true;
                    children[i].center = (children[i].Extends[0] + children[i].Extends[1]) * 0.5f;
                    size = (children[i].Extends[1] - children[i].Extends[0]) * 0.5f;
                    children[i].hScale = Vector3.Magnitude(size);
                }
            }
        }

        internal void Destroy()
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].Destroy();
            }
            objs.Clear();
            particles.Clear();
            children.Clear();
        }
    }

    void Start()
    {
        ParseScene();
        if (EndTrees.Count > 16)
        {
            if(mRTS != null) mRTS.UseScreenScaler = true;
        }
        if (LodAnimation)
        {
            LODGroup[] lods = FindObjectsOfType<LODGroup>();
            for (int i = 0; i < lods.Length; i++)
            {
                lods[i].fadeMode = LODFadeMode.CrossFade;
                lods[i].animateCrossFading = true;
            }
        }
    }

    public void OnDestroy()
    {
        for (int i = 0; i < EndTrees.Count; i++)
        {
            EndTrees[i].SetActive(true);
            EndTrees[i].SwitchParticles(0);
        }
        ClearScene();
    }

    public void OnDisable()
    {
        for (int i = 0; i < EndTrees.Count; i++)
        {
            EndTrees[i].SetActive(true);
            EndTrees[i].SwitchParticles(0);
        }
    }

    private void ClearScene()
    {
        Tree.Destroy();
        Tree = new QuadTree();
        EndTrees.Clear();
    }

    //public void OnEnable()
    //{
    //    ParseScene();
    //}

    private void ParseScene()
    {
        MeshRenderer[] mrs = FindObjectsOfType<MeshRenderer>();

        Extends[0] = Vector3.one * 9999999;
        Extends[1] = Vector3.one * -9999999;
        float farDis = GetComponent<Camera>().farClipPlane + 600;
        //Shader billboardShader = UnityUtils.FindShader("SAO_TJia/BRDF_Billboard");

        List<Shader> NewMatShaders = new List<Shader>();
        NewMatShaders.Add(UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Terrain)"));
        NewMatShaders.Add(UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(AlphaBlend)"));
        NewMatShaders.Add(UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Cutoff)"));
        NewMatShaders.Add(UnityUtils.FindShader("SAO_TJia_V3/Obj/BRDF_Billboard"));
        NewMatShaders.Add(UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Opaque)"));

        for (int i = 0; i < mrs.Length; i++)
        {
            //Disable GPU Instancing
            /*Material[] mats = mrs[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                if (mats[j] != null)
                {
                    mats[j].enableInstancing = false;
                    if (mats[j].shader == billboardShader)
                    {
                        mats[j].enableInstancing = true;
                    }
                }
            }*/

            if (mrs[i].enabled == true && !this.mCullLayerList.Contains(mrs[i].gameObject.layer))
            {
                foreach (Material mat in mrs[i].sharedMaterials)
                {
                    if (NewMatShaders.Contains(mat.shader) && mat.enableInstancing == false)
                    {
                        mat.enableInstancing = true;
                    }
                }

                float mrHalfSize = Vector3.Magnitude(mrs[i].bounds.extents);
                if (mrHalfSize < 8.33f * 0.5f)
                {
                    QuadIgnore qi = mrs[i].GetComponent<QuadIgnore>();
                    if (qi == null)
                    {
                        Vector3 objPos = mrs[i].transform.position;
                        if (Vector3.Distance(objPos, transform.position) < farDis)
                        {
                            objPos.y = 0;
                            Extends[0] = Vector3.Min(Extends[0], objPos);
                            Extends[1] = Vector3.Max(Extends[1], objPos);
                            Tree.objs.Add(mrs[i]);
                        }
                    }
                    else
                    {
                        Destroy(qi);
                    }
                }
                if (mrHalfSize < 8.33f)
                {
                    if (/*mrs[i].name.Contains("LOD1") || */mrs[i].name.Contains("LOD2"))
                    {
                        mrs[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                }
            }
        }

        NewMatShaders.Clear();
        NewMatShaders = null;

        MaxTransPerTrunk = MaxTransPerTrunk > Tree.objs.Count / MaxTrancks ? MaxTransPerTrunk : Tree.objs.Count / MaxTrancks;
        Extends[0] *= 1.1f;
        Extends[1] *= 1.1f;
        if (mrs.Length > 0)
        {
            Extends[0].y = Extends[1].y = 0;
            Tree.Extends[0] = Extends[0];
            Tree.Extends[1] = Extends[1];
        }
        //Tree.name = "Root";
        if (mrs.Length > MaxTransPerTrunk)
        {
            Tree.Divide(MaxTransPerTrunk, 0, mMaxDepth);
        }
        else
        {
            Tree.End = true;
        }
        FindEndTrees(Tree);
        AddParticlesInEndTrees();

        gameObject.AddComponent<HideSmallDetail>();
        //Destroy(this);
        //return;
    }

    private void AddParticlesInEndTrees()
    {
        ParticleSystem[] pss = FindObjectsOfType<ParticleSystem>();
        for (int i = 0; i < pss.Length; i++)
        {
            if (pss[i].GetComponent<Renderer>().enabled && pss[i].isPlaying)
            {
                QuadIgnore qi = pss[i].GetComponent<QuadIgnore>();
                if (qi == null)
                {
                    QuadTree end = Tree.FindEndTrunck(pss[i].transform.position);
                    if (end != null)
                    {
                        end.particles.Add(pss[i]);
                    }
                }
                else
                {
                    Destroy(qi);
                }
            }
        }
    }

    private void FindEndTrees(QuadTree tree)
    {
        if (tree.End)
        {
            EndTrees.Add(tree);
        }
        else
        {
            for (int i = 0; i < tree.children.Count; i++)
            {
                FindEndTrees(tree.children[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bLodAnimation != LodAnimation)
        {
            bLodAnimation = LodAnimation;
            if (LodAnimation)
            {
                LODGroup[] lods = FindObjectsOfType<LODGroup>();
                for (int i = 0; i < lods.Length; i++)
                {
                    lods[i].fadeMode = LODFadeMode.CrossFade;
                    lods[i].animateCrossFading = true;
                }
            }
            else
            {
                LODGroup[] lods = FindObjectsOfType<LODGroup>();
                for (int i = 0; i < lods.Length; i++)
                {
                    lods[i].fadeMode = LODFadeMode.None;
                }
            }
        }

        if (SwitchGizmos)
        {
            SwitchGizmos = false;
            mSwitchGizmos = !mSwitchGizmos;
        }
        if (mPosMemo != transform.position || mRotMemo != transform.rotation)
        {
            mPosMemo = transform.position;
            mRotMemo = transform.rotation;

            if (Tree != null)
            {
                hideAngle = HideAngle * Tree.hScale * 0.001f;
            }
            tanHideAngle = Mathf.Tan(hideAngle * 0.5f * Mathf.Deg2Rad);
            OctreeCulling();
        }

        if (ShowQuadtree)
        {
            DrawTree(Tree);
        }
    }

    private void OctreeCulling()
    {
        Vector3 pos = transform.position;
        pos.y = 0;
        if (CurrentPos != null && !CurrentPos.InTrunck(pos))
        {
            CurrentPos = null;
        }
        if (CurrentPos == null || CurrentPos.End == false)
        {
            CurrentPos = Tree.FindEndTrunck(pos);
            if (CurrentPos != null && CurrentPos.Neighbors == null)
            {
                AddNeighbors(CurrentPos);
            }
        }
        if (CurrentPos != null)
        {
            Vector3 forward = transform.forward;
            for (int i = 0; i < EndTrees.Count; i++)
            {
                QuadTree thisTree = EndTrees[i];
                Vector3 dir = (thisTree.center - pos);
                float dis = Vector3.Magnitude(dir);

                if (dis > 0)
                {
                    float oneDivDis = 1 / dis;
                    float tanAngle = thisTree.hScale * oneDivDis;
                    float cosAngle = Vector3.Dot(forward, dir) * oneDivDis;
                    bool active = true;
                    bool buffer = false;
                    if (thisTree != CurrentPos)
                    {
                        if (cosAngle > 0.0f)
                        {
                            if (dis > MinDis * 3 && tanAngle < tanHideAngle)
                            {
                                active = false;
                            }
                        }
                        else
                        {
                            if (dis > MinDis)
                            {
                                active = false;
                            }
                        }
                        if (dis < thisTree.hScale * 4)
                        {
                            if (CurrentPos.Neighbors.Contains(thisTree))
                            {
                                active = true;
                                buffer = true;
                            }
                        }
                    }
                    thisTree.SetActive(active);
                    int localParticleState = 0;
                    if (thisTree == CurrentPos)
                    {
                        localParticleState = 0;
                    }
                    else if (buffer)
                    {
                        localParticleState = 0;
                    }
                    else if (cosAngle > 0.0f)
                    {
                        if (dis < 70)
                        {
                            localParticleState = 0;
                        }
                        else if (dis < 120 && tanAngle > tanHideAngle)
                        {
                            localParticleState = 1;
                        }
                        else
                        {
                            localParticleState = 2;
                        }
                    }
                    else
                    {
                        if (dis < 20)
                        {
                            localParticleState = 0;
                        }
                        else if (dis < 40 && tanAngle > tanHideAngle)
                        {
                            localParticleState = 1;
                        }
                        else
                        {
                            localParticleState = 2;
                        }
                    }
                    thisTree.SwitchParticles(localParticleState);
                }
            }
            CurrentPos.SetActive(true);
            //CurrentPos.ActiveNeibors();
        }

    }

    private void AddNeighbors(QuadTree tree)
    {
        if (tree.Neighbors == null)
        {
            tree.Neighbors = new List<QuadTree>();
            float length = tree.hScale * 0.7071f * 1.1f;
            tree.Neighbors.Add(Tree.FindEndTrunck(tree.center + new Vector3(length, 0, 0)));
            tree.Neighbors.Add(Tree.FindEndTrunck(tree.center - new Vector3(length, 0, 0)));
            tree.Neighbors.Add(Tree.FindEndTrunck(tree.center + new Vector3(0, 0, length)));
            tree.Neighbors.Add(Tree.FindEndTrunck(tree.center - new Vector3(0, 0, length)));
            tree.Neighbors.Add(Tree.FindEndTrunck(tree.center + new Vector3(length, 0, length)));
            tree.Neighbors.Add(Tree.FindEndTrunck(tree.center - new Vector3(length, 0, length)));
            tree.Neighbors.Add(Tree.FindEndTrunck(tree.center + new Vector3(length, 0, -length)));
            tree.Neighbors.Add(Tree.FindEndTrunck(tree.center - new Vector3(length, 0, -length)));
        }
    }

    void OnDrawGizmos()
    {
        if (ShowQuadtree)
        {
            DrawTree(Tree);
        }
    }

    private void DrawTree(QuadTree tree)
    {
        if (tree.End)
        {
            if (mSwitchGizmos)
            {
                if (tree.Active)
                {
                    Gizmos.color = Color.cyan;
                }
                else
                {
                    Gizmos.color = Color.yellow;
                }
            }
            else
            {
                if (tree.ParticleState == 0)
                {
                    Gizmos.color = Color.cyan;
                }
                else if (tree.ParticleState == 1)
                {
                    Gizmos.color = Color.yellow;
                }
                else if (tree.ParticleState == 2)
                {
                    Gizmos.color = Color.red;
                }
            }

            Gizmos.DrawWireCube((tree.Extends[1] + tree.Extends[0]) * 0.5f, (tree.Extends[1] - tree.Extends[0]));
        }
        if (tree.children.Count > 0)
        {
            for (int i = 0; i < tree.children.Count; i++)
            {
                DrawTree(tree.children[i]);
            }
        }
    }
}
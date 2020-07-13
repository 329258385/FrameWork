//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-07-09
// Author: LJP
// Date: 2020-07-09
// Description: 接入 AstarPathfindingProject 导航插件
//---------------------------------------------------------------------------------------
using System.Collections;
using UnityEngine;







public class AstarPathGo : MonoBehaviour
{
    public enum PointNavModel
    {
        /// <summary>
        /// 开启短距离直接使用navmesh（使用路点的情况下）
        /// </summary>
        POINT_OPEN_LESS_DIRECT_REACH = 1,
        /// <summary>
        /// 起始点，目标点能否直达
        /// </summary>
        POINT_OPEN_START_END_DIRECT_REACH = 2,
        /// <summary>
        /// 使用路点，并检测路点能都直接到目标点，则删除中间过渡路点（前置条件场景有路点）
        /// </summary>
        NAV_OPEN_MIDDLE_DIRECT_REACH = 4,
        /// <summary>
        /// 如果导航路点路径的第一个点和第二个点是转折的话，第一个点抛弃
        /// 如果导航路点路径的最后一个点和倒数第二个点转折是，抛弃最后一个点
        /// </summary>
        NAV_OPEN_POINT_FIRST_END_DEL = 8,
    }


    public static AstarPathGo Instance
    {
        get;
        private set;
    }
    private bool    needUpdate = false;
    public int      maxCalcCount = 40;
    public float    startPointCalc = 12;
    public float    endPointCalc = 10;
    public bool     calcY = true;
    public bool     calcStartCanReach = true;

    float           _sqrStartPointCalc = 0;
    public float    SqrStartPointCalc
    {
        get
        {
            return _sqrStartPointCalc;
        }
    }
    float           _sqrEndPointCalc = 0;
    public float    SqrEndPointCalc
    {
        get
        {
            return _sqrEndPointCalc;
        }
    }

    public PointNavModel pointNavSetting = PointNavModel.POINT_OPEN_LESS_DIRECT_REACH | PointNavModel.NAV_OPEN_MIDDLE_DIRECT_REACH | PointNavModel.POINT_OPEN_START_END_DIRECT_REACH;
    private void    Awake()
    {
        Instance            = this;
        _sqrStartPointCalc  = startPointCalc * startPointCalc;
        _sqrEndPointCalc    = endPointCalc * endPointCalc;

        pointNavSetting     =  PointNavModel.POINT_OPEN_LESS_DIRECT_REACH | PointNavModel.NAV_OPEN_POINT_FIRST_END_DEL;
        AstarPath path      = GetComponent<AstarPath>();
        Instance.UpdatePath();
        SetMeshHide();
    }

    private void    OnDestroy()
    {
        Instance = null;
    }

    private void    SetMeshHide()
    {
        MeshRenderer[] renders      = transform.GetComponentsInChildren<MeshRenderer>();
        MeshRenderer[] meshFilter   = transform.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renders.Length; i++)
        {
            if (renders[i] != null)
            {
                renders[i].enabled = false;
            }
        }
        for (int i = 0; i < meshFilter.Length; i++)
        {
            if (meshFilter[i] != null)
            {
                meshFilter[i].enabled = false;
            }
        }
    }

    public void     UpdatePath()
    {
        needUpdate  = true;
        StartCoroutine(WaitToCutAStar());
    }

    IEnumerator     WaitToCutAStar()
    {
        //等一帧处理，扎样的话如果同一帧有几十个需要更新的话，只需要计算一次
        yield return null;
        if (needUpdate)
        {
            StartCoroutine(PathRecalculate2()); 
            needUpdate = false;
        }
    }
 

    /// <summary>
    /// 采用的是路点里面单独删除节点方法
    /// </summary>
    /// <returns></returns>
    public IEnumerator PathRecalculate2()
    {
        int layer = LayerMask.GetMask("Scenes_mesh");
        Pathfinding.NodeLink[] links = gameObject.GetComponentsInChildren<Pathfinding.NodeLink>();
        int count = 0;
        for (int i = 0; i < links.Length; i++)
        {
            if (links[i].End != null)
            {

                Vector3 dir     = links[i].End.position - links[i].Start.position;
                float length    = dir.magnitude;
                if (Physics.Raycast(links[i].Start.position, dir, length, layer) || Physics.Raycast(links[i].End.position, -dir, length, layer))
                {
                    links[i].deleteConnection = true;
                    links[i].Apply();
                }
                else
                {
                    links[i].deleteConnection = false;
                    links[i].InternalOnPostScan();
                }

                count++;
                if (count >= maxCalcCount)
                {
                    count = 0;
                    yield return null;
                }
            }
        }
        if (AstarPath.active)
        {
            AstarPath.active.Scan();
        }
    }
}

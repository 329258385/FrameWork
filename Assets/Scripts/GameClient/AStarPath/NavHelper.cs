//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-07-09
// Author: LJP
// Date: 2020-07-09
// Description: 接入 AstarPathfindingProject 导航插件
//---------------------------------------------------------------------------------------
using ActClient;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



// unity NavMesh helper
public class NavHelper
{

    /// <summary>
    /// 获得路径点
    /// </summary>
    public static void GetNavPath(Vector3 start, Vector3 target,List<Vector3> paths)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        bool result = NavMesh.CalculatePath(start, target, -1, navMeshPath);
        switch (navMeshPath.status)
        {
            case NavMeshPathStatus.PathComplete:
                {
                    paths.AddRange(navMeshPath.corners);
                    break;
                }
            case NavMeshPathStatus.PathPartial:
                {
                    paths.AddRange(navMeshPath.corners);
                    GetNavPath(paths[paths.Count - 1], target, paths);
                    break;
                }
            case NavMeshPathStatus.PathInvalid:
                {
                    break;
                }
        }
    }

    public static List<NavMeshPath> GetEmptyPath()
    {
        return new List<NavMeshPath>();
    }

    public static List<NavMeshPath> GetRealPath(Vector3 start, Vector3 target)
    {
        List<NavMeshPath> paths = new List<NavMeshPath>();
        CanReachPoint(start, target, paths);
        return paths;
    }

   
    public static bool CanReachPoint(Vector3 start, Vector3 target, List<NavMeshPath> paths)
    {
        paths.Clear();
        //直接计算导航能否到达，能到达返回true
        bool result = CanReachPointOnlyByNavMesh(start, target, paths);
        if (result) { return result; }

        return false;
    }

    public static bool CanReachPointOnlyByNavMesh(Vector3 start, Vector3 target, List<NavMeshPath> paths)
    {
        int count = 5;
        //直接检测是否可以达到目标点
        while (count > 0)
        {
            NavMeshPath navMeshPath = new NavMeshPath();
            bool result = NavMesh.CalculatePath(start, target, -1, navMeshPath);

            switch (navMeshPath.status)
            {
                case NavMeshPathStatus.PathComplete:
                    {
                        paths.Add(navMeshPath);
                        return true;
                    }
                case NavMeshPathStatus.PathPartial:
                    {
                        paths.Add(navMeshPath);
                        start = navMeshPath.corners[navMeshPath.corners.Length - 1];
                        break;
                    }
                case NavMeshPathStatus.PathInvalid:
                    {
                        paths.Clear();
                        return false;
                    }
            }
            count--;
        }
        paths.Clear();
        return false;
    }

    /// <summary>
    /// 当前是否有路点导航
    /// </summary>
    public static bool HasAStarPoint()
    {
        return (AstarPathGo.Instance != null);
    }

    /// <summary>
    /// 两点直接是否有障碍物，没有障碍物意味着能直接到达
    /// </summary>
    public static bool FastRaycast(Vector3 start, Vector3 end)
    {
        return GlobalHelper.FastRaycastReach(start,end, LayerDefine.ObstanceLayerMask);
    }
}

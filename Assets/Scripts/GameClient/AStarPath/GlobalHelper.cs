//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-07-09
// Author: LJP
// Date: 2020-07-09
// Description: 接入 AstarPathfindingProject 导航插件
//---------------------------------------------------------------------------------------
using UnityEngine;






public static class GlobalHelper
{

    public static bool FastRaycastReach(Vector3 start, Vector3 end, int layer)
    {
        Vector3 dir = end - start;
        Vector3 normal = Vector3.Cross(dir, Vector3.up);
        normal.Normalize();
        //bool result1 = Physics.Raycast(start, dir, out gHit, dir.magnitude, layer, QueryTriggerInteraction.Ignore);
        //左下
        bool result2 = Physics.Raycast(start + normal * 0.3f + new Vector3(0, 0.3f, 0), dir, dir.magnitude, layer, QueryTriggerInteraction.Ignore);
        //bool result3 = Physics.Raycast(start - normal * 0.3f, dir, dir.magnitude, layer, QueryTriggerInteraction.Ignore);
        //bool result4 = Physics.Raycast(start + normal * 0.3f + new Vector3(0, 1.8f,0), dir, dir.magnitude, layer, QueryTriggerInteraction.Ignore);
        //右上
        bool result5 = Physics.Raycast(start - normal * 0.3f + new Vector3(0, 1.8f, 0), dir, dir.magnitude, layer, QueryTriggerInteraction.Ignore);
        //Debug.DrawLine(start + normal * 0.3f, end + normal * 0.3f, Color.red,1000);
        //Debug.DrawLine(start - normal * 0.3f, end - normal * 0.3f, Color.red, 1000);
        //Debug.DrawLine(start + normal * 0.3f + new Vector3(0, 1.8f, 0), end + normal * 0.3f + new Vector3(0, 1.8f, 0), Color.red, 1000);
        //Debug.DrawLine(start - normal * 0.3f + new Vector3(0, 1.8f, 0), end - normal * 0.3f + new Vector3(0, 1.8f, 0), Color.red, 1000);
        bool result = !result2 && !result5;
        if (result)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
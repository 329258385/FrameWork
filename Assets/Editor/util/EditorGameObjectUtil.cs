using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class EditorGameObjectUtil
{
    public class MyComponentList
    {
        public MyComponentList()
        {
        }

        public List<Component> gameObjList;
        public List<MyComponentList> nextList;
    }
    public static void DeepCopy(GameObject obj,GameObject target)
    {
        MyComponentList pri_my_list = new MyComponentList();
        GetAllChilds(obj,ref pri_my_list);
        PasteChildComponent(target, pri_my_list);
    }
    

    private static void GetAllChilds(GameObject transformForSearch,ref MyComponentList next)
    {
        List<Component> childsOfGameobject = new List<Component>();
        next.gameObjList = childsOfGameobject;
        next.nextList = new List<MyComponentList>();

        foreach (var item in transformForSearch.GetComponents<Component>())
        {
            childsOfGameobject.Add(item);
        }

        foreach (Transform item in transformForSearch.transform)
        {
            MyComponentList tmpnext = new MyComponentList();
            GetAllChilds(item.gameObject,ref tmpnext);
            next.nextList.Add(tmpnext);
        }
        return;
    }

    private static void PasteChildComponent(GameObject gameObj, MyComponentList next)
    {
        if (next.gameObjList != null)
        {
            foreach (var copiedComponent in next.gameObjList)
            {
                if (!copiedComponent) continue;

                UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(gameObj);
            }
        }

        if (next.nextList != null)
        {
            List<Transform> TmpListTrans = new List<Transform>();
            foreach (Transform item in gameObj.transform)
            {
                TmpListTrans.Add(item);
            }
            int i = 0;
            foreach (var item in next.nextList)
            {
                if (i < TmpListTrans.Count)
                {
                    PasteChildComponent(TmpListTrans[i].gameObject, item);
                }
                i++;
            }
        }
    }


    

}


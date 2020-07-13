//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-22
// Author: LJP
// Date: 2020-06-20
// Description: prefab 资源针对Lua 层使用资源的方式，为适应现在lua
//---------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;





namespace ActClient
{
    public class ResObject : ResBase
    {
        /// <summary>
        /// 缓存prefab, 可能hold 住了一堆资源 
        /// </summary>
       Object _object;


        override public IEnumerator LoadAsync(AssetBundle bundle)
        {
            var request     = bundle.LoadAssetAsync(_assetName, typeof(UnityEngine.Object));
            yield return request;
            _object         = request.asset;
        }


        override public void    Load(AssetBundle bundle)
        {
            _object         = bundle.LoadAsset(_assetName);
        }

        override public Object Load(AssetBundle bundle, string assetName)
        {
            _object = bundle.LoadAsset(assetName);
            return _object;
        }

        override protected void Cleanup()
        {
            if (null != _object)
            {
                //Resources.UnloadAsset(_object);
                _object = null;
            }
        }


        override public object   obj     { get { return _object; } }
        override public object[] objs { get { return null; } }
    }
}


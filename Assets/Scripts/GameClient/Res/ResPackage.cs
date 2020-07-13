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
    public class ResPackage : ResBase
    {
        /// <summary>
        /// 缓存prefab, 可能hold 住了一堆资源 
        /// </summary>
        Object[] _object;


        override public IEnumerator LoadAsync(AssetBundle bundle)
        {
            var request     = bundle.LoadAllAssetsAsync();
            yield return request;
            _object         = request.allAssets;
        }


        override public void    Load(AssetBundle bundle)
        {
            _object         = bundle.LoadAllAssets();
        }


        override protected void Cleanup()
        {
            if (null != _object)
            {
                //Resources.UnloadAsset(_object);
                for( int i = 0; i < _object.Length; i++)
                    _object[i] = null;
            }
        }


        override public object   obj     { get { return null; } }
        override public object[] objs    { get { return _object; } }
    }
}


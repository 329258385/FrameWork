//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-22
// Author: LJP
// Date: 2020-05-22
// Description: 模型资源
//---------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;





namespace ActClient
{
    public class ResMesh : ResBase
    {
        /// <summary>
        /// 缓存模型
        /// </summary>
        Mesh  _mesh;


        override public IEnumerator LoadAsync(AssetBundle bundle)
        {
            var request = bundle.LoadAssetAsync(_assetName, typeof(Mesh));
            yield return request;
            _mesh       = request.asset as Mesh;
        }


        override public void        Load(AssetBundle bundle)
        {
            _mesh = bundle.LoadAsset(_assetName) as Mesh;
        }


        override protected void     Cleanup()
        {
            if (null != _mesh)
            {
                Resources.UnloadAsset(_mesh);
                _mesh = null;
            }
        }

        override public object[]    objs { get { return null; } }
        override public object      obj { get { return _mesh; } }
        public Mesh                 mesh { get { return _mesh; } }
    }
}

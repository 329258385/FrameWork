//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-22
// Author: LJP
// Date: 2020-05-22
// Description: 纹理资源
//---------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;





namespace ActClient
{
    public class ResTexture : ResBase
    {
        /// <summary>
        /// 缓存纹理
        /// </summary>
        Texture _texture;


        override public IEnumerator LoadAsync(AssetBundle bundle)
        {
            var request     = bundle.LoadAssetAsync(_assetName, typeof(Texture));
            yield return request;
            _texture        = request.asset as Texture;
        }


        override public void    Load(AssetBundle bundle)
        {
            _texture        = bundle.LoadAsset(_assetName) as Texture;
        }

 
        override protected void Cleanup()
        {
            if (null != _texture)
            {
                Resources.UnloadAsset(_texture);
                _texture   = null;
            }
        }

        override public object[]  objs { get { return null; } }
        override public object  obj     { get { return _texture; } }
        public Texture          texture { get { return _texture; } }
    }
}


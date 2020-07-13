//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-22
// Author: LJP
// Date: 2020-05-22
// Description: 场景资源
//---------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

namespace ActClient
{
    public class ResScene : ResBase
    {
        /// <summary>
        /// 异步加载
        /// </summary>
        override public IEnumerator LoadAsync(AssetBundle bundle)
        {
            if(isRoom)
            {
                //var op = Application.LoadLevelAdditiveAsync(_assetName);
                var op = SceneManager.LoadSceneAsync(_assetName, LoadSceneMode.Additive );
                yield return op;
            }
            else
            {
                //var op = Application.LoadLevelAsync(_assetName);
                var op = SceneManager.LoadSceneAsync(_assetName);
                yield return op;
            }
        }

        /// <summary>
        /// 加载
        /// </summary>
        override public void Load(AssetBundle bundle)
        {
            if(isRoom)
            {
                //Application.LoadLevelAdditive(_assetName);
                SceneManager.LoadScene(_assetName, LoadSceneMode.Additive);
            }
            else
            {
                // 
                SceneManager.LoadScene(_assetName);
                //Application.LoadLevel(_assetName);
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        override protected void Cleanup()
        {

        }

        /// <summary>
        /// 是否是房间
        /// </summary>
        public bool             isRoom { get { return false; } }
        override public object  obj { get { return null; } }
        override public object[] objs { get { return null; } }
    }
}
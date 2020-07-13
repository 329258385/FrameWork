//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-22
// Author: LJP
// Date: 2020-05-22
// Description: 场景控制器，用于更新所有的树
//---------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;




namespace ActClient
{
    public class SceneController
    {
        /// <summary>
        /// 当前的场景描述
        /// </summary>
        static SceneDesc        _sceneDesc;

        /// <summary>
        /// 场景资源
        /// </summary>
        static ResRef           _resScene;

        /// <summary>
        /// 场景树
        /// </summary>
        static List<SceneTree>  _trees = new List<SceneTree>();

        /// <summary>
        /// 当前场景名字
        /// </summary>
        static string           _sceneName;

        /// <summary>
        /// 场景相机
        /// </summary>
        static Camera           _camera;

        /// <summary>
        /// 资源，优先资源
        /// </summary>
        static ResCollection    _resCollection;

        /// <summary>
        /// 视锥
        /// </summary>
        static Frustum          _frustum = new Frustum();

        /// <summary>
        /// 加载场景，原来的意义还保留， 以后只初始化场景树资源相关 fixed by ljp 2020-06-23
        /// </summary>
        /// <param name="sceneName"></param>
        static public void LoadQuadtree(string sceneName)
        {
            _camera             = Camera.main;
            if (!string.IsNullOrEmpty(sceneName))
                _resScene       = ResManager.LoadScene(sceneName, OnLoadSceneComplete);
            else
                OnLoadSceneComplete(null);
            _sceneName          = sceneName;
        }

        /// <summary>
        /// 销毁场景
        /// </summary>
        static public void DestroyScene()
        {
            for (int i = 0, ci = _trees.Count; i < ci; ++i)
            {
                var tree = _trees[i];
                ObjectPool.Release(ref tree);
            }
            _trees.Clear();
            _sceneName = null;
            _sceneDesc = null;
            _frustum   = null;
            ObjectPool.Release(ref _resCollection);
            ResManager.Release(ref _resScene);
        }

        /// <summary>
        /// 场景加载完成
        /// </summary>
        static void OnLoadSceneComplete(ResBase res)
        {
            if (SceneDesc.current == null)
                return;

            if(null != _sceneDesc)
            {
                Debug.LogErrorFormat("Load scene {0} repeat!", _sceneName);
                DestroyScene();
            }

            _sceneDesc = SceneDesc.current;
            ResManager.InitGrass(_sceneDesc.prefabs);

            if (null != _sceneDesc)
            {
                
                for (int i = 0; i < _sceneDesc.trees.Length; ++i)
                {
                    var tree = ObjectPool.New<SceneTree>();
                    tree.Init(_sceneDesc.trees[i]);
                    _trees.Add(tree);
                }

                // ResCollection
                _resCollection = ObjectPool.New<ResCollection>();
                _resCollection.Init(_sceneDesc.resCollection);
                _resCollection.LoadRes();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        static public void Update()
        {
            // 相机
            if(null == _camera)
            {
                return;
            }
            
            // 更新视锥
            _frustum.Update(_camera);

            var center = _camera.transform.position;
            for (int i = 0, ci = _trees.Count; i < ci; ++i)
            {
                var tree = _trees[i];
                tree.Update(center);
            }
        }

        static public string   sceneName   { get { return _sceneName; } }

        static public Frustum  frustum     { get { return _frustum; } }
    }
}
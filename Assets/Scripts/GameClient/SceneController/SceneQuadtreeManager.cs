//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-28
// Author: LJP
// Date: 2020-05-28
// Description: 场景管理器
// fixed update : 要完善 lua 和 框架内接口逻辑 2020-06-23 
// 这里改变一下概念，不再是场景的概念了，而是管理场景中四叉树的数据
// 场景的加载释放【Unity资源部分】，交由Lua层去控制， 因为四叉化的场景只是个空的框架壳子
// 场景四叉树只能……
//---------------------------------------------------------------------------------------
using XLua;



namespace ActClient
{
    [Hotfix]
    [LuaCallCSharp]
    public class SceneQuadtreeManager : MonoSingleton<SceneQuadtreeManager>
    {
        //---------------------------------------------------------------------------------------
        // 场景加载状态
        //---------------------------------------------------------------------------------------
        public enum SceneLoadState
        {
            LoadScene = 0,
            InitPlayer,
            Max,    // 状态最大值  
        }


        //---------------------------------------------------------------------------------------
        // 成员变量
        //---------------------------------------------------------------------------------------
        static private bool                 _loadComplete = false;
        static private ResRef               _resOnLoadSceneComplete;


        /// <summary>
        /// 初始化
        /// </summary>
        static public void Init()
        {
            _loadComplete       = false;
        }

        public void TestHotfix()
        {
            Logger.Log("********** AssetBundleManager : Call TestHotfix in cs...");
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void DestroydQuadtree()
        {
            // 销毁场景
            if (_loadComplete)
            {
                ResManager.Release(ref _resOnLoadSceneComplete);
                _loadComplete = false;
            }
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="deltaTime"></param>
        void Update()
        {
            if (_loadComplete)
            {
                SceneController.Update();
            }
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        public void BuildQuadtree( )
        {
            _loadComplete = false;

            // 场景收集数据
            bool isNewLevelRes = true;
            if(isNewLevelRes)
            {
                SceneController.LoadQuadtree("");
            }

            PushLoadCompleteDelegate();
        }


        /// <summary>
        /// 设置状态
        /// </summary>
        void SetLoadStateFinish(SceneLoadState state)
        {
            _loadComplete = true;
        }

        /// <summary>
        /// 设置加载完成回调
        /// </summary>
        void PushLoadCompleteDelegate()
        {

            ResManager.Release(ref _resOnLoadSceneComplete);
            if (string.IsNullOrEmpty(SceneController.sceneName))
            {
                _resOnLoadSceneComplete = ResManager.PushDelegate(OnLoadSceneAllAssetsComplete);
            }
        }

        /// <summary>
        /// 场景资源加载完成
        /// </summary>
        private void OnLoadSceneAllAssetsComplete(object obj)
        {
            SetLoadStateFinish(SceneLoadState.LoadScene);
        }


        //---------------------------------------------------------------------------------------
        // getters
        //---------------------------------------------------------------------------------------
        public int   sceneId { get { return -1; } }
        public bool  loadComplete { get { return _loadComplete; } }
    }
}
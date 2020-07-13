--[[
-- added by ljp 2020-06-23
-- 四叉树资源管理
-- 将场景四叉树管理的资源，作为lua 层资源管理的一部分，主要是跟 unity 层交互，调用频率一改非常低
--]]

local SceneQuadtreeManager = BaseClass("SceneQuadtreeManager", Singleton)


local QuadtreeManager = CS.ActClient.SceneQuadtreeManager.Instance

-- 场景加载完成后
-- 构建四叉树
local function BuildQuadtree(self)

	QuadtreeManager:BuildQuadtree()
end

-- 切换场景，销毁死叉树
local function DestroydQuadtree(self)

	QuadtreeManager:DestroydQuadtree()

end

-- 场景加载完毕
local function OnComplete(self)
end


SceneQuadtreeManager.BuildQuadtree = BuildQuadtree
SceneQuadtreeManager.DestroydQuadtree = DestroydQuadtree
SceneQuadtreeManager.OnComplete = OnComplete


return SceneQuadtreeManager
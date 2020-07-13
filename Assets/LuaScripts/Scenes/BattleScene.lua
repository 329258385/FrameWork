--[[
-- added by wsh @ 2017-11-19
-- 战斗场景
-- TODO：这里只是做一个战斗场景展示Demo，大部分代码以后需要挪除
--]]
local BattleScene = BaseClass("BattleScene", BaseScene)
local base = BaseScene

local CharacterAnimation = require "GameLogic.Battle.CharacterAnimation"
local MonstarAnimation = require "GameLogic.Battle.Monstar"

-- 临时：角色资源路径
local chara_res_path = "Models/hero_01/Hero_01.prefab"  --"Models/m_01/m_01.prefab"--
local monstar_res_path = "Models/laohuxi_katelinu/101011.prefab"

-- 创建：准备预加载资源
local function OnCreate(self)
    base.OnCreate(self)
    -- TODO
    -- 预加载资源
    self:AddPreloadResource(chara_res_path, typeof(CS.UnityEngine.GameObject), 1)
    self:AddPreloadResource(UIConfig[UIWindowNames.UIBattleMain].PrefabPath, typeof(CS.UnityEngine.GameObject), 1)

    -- 临时：角色动画控制脚本
    self.charaAnim = nil
end

-- 准备工作
local function OnComplete(self)
    base.OnComplete(self)

    -- 创建角色
    local chara =
        GameObjectPool:GetInstance():GetGameObjectAsync(
        chara_res_path,
        function(inst)
            if IsNull(inst) then
                error("Load chara res err!")
                do
                    return
                end
            end

            local chara_root = CS.UnityEngine.GameObject.Find("CharacterRoot")
            if IsNull(chara_root) then
                error("chara_root null!")
                do
                    return
                end
            end

            inst.transform:SetParent(chara_root.transform)
            -- 初始位置
            inst.transform.localPosition = Vector3.New(451, 60, 507)

            UIManager:GetInstance():OpenWindow(UIWindowNames.UIBattleMain)

            -- 启动角色控制
            self.charaAnim = CharacterAnimation.New()
			self.charaAnim:Start(inst)
			
			CharacterManager:GetInstance().MainPlayer=  inst.transform
			CharacterManager:GetInstance().MainPlayerLua=  self.charaAnim
        end
	)
	
	 -- 创建怪
	local monstar =
	      GameObjectPool:GetInstance():GetGameObjectAsync(
			  monstar_res_path,
			  function(inst)
				if IsNull(inst) then
					error("Load monstar res err!")
					do
						return
					end
				end
				local chara_root = CS.UnityEngine.GameObject.Find("CharacterRoot")

				inst.transform:SetParent(chara_root.transform)
				-- 初始位置
				inst.transform.localPosition = Vector3.New(105, 2, 100)

				self.monstarAnim = MonstarAnimation.New()
				self.monstarAnim:Start(inst)
			end	

	)
	
	--[[local monstar2 = GameObjectPool:GetInstance():GetGameObjectAsync(
			  monstar_res_path,
			  function(inst)
				if IsNull(inst) then
					error("Load monstar res err!")
					do
						return
					end
				end
				local chara_root = CS.UnityEngine.GameObject.Find("CharacterRoot")

				inst.transform:SetParent(chara_root.transform)
				-- 初始位置
				inst.transform.localPosition = Vector3.New(105, 2, 110)

				self.monstarAnim2 = MonstarAnimation.New()
				self.monstarAnim2:Start(inst)
			end	

	)--]]

    --temp
    --CharacterManager:GetInstance():SpawnCharacter(chara_res_path, Vector3.New(-7.86, 1.5, 5.85))
    --UIManager:GetInstance():OpenWindow(UIWindowNames.UIBattleMain)
end

-- 离开场景
local function OnLeave(self)
    --temp
    --self.charaAnim:Delete()
    --self.charaAnim = nil

    UIManager:GetInstance():CloseWindow(UIWindowNames.UIBattleMain)
    base.OnLeave(self)
end

BattleScene.OnCreate = OnCreate
BattleScene.OnComplete = OnComplete
BattleScene.OnLeave = OnLeave
BattleScene.CharacterAnimation = CharacterAnimation
return BattleScene

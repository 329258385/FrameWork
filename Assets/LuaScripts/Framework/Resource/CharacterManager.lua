--[[
-- added by LunaticToy @ 2020-02-29
-- 角色管理系统：提供角色相关管理
--]]
local Player = require "GameLogic.Battle.Character.Player"
local CharacterManager = BaseClass("CharacterManager", Singleton)

local chara_root = nil
local character_pool = {}

-- 获取角色根挂载节点
local function GetCharacterRoot()
    if not chara_root then
        chara_root = CS.UnityEngine.GameObject.Find("CharacterRoot")
    end

    return chara_root
end

-- 创建角色
local function SpawnCharacter(self, chara_res_path, position)
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

            chara_root = GetCharacterRoot()
            if IsNull(chara_root) then
                error("chara_root null!")
                do
                    return
                end
            end

            inst.transform:SetParent(chara_root.transform)
            inst.transform.localPosition = position

            -- 启动角色
            Player.New(inst)
        end
    )
end

-- 加载武器
local function LoadWeapon(self, path, parent)
    local weapon =
        GameObjectPool:GetInstance():GetGameObjectAsync(
        path,
        function(inst)
            if IsNull(inst) then
                error("Load weapon res err!")
                do
                    return
                end
            end

            chara_root = GetCharacterRoot()
            if IsNull(parent) then
                error("parent null!")
                do
                    return
                end
            end

            inst.transform:SetParent(parent.transform)
            inst.transform.localPosition = Vector3.zero
            -- unity暂时没导出骨骼，先强制设置
            inst.transform.localEulerAngles = Vector3.New(-193, -179, 0)
        end
    )

    return weapon
end

CharacterManager.GetCharacterRoot = GetCharacterRoot
CharacterManager.SpawnCharacter = SpawnCharacter
CharacterManager.LoadWeapon = LoadWeapon

return CharacterManager

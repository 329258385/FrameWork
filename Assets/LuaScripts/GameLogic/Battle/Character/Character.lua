--[[角色基类]]
local StateMachine = require "GameLogic.Battle.State.StateMachine"
local Character = BaseClass("Character")

CharacterType = {
    Player = 1,
    Monster = 2,
    Npc = 3,
    Thing = 4
}

local function __init(self, characterGo)
    Logger.Log("Character.__init")
    -- 角色预制体
    self.__characterGo = characterGo
    self.__stateMachine = StateMachine.New()
    self:OnCreate()
end

local function __delete(self)
    self:OnDestroy()
end

local function OnCreate(self)
end

local function OnDestroy()
end

-- 注册动画事件
local function RegisterAnimationEvent(clip, time, param)
    local _event = CS.UnityEngine.AnimationEvent.New()
    _event.functionName = "AnimationEventCallBack"
    -- 例如 类型|参数
    _event.stringParameter = param
    _event.time = time
    clip.AddEvent(_event)
end

-- 动画事件的回调
local function AnimationEventCallBack(stringParameter)
    Logger.Log("From Character Base")
    Logger.Log(stringParameter)
end

Character.__init = __init
Character.__delete = __delete
Character.OnCreate = OnCreate
Character.OnDestroy = OnDestroy
Character.AnimationEventCallBack = AnimationEventCallBack

return Character

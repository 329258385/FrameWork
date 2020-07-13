--[[玩家]]
local StateIdle = require "GameLogic.Battle.State.StateIdle"
local Player = BaseClass("Player", Character)
local base = Character

local function OnCreate(self)
    Logger.Log("Player.OnCreate")
    base.OnCreate(self)
    -- 角色控制器
    self.__controller = self.__characterGo:GetComponentInChildren(typeof(CS.UnityEngine.CharacterController))
    -- 动画组件
    self.__animation = self.__characterGo:GetComponentInChildren(typeof(CS.UnityEngine.Animation))
    -- 动画状态组件
    self.__animator = self.__characterGo.GetComponentInChildren(typeof(CS.UnityEngine.Animator))
    -- 初始化动画事件
    self:InitAnimationEventData(self)

    --添加状态
    --待机
    self.__stateMachine:AddState(StateIdle.New(self.__controller, self.__animation))

    --默认待机动作
    self.__stateMachine:SetState(StateType.Idle)
end

local function Update(self)
    self:UpdateGravity(self)
end

local Gravity = 9.81
local YAxisMove = nil
local function UpdateGravity(self)
    YAxisMove = Vector3.up:Mul(-Gravity)
    self.__controller:Move(YAxisMove:Mul(Time.deltaTime))
end

local function InitAnimationEventData(self)
    if self.__animator then
        local clips = self.__animator.runtimeAnimatorController.animationClips
        for i = 1, #clips do
            base.RegisterAnimationEvent(clips[i], 0.1 * i, "1|1234")
        end
    end
end

Player.OnCreate = OnCreate
Player.Update = Update

return Player

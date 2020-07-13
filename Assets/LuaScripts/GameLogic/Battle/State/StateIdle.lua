--[[赋闲状态]]
local AppearIdle = require "GameLogic.Battle.Appear.AppearIdle"
local StateIdle = BaseClass("StateIdle", State)
local base = State

local function __init(self, controller, animation)
    Logger.Log("StateIdle.__init")
    base.__init(self)
    --类型索引（配表）
    self.__typeIndex = 1
    --优先级（配表）
    self.__priorityNum = 1
    --表现（配表）
    self.__appear[#self.__appear + 1] = AppearIdle.New(controller, animation)
end

local function OnInit(self)
end

local function Execute(self)
    self.__appear[1]:Execute()
end

StateIdle.__init = __init
StateIdle.OnInit = OnInit
StateIdle.Execute = Execute

return StateIdle

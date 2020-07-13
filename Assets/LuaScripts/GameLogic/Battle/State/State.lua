--[[状态基类]]
local State = BaseClass("State")

StateType = {
    Idle = 1,
    Move = 2,
    Jump = 3,
    Skill = 4,
    Die = 5
}

local function __init(self, controller, animation)
    Logger.Log("State.__init")
    self.__appear = {}
end

local function OnInit(self)
end

--实现状态
local function Execute(self)
end

State.__init = __init
State.OnInit = OnInit
State.Execute = Execute

return State

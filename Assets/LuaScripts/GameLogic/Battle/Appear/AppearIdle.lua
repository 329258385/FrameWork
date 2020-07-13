--[[赋闲表现]]
local AppearIdle = BaseClass("AppearIlde", Appear)
local base = Appear

local function __init(self, controller, animation)
    self.__con = controller
    self.__ani = animation
    self.__aniName = "soldierIdle"
    self.__isLoop = true
end

local function OnInit(self)
end

local function Execute(self)
    self.__ani:CrossFade(self.__aniName)
end

local function Exit(self)
end

local function Update(self)
    base:Update(self)
end

AppearIdle.__init = __init
AppearIdle.OnInit = OnInit
AppearIdle.Execute = Execute
AppearIdle.Exit = Exit

return AppearIdle

--[[表现基类]]
local Appear = BaseClass("Appear", Updatable)

local function __init(self, controller, animation)
end

local function OnInit(self)
end

local function Execute(self)
end

local function Exit(self)
end

local function Update(self)
end

Appear.__init = __init
Appear.OnInit = OnInit
Appear.Execute = Execute
Appear.Exit = Exit
Appear.Update = Update

return Appear

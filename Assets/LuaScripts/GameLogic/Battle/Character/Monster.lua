--[[怪物]]
local Monster = BaseClass("Monster", Character)
local base = Character

local function Start(self, chra_go)
end

local function __delete(self)
end

Monster.Start = Start
Monster.__delete = __delete

return Monster

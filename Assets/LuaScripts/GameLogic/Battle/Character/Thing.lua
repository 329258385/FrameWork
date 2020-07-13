--[[可交互物品]]
local Thing = BaseClass("Thing", Character)
local base = Character

local function Start(self, chra_go)
end

local function __delete(self)
end

Thing.Start = Start
Thing.__delete = __delete

return Thing

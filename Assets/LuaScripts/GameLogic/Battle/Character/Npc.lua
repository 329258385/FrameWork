--[[Npc]]
local Npc = BaseClass("Npc", Character)
local base = Character

local function Start(self, chra_go)
end

local function __delete(self)
end

Npc.Start = Start
Npc.__delete = __delete

return Npc

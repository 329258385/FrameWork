require 'behavior3_lua.core.Action'

local inViewTarget = b3.Class("InViewTarget", b3.Action)
b3.InViewTarget = inViewTarget

function inViewTarget:ctor(settings)
	b3.Action.ctor(self, settings)
	self.name = "InViewTarget"
	self.title = "视野内的目标"
	self.properties = {fieldOfViewAngle = "90",sharedViewDistance = "7"}
	self.viewTargets = {}
	settings = settings or {}
end

function inViewTarget:tick(tick)
	return b3.SUCCESS
end

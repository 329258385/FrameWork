require 'behavior3_lua.core.Action'

local logAction = b3.Class("LogAction", b3.Action)
b3.LogAction = logAction

function logAction:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "LogAction"
	self.title = "打印日志"
	self.properties = {logStr = "333",}

	settings = settings or {}
end

function logAction:tick(tick)
	print(self.properties.logStr);
	return b3.SUCCESS
end

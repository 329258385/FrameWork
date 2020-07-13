require 'behavior3_lua.core.Action'
----执行气泡说话，立即返回，值始终为true
local speek = b3.Class("Speek", b3.Action)
b3.Speek = speek

function speek:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "Speek"
	self.title = "气泡说话"
	self.properties = {context = "你是谁？", time = "2"}
	settings = settings or {}
end

function speek:tick(tick)
	--local result = tick.targetLua:Speek(self.properties.context,self.properties.time);--调用说话接口
	print(self.properties.context);
	return b3.SUCCESS
end

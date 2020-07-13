require 'behavior3_lua.core.BaseNode'

local condition = b3.Class("Condition", b3.BaseNode)
b3.Condition = condition

function condition:ctor(params)
	b3.BaseNode.ctor(self, params)
end
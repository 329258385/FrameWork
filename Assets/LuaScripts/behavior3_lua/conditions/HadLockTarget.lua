require 'behavior3_lua.core.Condition'
--仇恨目标 
--判断有没有仇恨目标，有返回true，否则返回false
--读仇恨列表
local hadLockTarget = b3.Class("HadLockTarget", b3.Condition)
b3.HadLockTarget = hadLockTarget

function hadLockTarget:ctor(settings)
	b3.Condition.ctor(self, settings)
	self.name = "HadLockTarget"
	self.title = "仇恨目标"
	self.properties = {}
	self.targetTf = nil
	settings = settings or {}
end

function hadLockTarget:open(tick)
end

function hadLockTarget:tick(tick)
	if(tick.targetLua  ~= nil and tick.targetLua.hatredTargets ~= nil)then
		for k, v in pairs(tick.targetLua.hatredTargets) do
			if(not IsNull(v))then
				return b3.SUCCESS;
			end
		end
	end
	return b3.FAILURE;
end

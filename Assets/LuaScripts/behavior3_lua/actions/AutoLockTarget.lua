require 'behavior3_lua.core.Action'
--自动寻找仇恨目标，把仇恨列表中的第一个作为仇恨目标，执行完返回，值始终为true
local autoLockTarget = b3.Class("AutoLockTarget", b3.Action)
b3.AutoLockTarget = autoLockTarget

function autoLockTarget:ctor(settings)
	b3.Action.ctor(self, settings)
	self.name = "AutoLockTarget"
	self.title = "仇恨列表中的第一个作为仇恨目标"
	self.properties = {}
	self.viewTargets = {}
	settings = settings or {}
end

function autoLockTarget:open(tick)
	tick.blackboard:set('LookTargetInfo',nil)
end

function autoLockTarget:tick(tick)
	if(tick.targetLua  ~= nil and tick.targetLua.hatredTargets ~= nil)then
		for k, v in pairs(tick.targetLua.hatredTargets) do
			if(not IsNull(v))then
				-- v {targetTf,targetLua}
				tick.blackboard:set('LookTargetInfo',v)
				return b3.SUCCESS;
			end
		end
	end
	return b3.SUCCESS
end

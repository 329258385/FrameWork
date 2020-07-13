require 'behavior3_lua.core.Condition'
--目标血量
--判断目标单位的血量百分比是否在指定范围内，如果是返回true，否则返回false
--hp percent | float
local checkTargetHpPercent = b3.Class("CheckTargetHpPercent", b3.Condition)
b3.CheckTargetHpPercent = checkTargetHpPercent

function checkTargetHpPercent:ctor(settings)
	b3.Condition.ctor(self, settings)
	self.name = "CheckTargetHpPercent"
	self.title = "目标单位的血量百分比是否在指定范围内"
	self.properties = {hpPercent = "0.5"}
	settings = settings or {}
end

function checkTargetHpPercent:open(tick)
	local targetInfo = tick.blackboard:get('LookTargetInfo')
	self.targetLua = targetInfo.targetLua;
end

function checkTargetHpPercent:tick(tick)
	if( (not IsNull(self.targetLua)) and (not IsNull(self.targetLua.curHpPer)) )then
		local curHpPer = self.targetLua.curHpPer;
		if (curHpPer < self.properties.hpPercent)then
			return b3.SUCCESS;
		end
	end
	
	return b3.FAILURE
end

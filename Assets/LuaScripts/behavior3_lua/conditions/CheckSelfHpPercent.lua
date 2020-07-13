require 'behavior3_lua.core.Condition'
--自身血量
--判断自身的血量百分比是否在指定范围内，如果是返回true，否则返回false
--hp percent | float
local checkSelfHpPercent = b3.Class("CheckSelfHpPercent", b3.Condition)
b3.CheckSelfHpPercent = checkSelfHpPercent

function checkSelfHpPercent:ctor(settings)
	b3.Condition.ctor(self, settings)
	self.name = "CheckSelfHpPercent"
	self.title = "自身的血量百分比是否在指定范围内"
	self.properties = {hpPercent = "0.5"}
	settings = settings or {}
end

--[[function checkSelfHpPercent:open(tick)
	local targetInfo = tick.blackboard:get('LookTargetInfo')
	self.targetLua = targetInfo.targetLua;
end--]]

function checkSelfHpPercent:tick(tick)
	if( (not IsNull(tick.targetLua)) and (not IsNull(tick.targetLua.curHpPer)) )then
		local curHpPer = tick.targetLua.curHpPer;
		if (curHpPer < self.properties.hpPercent)then
			return b3.SUCCESS;
		end
	end
	
	return b3.FAILURE
end

require 'behavior3_lua.core.Condition'
--目标状态
--判断目标单位是否有指定ID的buff，如果有返回true，否则返回false
--buff ID |  int
local checkTargetHasBuff = b3.Class("CheckTargetHasBuff", b3.Condition)
b3.CheckTargetHasBuff = checkTargetHasBuff

function checkTargetHasBuff:ctor(settings)
	b3.Condition.ctor(self, settings)
	self.name = "CheckTargetHasBuff"
	self.title = "判断目标单位是否有指定ID的buff"
	self.properties = {buffid = "1001"}
	settings = settings or {}
end

function checkTargetHasBuff:open(tick)
	local targetInfo = tick.blackboard:get('LookTargetInfo')
	self.targetLua = targetInfo.targetLua;
end

function checkTargetHasBuff:tick(tick)
	if( (not IsNull(self.targetLua)) and (not IsNull(self.targetLua.buffInfos)) )then
		for k,v in pairs(self.targetLua.buffInfos)do
			if(k == self.properties.buffid)then
				return b3.SUCCESS;
			end
		end
	end
	
	return b3.FAILURE
end

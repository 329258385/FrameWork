require 'behavior3_lua.core.Condition'
--自身状态
--判断自身是否有指定ID的buff，如果有返回true，否则返回false
--buff ID |  int
local checkSelfHasBuff = b3.Class("CheckSelfHasBuff", b3.Condition)
b3.CheckSelfHasBuff = checkSelfHasBuff

function checkSelfHasBuff:ctor(settings)
	b3.Condition.ctor(self, settings)
	self.name = "CheckSelfHasBuff"
	self.title = "判断自身是否有指定ID的buff"
	self.properties = {buffid = "1001"}
	settings = settings or {}
end

--[[function checkSelfHasBuff:open(tick)
	local targetInfo = tick.blackboard:get('LookTargetInfo')
	self.targetLua = targetInfo.targetLua;
end--]]

function checkSelfHasBuff:tick(tick)
	if( (not IsNull(tick.targetLua)) and (not IsNull(tick.targetLua.buffInfos)) )then
		for k,v in pairs(tick.targetLua.buffInfos)do
			if(k == self.properties.buffid)then
				return b3.SUCCESS;
			end
		end
	end
	
	return b3.FAILURE
end

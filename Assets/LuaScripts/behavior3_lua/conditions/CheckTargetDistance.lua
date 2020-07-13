require 'behavior3_lua.core.Condition'
--目标距离
--判断与目标的距离是否在指定范围内，如果是返回true，否则返回false
--distance |  float
local checkTargetDistance = b3.Class("CheckTargetDistance", b3.Condition)
b3.CheckTargetDistance = checkTargetDistance

function checkTargetDistance:ctor(settings)
	b3.Condition.ctor(self, settings)
	self.name = "CheckTargetDistance"
	self.title = "目标距离是否在指定范围内"
	self.properties = {distance = "7"}
	settings = settings or {}
	self.targetTf = null
	self.sqrDistance = self.properties.distance *self.properties.distance;
end

function checkTargetDistance:open(tick)
	local targetInfo = tick.blackboard:get('LookTargetInfo')
	self.targetTf = targetInfo.targetTf;
	self.sqrDistance = self.properties.distance *self.properties.distance;
end

function checkTargetDistance:tick(tick)
	if( not IsNull(self.targetTf) )then
		local targetPos = self.targetTf.position;
		local direction = targetPos - tick.target.transform.position;
		local sqrPos = Vector3.SqrMagnitude(direction);
		if (sqrPos < self.sqrDistance)then
			print("checkTargetDistance true")
			return b3.SUCCESS;
		end
	end
	print("checkTargetDistance false")
	return b3.FAILURE
end

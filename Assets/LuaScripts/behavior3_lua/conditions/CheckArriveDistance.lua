require 'behavior3_lua.core.Condition'
--是否已经到达指定位置 如果targetTf存在，目标点为targetTf所在位置 如果不存在，目标点为配置中的坐标位置(targetPosX,targetPosY,targetPosZ)
local checkArriveDistance = b3.Class("CheckArriveDistance", b3.Condition)
b3.CheckArriveDistance = checkArriveDistance

function checkArriveDistance:ctor(settings)
	b3.Condition.ctor(self, settings)

	self.name = "CheckArriveDistance"
	self.title = "检测是否到达目标位置"
	self.properties = {arriveDistance ="0.5", targetPosX = "0", targetPosY = "0", targetPosZ = "0"}
	self.targetTf = nil
	self.targetPos = Vector3.Zero
	settings = settings or {}
end

function checkArriveDistance:open(tick)
	local targetInfo = tick.blackboard:get('LookTargetInfo')
	self.targetTf = targetInfo.targetTf;
	self.configPos = Vector3.New(tonumber(targetPosX),tonumber(targetPosY),tonumber(targetPosZ));--配置中的位置
end

function checkArriveDistance:tick(tick)
	if( IsNull(self.targetTf) )then
		self.targetPos = self.configPos;
	else
		self.targetPos = self.targetTf.position;
	end
	local direction = self.targetPos - tick.target.transform.position;
	local sqrPos = Vector3.SqrMagnitude(direction);
	if (sqrPos < self.properties.arriveDistance)then
		--print("checkArriveDistance:tick  到达目标位置 ")
		return b3.SUCCESS;
	end
	return b3.FAILURE
end

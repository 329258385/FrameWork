require 'behavior3_lua.core.Condition'
--判断自身与某个点的距离是否在指定范围内，如果是返回true，否则返回false
--目标点为配置中的坐标位置(targetPosX,targetPosY,targetPosZ)
--pos(x,y,z) | float , distance | float
local checkSelfPosDistance = b3.Class("CheckSelfPosDistance", b3.Condition)
b3.CheckSelfPosDistance = checkSelfPosDistance

function checkSelfPosDistance:ctor(settings)
	b3.Condition.ctor(self, settings)

	self.name = "CheckSelfPosDistance"
	self.title = "判断自身与某个点的距离是否在指定范围内"
	self.properties = {distance ="1", targetPos = "0,0,0"}
	self.targetTf = nil
	self.targetPos = Vector3.Zero
	settings = settings or {}
	local tmpPos = string.split(settings.targetPos,",");
	self.targetPos = Vector3.New(tonumber(tmpPos[1]),tonumber(tmpPos[2]),tonumber(tmpPos[3]));
	self.sqrDistance = settings.distance * settings.distance;
end

--[[function checkSelfPosDistance:open(tick)
	local tmpPos = string.split(self.properties.targetPos,",");
	self.targetPos = Vector3.New(tonumber(tmpPos[1]),tonumber(tmpPos[2]),tonumber(tmpPos[3]));
	self.sqrDistance = settings.distance * settings.distance;
end--]]

function checkSelfPosDistance:tick(tick)
	local direction = self.targetPos - tick.target.transform.position;
	local sqrPos = Vector3.SqrMagnitude(direction);
	if (sqrPos < self.sqrDistance)then
		return b3.SUCCESS;
	end
	return b3.FAILURE
end

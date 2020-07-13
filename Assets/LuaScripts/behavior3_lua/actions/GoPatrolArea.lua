require 'behavior3_lua.core.Action'
--回到巡逻区域 强制返回success
--gopatrolType 1对象的出生点 2上次巡逻点
--speed回去的速度
--angularSpeed 转角度的速度
--arriveDistance 自身和目标点的距离小于此值认为到达
--targetpos 取不到上次巡逻点的情况下使用此配置目标点如： "120,2,100" 
local goPatrolArea = b3.Class("GoPatrolArea", b3.Action)
b3.GoPatrolArea = goPatrolArea

local GoPatrolAreaType= {
    BirthPos = 1 ,    		---出生点
    LastPatrolPos = 2   		---上次的巡逻点

}

function goPatrolArea:ctor(settings)
	b3.Action.ctor(self, settings)
	self.name = "GoPatrolArea"
	self.title = "回到巡逻区域"
	self.properties = {gopatrolType = "1", speed = "0",angularSpeed = "0",arriveDistance = "", targetpos = ""}
	settings = settings or {}
	self.targetPos = settings.targetpos;
	self.pathPending = false --是否正在赶往目标点
end

function goPatrolArea:open(tick)
	if(self.properties.gopatrolType == GoPatrolAreaType.BirthPos)then
		--目标点是出生点
		self.targetPos = tick.targetLua.birthPos;
	else
		--目标点是上次巡逻点
		local tmpPos = tick.blackboard:get("lastPatrolPos")
		if(tmpPos == nil)then
			--没有取到上次巡逻点 就用配置中的坐标点
			self.targetPos = self.properties.targetpos;
		else
			self.targetPos = tmpPos;
		end
	end
end

function goPatrolArea:tick(tick)
	local direction = self.targetPos - tick.target.transform.position;
	
	direction.y = 0;
	print("goPatrolArea targetPos x="..self.targetPos.x.." y="..self.targetPos.y.." z="..self.targetPos.z)
	print("goPatrolArea tick.target.transform.position x="..tick.target.transform.position.x.." y="..tick.target.transform.position.y.." z="..tick.target.transform.position.z)
	local sqrPos = Vector3.SqrMagnitude(direction);
	print("goPatrolArea sqrPos ="..sqrPos);
	self.targetRotation = Quaternion.LookRotation(direction, Vector3.up)
	if not IsNull(self.targetRotation) then
		tick.target.transform.rotation = self.targetRotation;--Quaternion.Slerp(tick.target.transform.rotation , self.targetRotation, self.properties.angularSpeed)
	end
	if (sqrPos < self.properties.arriveDistance)then
		tick.target.transform.rotation = self.targetRotation;
	else			
		tick.target.transform:Translate(Vector3.forward * Time.deltaTime * self.properties.speed)
	end
	return b3.SUCCESS
end

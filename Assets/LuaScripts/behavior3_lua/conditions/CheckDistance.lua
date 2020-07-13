require 'behavior3_lua.core.Condition'
--找到所有Tag为targetTag的目标 然后从这些目标中找到和自身距离小于distance的一个目标 
--找到了返回b3.SUCCESS
--所有目标和自身的距离都超过distance则返回b3.FAILURE
local checkDistance = b3.Class("CheckDistance", b3.Condition)
b3.CheckDistance = checkDistance

function checkDistance:ctor(settings)
	b3.Condition.ctor(self, settings)
	self.name = "CheckDistance"
	self.title = "检查距离"
	self.properties = {distance = "7", targetTag = ""}
	self.targetTranforms = nil
	settings = settings or {}
	
	self.sqrMagnitude =  0 --distance * distance 为了优化 不需要取平方根了
end
function checkDistance:open(tick)
	tick.blackboard:set('LookTargetInfo',nil)
	self.sqrMagnitude = self.properties.distance * self.properties.distance;
	if(self.properties.targetTag ~= nil and self.properties.targetTag ~= "")then
		local gameObjects = CS.UnityEngine.GameObject.FindGameObjectsWithTag(self.properties.targetTag);
		if((not IsNull(gameObjects)) and gameObjects.Length > 0)then
			self.targetTranforms = {}
			for i =0, gameObjects.Length-1 do
				table.insert(self.targetTranforms,gameObjects[i].transform);
			end
		end
	end
end

function checkDistance:tick(tick)
	if(self.targetTranforms ~= nil and #self.targetTranforms > 0)then
		for i =1, #self.targetTranforms do
			local curTargetTf = self.targetTranforms[i]
			local direction = curTargetTf.position - tick.target.transform.position;
			local sqrPos = Vector3.SqrMagnitude(direction);
			if (sqrPos < self.sqrMagnitude)then
				local targetinfo = {targetTf = curTargetTf, targetLua = nil}
				tick.blackboard:set('LookTargetInfo',targetinfo)
				tick.blackboard:set('FindTargetDistanceValue',sqrPos)
				return b3.SUCCESS;
			end
		end
	end
	return b3.FAILURE;
end

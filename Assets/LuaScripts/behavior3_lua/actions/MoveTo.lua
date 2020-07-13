require 'behavior3_lua.core.Action'
--移动到指定位置 如果targetTf存在就移动到targetTf所在位置 如果不存在就移动到配置中的坐标位置(targetPosX,targetPosY,targetPosZ)
--强制返回b3.SUCCESS
local moveTo = b3.Class("MoveTo", b3.Action)
b3.MoveTo = moveTo

function moveTo:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "MoveTo"
	self.title = "到指定的目标位置"
	self.properties = {speed = "0",angularSpeed = "0", arriveDistance ="0.5", targetPosX = "0", targetPosY = "0", targetPosZ = "0"}
	self.targetTf = nil
	self.targetPos = Vector3.Zero
	settings = settings or {}
end

function moveTo:open(tick)
	local targetInfo = tick.blackboard:get('LookTargetInfo')
	self.targetTf = targetInfo.targetTf;
	self.configPos = Vector3.New(tonumber(targetPosX),tonumber(targetPosY),tonumber(targetPosZ));--配置中的位置
	self.curMoveSpeed = 0
end

function moveTo:tick(tick)
	--[[if(self.animator == nil)then
		return b3.FAILURE
	end--]]

	if( IsNull(self.targetTf) )then
		self.targetPos = self.configPos;
	else
		self.targetPos = self.targetTf.position;
	end
	local forwVector = self.targetPos - tick.target.transform.position;
	forwVector.y = 0;
	self.targetRotation = Quaternion.LookRotation(forwVector, Vector3.up)
	print("moveTo self.targetRotation.x ="..self.targetRotation.x.." self.targetRotation.y="..self.targetRotation.y.." self.targetRotation.z="..self.targetRotation.z);
	local rota = tick.target.transform.rotation
	if not IsNull(self.targetRotation) then
		tick.target.transform.rotation = Quaternion.Slerp(tick.target.transform.rotation , self.targetRotation, self.properties.angularSpeed)
	end

	--距离判断单独写到CheckArriveDistance脚本内了 可以两个配合使用
	local direction = self.targetPos - tick.target.transform.position;
	local sqrPos = Vector3.SqrMagnitude(direction);
	
	if (sqrPos < self.properties.arriveDistance * self.properties.arriveDistance )then
		self.curMoveSpeed  = 0
		tick.blackboard:set("SeekMoveSpeed",self.curMoveSpeed);
		tick.target.transform.rotation = self.targetRotation
		--self.animator:SetFloat("MoveSpeed", self.curMoveSpeed);
	else	
		self.curMoveSpeed = self.properties.speed--Mathf.Lerp(self.curMoveSpeed,self.properties.speed,Time.deltaTime * 3)	
		tick.blackboard:set("SeekMoveSpeed",self.curMoveSpeed/self.properties.speed);
		tick.target.transform:Translate(Vector3.forward * Time.deltaTime * self.curMoveSpeed)
	end
	return b3.SUCCESS
end

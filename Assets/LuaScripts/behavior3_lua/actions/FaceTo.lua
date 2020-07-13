require 'behavior3_lua.core.Action'
--执行朝向逻辑，执行完后返回，不管有没被成功执行返回值始终为true
--如果有锁定的目标就朝向目标
--如果没有锁定的目标就朝向设置的坐标点
local faceTo = b3.Class("FaceTo", b3.Action)
b3.FaceTo = faceTo

function faceTo:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "FaceTo"
	self.title = "朝向"
	self.properties = {angularSpeed = "0", targetPosX = "0", targetPosY = "0", targetPosZ = "0"}
	self.targetTf = nil
	self.targetPos = Vector3.Zero
	settings = settings or {}
	
end

function faceTo:open(tick)
	local targetInfo = tick.blackboard:get('LookTargetInfo')
	self.targetTf = targetInfo.targetTf;
	self.configPos = Vector3.New(tonumber(targetPosX),tonumber(targetPosY),tonumber(targetPosZ));--配置中的位置
end

function faceTo:tick(tick)
	if( IsNull(self.targetTf) )then
		self.targetPos = self.configPos;
	else
		self.targetPos = self.targetTf.position;
	end
	local forwVector = self.targetPos - tick.target.transform.position;
	forwVector.y = 0;
	self.targetRotation = Quaternion.LookRotation(forwVector, Vector3.up)
	local rota = tick.target.transform.rotation
	if not IsNull(self.targetRotation) then
		tick.target.transform.rotation = Quaternion.Slerp(tick.target.transform.rotation , self.targetRotation, self.properties.angularSpeed)
	end
	return b3.SUCCESS
end

require 'behavior3_lua.core.Condition'
--检测目标是否在视野范围
--有时不仅需要判断距离 还要有视野概念 距离够了但是不在视野内也是不符合条件的
--这里的目标是取的共享数据里的LookTargetInfo为目标 如果没有则直接返回b3.FAILURE
--如果有目标并且在视野内 返回b3.SUCCESS
local checkView = b3.Class("CheckView", b3.Condition)
b3.CheckView = checkView

function checkView:ctor(settings)
	b3.Condition.ctor(self, settings)
	self.name = "CheckView"
	self.title = "检测视野范围"
	self.properties = {angle = "90"}
	self.targetTf = nil
	settings = settings or {}
end

function checkView:open(tick)
	--print("checkView:open")
	local targetInfo = tick.blackboard:get('LookTargetInfo')
	self.targetTf = targetInfo.targetTf;
	
end

function checkView:tick(tick)
	if( not IsNull(self.targetTf))then
		local tempAngle = Vector3.Angle(tick.target.transform.forward, (self.targetTf.position - tick.target.transform.position));
        if (tempAngle < (self.properties.angle/2)) then--目标和自身正前方的夹角小于视野范围的一半就认为在视野范围内
			return b3.SUCCESS;
		end
	end
	return b3.FAILURE;
end

require 'behavior3_lua.core.Action'
--根据外部数据动态设置动画器上的float参数
--paramName 动画器上要设置的float参数的名字 如：MoveSpeed
--paramSource 参数动态取值在全局中的变量名称
local setDynamicFloatParameter = b3.Class("SetDynamicFloatParameter", b3.Action)
b3.SetDynamicFloatParameter = setDynamicFloatParameter

function setDynamicFloatParameter:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "SetDynamicFloatParameter"
	self.title = "动态设置动画器上的float参数"
	self.properties = {paramName = "",paramSource = ""}
	settings = settings or {}
	
	self.targetGameObject = nil
	self.prevGameObject = nil
	self.animator = nil
	
end

function setDynamicFloatParameter:open(tick)
	self.curFloatParamerer = nil;
	if( self.properties.paramSource ~= "" and self.properties.paramSource ~= nil )then
		self.curFloatParamerer = tick.blackboard:get(self.properties.paramSource)
	end
	self.targetGameObject = tick.target;
	if(self.targetGameObject ~= nil and self.targetGameObject ~= self.prevGameObject) then
		self.animator = self.targetGameObject:GetComponent(typeof(CS.UnityEngine.Animator));
		self.prevGameObject = self.targetGameObject;
	end
end

function setDynamicFloatParameter:tick(tick)
	if(self.animator == nil or self.curFloatParamerer == nil)then
		return b3.FAILURE
	end	
	self.animator:SetFloat(self.properties.paramName, self.curFloatParamerer);
	return b3.SUCCESS
end

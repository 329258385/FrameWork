require 'behavior3_lua.core.Action'
----设置动画器上的float参数
local setFloatParameter = b3.Class("SetFloatParameter", b3.Action)
b3.SetFloatParameter = setFloatParameter

function setFloatParameter:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "SetFloatParameter"
	self.title = "设置动画器上的float参数"
	self.properties = {paramaterName = "", floatValue = "0", setOnce = false}
	self.targetGameObject = nil
	self.prevGameObject = nil
	self.animator = nil
	settings = settings or {}
end

function setFloatParameter:open(tick)
	self.targetGameObject = tick.target;
	if(self.targetGameObject ~= nil and self.targetGameObject ~= self.prevGameObject) then
		self.animator = self.targetGameObject:GetComponent(typeof(CS.UnityEngine.Animator));
		self.prevGameObject = self.targetGameObject;
	end
end

function setFloatParameter:tick(tick)
	
	if(self.animator == nil)then
		return b3.FAILURE
	end
	local hashID = CS.UnityEngine.Animator.StringToHash(self.properties.paramaterName);
	local prevValue = self.animator:GetFloat(hashID);
	self.animator:SetFloat(hashID, self.properties.floatValue);
	if(self.properties.setOnce == "true") then
		coroutine.start(function()
			coroutine.waitforframes(1)
			self.animator:SetFloat(hashID, prevValue);
		end)
	end
	return b3.SUCCESS
end

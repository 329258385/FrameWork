require 'behavior3_lua.core.Action'
----设置动画器上的int参数
local setIntegerParameter = b3.Class("SetIntegerParameter", b3.Action)
b3.SetIntegerParameter = setIntegerParameter

function setIntegerParameter:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "SetIntegerParameter"
	self.title = "设置动画器上的int参数"
	self.properties = {paramaterName = "", intValue = "0", setOnce = false}
	self.targetGameObject = nil
	self.prevGameObject = nil
	self.animator = nil
	settings = settings or {}
end

function setIntegerParameter:open(tick)
	self.targetGameObject = tick.target;
	if(self.targetGameObject ~= nil and self.targetGameObject ~= self.prevGameObject) then
		self.animator = self.targetGameObject:GetComponent(typeof(CS.UnityEngine.Animator));
		self.prevGameObject = self.targetGameObject;
	end
end

function setIntegerParameter:tick(tick)
	
	if(self.animator == nil)then
		return b3.FAILURE
	end
	local hashID = CS.UnityEngine.Animator.StringToHash(self.properties.paramaterName);
	local prevValue = self.animator:GetInteger(hashID);
	self.animator:SetInteger(hashID, self.properties.intValue);
	if(self.properties.setOnce == "true") then
		coroutine.start(function()
			coroutine.waitforframes(1)
			self.animator:SetInteger(hashID, prevValue);
		end)
	end
	return b3.SUCCESS
end

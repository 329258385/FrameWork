require 'behavior3_lua.core.Action'
--设置动画器上的bool参数
local setBoolParameter = b3.Class("SetBoolParameter", b3.Action)
b3.SetBoolParameter = setBoolParameter

function setBoolParameter:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "SetBoolParameter"
	self.title = "设置动画器上的bool参数"
	self.properties = {paramaterName = "", boolValue = false, setOnce = false}
	self.targetGameObject = nil
	self.prevGameObject = nil
	self.animator = nil
	settings = settings or {}
end

function setBoolParameter:open(tick)
	self.targetGameObject = tick.target;
	if(self.targetGameObject ~= nil and self.targetGameObject ~= self.prevGameObject) then
		self.animator = self.targetGameObject:GetComponent(typeof(CS.UnityEngine.Animator));
		self.prevGameObject = self.targetGameObject;
	end
end

function setBoolParameter:tick(tick)
	
	if(self.animator == nil)then
		return b3.FAILURE
	end
	local hashID = CS.UnityEngine.Animator.StringToHash(self.properties.paramaterName);
	local prevValue = self.animator:GetBool(hashID);
	self.animator:SetBool(hashID, self.properties.boolValue);
	if(self.properties.setOnce == "true") then
		coroutine.start(function()
			coroutine.waitforframes(1)
			self.animator:SetBool(hashID, prevValue);
		end)
	end
	return b3.SUCCESS
end

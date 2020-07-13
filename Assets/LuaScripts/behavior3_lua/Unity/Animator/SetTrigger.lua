require 'behavior3_lua.core.Action'
----触发动画器上的触发器
local setTrigger = b3.Class("SetTrigger", b3.Action)
b3.SetTrigger = setTrigger

function setTrigger:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "SetTrigger"
	self.title = "触发动画器上的触发器"
	self.properties = {paramaterName = ""}
	self.targetGameObject = nil
	self.prevGameObject = nil
	self.animator = nil
	settings = settings or {}
end

function setTrigger:open(tick)
	self.targetGameObject = tick.target;
	if(self.targetGameObject ~= nil and self.targetGameObject ~= self.prevGameObject) then
		self.animator = self.targetGameObject:GetComponent(typeof(CS.UnityEngine.Animator));
		self.prevGameObject = self.targetGameObject;
	end
end

function setTrigger:tick(tick)
	
	if(self.animator == nil)then
		return b3.FAILURE
	end
	self.animator:SetTrigger(self.properties.paramaterName);
	return b3.SUCCESS
end

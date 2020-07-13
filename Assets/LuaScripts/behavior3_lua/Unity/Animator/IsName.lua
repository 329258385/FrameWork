require 'behavior3_lua.core.Action'
----动作正在播放，则返回成功
--layerIndex 动作所在的层索引
--actionName 动作名
local isName = b3.Class("IsName", b3.Action)
b3.IsName = isName

function isName:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "IsName"
	self.title = "判断动作是否正在播放"
	self.properties = {layerIndex = "", actionName = ""}
	self.targetGameObject = nil
	self.prevGameObject = nil
	self.animator = nil
	settings = settings or {}
end

function isName:open(tick)
	self.targetGameObject = tick.target;
	if(self.targetGameObject ~= nil and self.targetGameObject ~= self.prevGameObject) then
		self.animator = self.targetGameObject:GetComponent(typeof(CS.UnityEngine.Animator));
		self.prevGameObject = self.targetGameObject;
	end
end

function isName:tick(tick)
	if(self.animator == nil)then
		return b3.FAILURE
	end
	self.animator:SetTrigger(self.properties.paramaterName);
	local result = self.animator:GetCurrentAnimatorStateInfo(self.properties.layerIndex):IsName(self.properties.actionName);
	if(result == false)then
		return b3.FAILURE
	end
	return b3.SUCCESS
end

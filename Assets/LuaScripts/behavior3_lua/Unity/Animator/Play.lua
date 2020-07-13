require 'behavior3_lua.core.Action'
--播放Animator动画
local play = b3.Class("Play", b3.Action)
b3.Play = play

function play:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "Play"
	self.title = "播放Animator动画"
	self.properties = {stateName = "",stateLayer = "-1", normalizedTime =""}
	self.targetGameObject = nil
	self.prevGameObject = nil
	self.animator = nil
	settings = settings or {}
end

function play:open(tick)
	self.targetGameObject = tick.target;
	if(self.targetGameObject ~= nil and self.targetGameObject ~= self.prevGameObject) then
		self.animator = self.targetGameObject:GetComponent(typeof(CS.UnityEngine.Animator));
		self.prevGameObject = self.targetGameObject;
	end
end

function play:tick(tick)
	if(self.animator == nil)then
		return b3.FAILURE
	end
	self.animator:Play(self.properties.stateName, self.properties.stateLayer, self.properties.normalizedTime)
	return b3.SUCCESS
end

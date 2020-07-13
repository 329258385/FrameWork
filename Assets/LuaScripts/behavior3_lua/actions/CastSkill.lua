require 'behavior3_lua.core.Action'
----释放技能 先判断是否可以释放技能 可以就播放相应的动作
local castSkill = b3.Class("CastSkill", b3.Action)
b3.CastSkill = castSkill

function castSkill:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "CastSkill"
	self.title = "释放技能"
	self.properties = {paramaterName = "", intValue = "0", skillid = "1001"}
	self.targetGameObject = nil
	self.prevGameObject = nil
	self.animator = nil
	settings = settings or {}
end

function castSkill:open(tick)
	self.targetGameObject = tick.target;
	if(self.targetGameObject ~= nil and self.targetGameObject ~= self.prevGameObject) then
		self.animator = self.targetGameObject:GetComponent(typeof(CS.UnityEngine.Animator));
		self.prevGameObject = self.targetGameObject;
	end
end

function castSkill:tick(tick)
	local result = tick.targetLua:PlaySkillAttack(tonumber(self.properties.skillid));--true 为可以释放技能 false为不能释放技能
	if(result == true)then		
		print("castSkill true");
		if(self.animator ~= nil)then			
			local hashID = CS.UnityEngine.Animator.StringToHash(self.properties.paramaterName);
			--local prevValue = self.animator:GetInteger(hashID);
			self.animator:SetInteger(hashID, self.properties.intValue);
			self.animator:SetTrigger("TriggerAttack");
			print("castSkill TriggerAttack "..self.properties.intValue);
		end
		return b3.SUCCESS
	end
	print("castSkill false");
	return b3.FAILURE
end

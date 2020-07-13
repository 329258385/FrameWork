require 'behavior3_lua.core.Condition'
--检测与目标之间是否有障碍物 没有障碍物返回b3.SUCCESS 有障碍物返回b3.FAILURE
local checkTag = b3.Class("CheckTag", b3.Condition)
b3.CheckTag = checkTag

function checkTag:ctor(settings)
	b3.Condition.ctor(self, settings)
	self.name = "CheckTag"
	self.title = "检查与目标间是否有障碍物"
	self.properties = {targetTag = "Player"}
	self.targetTf = nil
	self.bRayHitPlayer = false --是否检测到目标
	settings = settings or {}
end

function checkTag:open(tick)
	print("checkTag:open")
	local targetInfo = tick.blackboard:get('LookTargetInfo')
	self.targetTf = targetInfo.targetTf;
end

function checkTag:tick(tick)
	if(self.targetTf ~= nil)then
		local ray = Ray.New(tick.target.transform.position,tick.target.transform.forward); 
		local _layer = 2 ^ LayerMask.NameToLayer('Player');
		local flagResult,hit = UnityEngine.Physics.Raycast(ray, nil, 20, _layer);
		
		if (flagResult) then
            if (hit.collider.tag == tostring(self.properties.targetTag))then --射线检测到目标
				self.bRayHitPlayer = true;
                return b3.SUCCESS;
            else
				--检测到的不是目标
				self.bRayHitPlayer = false;
                return b3.FAILURE;
			end
        
        else
			--射线什么也没检测到那么也默认为检测到目标，因为在这个节点里面，此时目标肯定是在视野范围内的并且没有被障碍物遮挡视野
            self.bRayHitPlayer = true;
			return b3.SUCCESS;
		end
	end
	return b3.FAILURE;
end

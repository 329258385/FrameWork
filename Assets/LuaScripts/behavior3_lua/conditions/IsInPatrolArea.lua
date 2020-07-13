require 'behavior3_lua.core.Condition'
--巡逻
--判断是否在巡逻区域，是返回true，否则返回false
--读怪物表配置或者关卡配置
local isInPatrolArea = b3.Class("IsInPatrolArea", b3.Condition)
b3.IsInPatrolArea = isInPatrolArea

function isInPatrolArea:ctor(settings)
	b3.Condition.ctor(self, settings)
	self.name = "IsInPatrolArea"
	self.title = "是否在巡逻区域"
	self.properties = {}
	settings = settings or {}
end

function isInPatrolArea:open(tick)
end

function isInPatrolArea:tick(tick)
	if(tick.targetLua  ~= nil and tick.targetLua.birthPos ~= nil and tick.targetLua.patrolRadius ~= nil)then
		--目前是有一个出生点和一个半径组成的圆形为巡逻区域
		local pos = tick.targetLua.birthPos;
		pos.y = 0;
		local radius = tick.targetLua.patrolRadius;
		local curPos = tick.target.transform.position;
		curPos.y = 0;
		local direction = pos - curPos;
		local sqrPos = Vector3.SqrMagnitude(direction);
		--print("IsInPatrolArea sqrPos ="..sqrPos);
		if (sqrPos < radius*radius)then
			return b3.SUCCESS;
		end
	end
	return b3.FAILURE;
end

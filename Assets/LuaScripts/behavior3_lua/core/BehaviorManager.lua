require 'behavior3_lua.core.Tick'
local json = require 'behavior3_lua.json'

local BehaviorManager = BaseClass("BehaviorManager", UpdatableSingleton)
local base = UpdatableSingleton
local behaviorTreesData = {}

--behaviordata = {behavior,blackBoard,target}
--behavior 		行为树AI
--blackBoard	黑板用于传递信息
--target 		AI的目标gameobject
local function EnableBehavior(behaviordata)
	if(behaviordata == nil or behaviordata.blackBoard == nil or behaviordata.behavior == nil)then
		return;
	end
	behaviordata.behavior.executionCount = 0
	table.insert(behaviorTreesData, behaviordata);
end

local function DisableBehavior(behaviordata)
	for k, v in pairs(behaviorTreesData) do
		if v == behaviordata then
			table.remove(behaviorTreesData, k)
		end
	end
end

local function Tick()
	for k, v in pairs(behaviorTreesData) do
		Tick(v);
	end
end

local function Tick(behaviordata)
	if(behaviordata == nil or behaviordata.blackBoard == nil)then
		return;
	end
	local status = behaviordata.behavior.status;
	--执行行为树
	if(behaviordata.behavior.executionCount == 0)then
		status = behaviordata.behavior:tick(behaviordata.target, behaviordata.blackBoard)
		
		--如果行为树返回状态为非Running 则此树的生命周期结束
		if(status ~= b3.RUNNING)then		
			behaviordata.behavior.executionCount = behaviordata.behavior.executionCount +1;
			if(#behaviordata.behavior.foreverNodes <= 0)then
				DisableBehavior(behaviordata);
			end
		end
	elseif(#behaviordata.behavior.foreverNodes > 0)then
		--这里应该只执行forever结点下的逻辑
		for k, v in pairs(behaviordata.behavior.foreverNodes) do
			local tick1 = b3.Tick.new()
			tick1.debug 		= behaviordata.behavior.debug
			tick1.target		= behaviordata.target
			tick1.blackboard = behaviordata.blackBoard
			tick1.tree 		= behaviordata.behavior
			status = v:_execute(tick1)
		end
	end
end

local function Update(self)
	local delta_time = Time.deltaTime
	Tick();
end

local function LateUpdate(self)
end

local function FixedUpdate(self)
end

local function Dispose(self)
end

BehaviorManager.Update = Update
BehaviorManager.LateUpdate = LateUpdate
BehaviorManager.FixedUpdate = FixedUpdate
BehaviorManager.Dispose = Dispose
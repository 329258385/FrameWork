require 'behavior3_lua.core.Tick'
local json = require 'behavior3_lua.json'

local BehaviorManager = BaseClass("BehaviorManager", UpdatableSingleton)
local base = UpdatableSingleton
local behaviorTreesData = {}

--behaviordata = {behavior,blackBoard,target}
--behavior 		��Ϊ��AI
--blackBoard	�ڰ����ڴ�����Ϣ
--target 		AI��Ŀ��gameobject
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
	--ִ����Ϊ��
	if(behaviordata.behavior.executionCount == 0)then
		status = behaviordata.behavior:tick(behaviordata.target, behaviordata.blackBoard)
		
		--�����Ϊ������״̬Ϊ��Running ��������������ڽ���
		if(status ~= b3.RUNNING)then		
			behaviordata.behavior.executionCount = behaviordata.behavior.executionCount +1;
			if(#behaviordata.behavior.foreverNodes <= 0)then
				DisableBehavior(behaviordata);
			end
		end
	elseif(#behaviordata.behavior.foreverNodes > 0)then
		--����Ӧ��ִֻ��forever����µ��߼�
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
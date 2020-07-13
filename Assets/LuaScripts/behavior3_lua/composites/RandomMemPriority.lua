require 'behavior3_lua.core.Composite'
--逻辑和MemPriority一样，但是这里会打乱子任务的顺序 不是按照从左到右从上到下的排列顺序执行了
local randomMemPriority = b3.Class("RandomMemPriority", b3.Composite)
b3.RandomMemPriority = randomMemPriority

function randomMemPriority:ctor()
	b3.Composite.ctor(self)
	self.children = {}
	self.childrenCount = 0
	self.shuffleChilden = {}
	self.name = "RandomMemPriority"
end

function randomMemPriority:open(tick)
	tick.blackboard:set("runningChild", 0, tick.tree.id, self.id)
	self.childrenCount = #self.children
	local randomArr = DeepCopy(self.children)
	self.shuffleChilden = RandomTable(randomArr)
	--print("randomMemPriority:open 第一个是"..tostring(self.shuffleChilden[1].name.."  "..tostring(self.shuffleChilden[1].properties.logStr)));
end

function randomMemPriority:tick(tick)	
	local currentChildIndex = tick.blackboard:get("runningChild", tick.tree.id, self.id)
	if(currentChildIndex == 0 or currentChildIndex > self.childrenCount)then
		currentChildIndex = 1;
	end
	for i = currentChildIndex, self.childrenCount do
		local v = self.shuffleChilden[i];
		local status = v:_execute(tick)

		if status ~= b3.FAILURE then
			if status == b3.RUNNING then
				tick.blackboard:set("runningChild", i, tick.tree.id, self.id)
			end
			
			return status
		end
	end
	return b3.FAILURE
	--[[local child = tick.blackboard:get("runningChild", tick.tree.id, self.id)
	for i,v in pairs(self.children) do
		local status = v:_execute(tick)

		if status ~= b3.FAILURE then
			if status == b3.RUNNING then
				tick.blackboard:set("runningChild", i, tick.tree.id, self.id)
			end
			
			return status
		end
	end

	return b3.FAILURE--]]
end
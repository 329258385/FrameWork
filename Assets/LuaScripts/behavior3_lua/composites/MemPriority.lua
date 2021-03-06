require 'behavior3_lua.core.Composite'
--逻辑和Priority一样但是这里涉及到一个b3.RUNNING的返回值，
--就是子任务并不知道执行结果是成功还是失败，我得晚一点才能告诉你，那如何处理这种情况咧？
--对于Priority任务，当它再次遍历子任务的时候，还是从第一个子任务开始遍历，
--但是MemPriority却会记住之前在Running的子任务,然后下次遍历的时候，
--直接从Running的那个子任务开始，这可以很方便的继续之前中断的逻辑决策。
local memPriority = b3.Class("MemPriority", b3.Composite)
b3.MemPriority = memPriority

function memPriority:ctor()
	b3.Composite.ctor(self)
	self.children = {}
	self.childrenCount = 0
	self.name = "MemPriority"
end

function memPriority:open(tick)
	tick.blackboard:set("runningChild", 0, tick.tree.id, self.id)
	self.childrenCount = #self.children
end

function memPriority:tick(tick)	
	local currentChildIndex = tick.blackboard:get("runningChild", tick.tree.id, self.id)
	if(currentChildIndex == 0 or currentChildIndex > self.childrenCount)then
		currentChildIndex = 1;
	end
	for i = currentChildIndex, self.childrenCount do
		local v = self.children[i];
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
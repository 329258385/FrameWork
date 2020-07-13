require 'behavior3_lua.core.Composite'
-- 顺序选择执行子任务，当子任务返回b3.FAILURE直接访问下一个子任务，
--直到所有子任务都返回b3.FAILURE,才从当前任务返回b3.FAILURE，
--当子任务返回b3.SUCCESS，则直接从当前任务返回b3.SUCCESS 类似于逻辑或的概念。
local priority = b3.Class("Priority", b3.Composite)
b3.Priority = priority

function priority:ctor()
	b3.Composite.ctor(self)

	self.name = "Priority"
end

function priority:tick(tick)
	for i,v in pairs(self.children) do
		local status = v:_execute(tick)

		if status ~= b3.FAILURE then
			return status
		end
	end

	return b3.FAILURE
end


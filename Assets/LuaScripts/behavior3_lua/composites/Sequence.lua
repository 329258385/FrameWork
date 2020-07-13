require 'behavior3_lua.core.Composite'
--顺序执行子任务 当子任务返回b3.SUCCESS直接访问下一个子任务，
--直到所有子任务都返回b3.SUCCESS,才从当前任务返回b3.SUCCESS，
--当子任务返回b3.FAILURE，则直接从当前任务返回b3.FAILURE 类似于逻辑与的概念。
local sequence = b3.Class("Sequence", b3.Composite)
b3.Sequence = sequence

function sequence:ctor()
	b3.Composite.ctor(self)

	self.name = "Sequence"
end

function sequence:tick(tick)
	for i = 1,#(self.children) do
		local v = self.children[i]
		local status = v:_execute(tick)
		--print(i,v)
		if status ~= b3.SUCCESS then
			return status
		end
	end
	return b3.SUCCESS
end

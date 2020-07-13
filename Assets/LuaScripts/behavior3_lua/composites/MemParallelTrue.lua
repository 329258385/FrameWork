require 'behavior3_lua.core.Composite'
--并行执行所有任务  有一个在running则返回running 否者强制返回成功
--下次直接执行所有running状态的任务，若没有则从头开始执行所有任务。
local memParallelTrue = b3.Class("MemParallelTrue", b3.Composite)
b3.MemParallelTrue = memParallelTrue

function memParallelTrue:ctor()
	b3.Composite.ctor(self)
	self.children = {}
	self.childrenCount = 0
	self.tChildStatus = {}
	self.tRunningChild = {}
	self.name = "MemParallelTrue"
end

function memParallelTrue:open(tick)
	self.tRunningChild = {}
	self.childrenCount = #self.children
end

function memParallelTrue:tick(tick)
	
	--如果有running状态的任务 则继续执行running状态的任务
	local curRuningChild = {}
	if(#self.tRunningChild > 0)then
		for i = 1, #self.tRunningChild do
			local vIndex = self.tRunningChild[i];
			local v = self.children[vIndex];
			local status = v:_execute(tick)
			self.tChildStatus[vIndex] = status;
			if(status == b3.RUNNING)then
				curRuningChild[#curRuningChild+1] = vIndex;
			end
		end
		self.tRunningChild = curRuningChild;
		if(#self.tRunningChild > 0)then
			return b3.RUNNING;
		end
		return b3.SUCCESS;
	end
	--如果没有running状态的任务则从头开始执行所有任务
	self.tRunningChild = {}
	for i = 1, self.childrenCount do
		local v = self.children[i];
		local status = v:_execute(tick)
		self.tChildStatus[i] = status;
		if(status == b3.RUNNING)then
			self.tRunningChild[#self.tRunningChild+1] = i;
		end
	end
	if(#self.tRunningChild > 0)then
		return b3.RUNNING;
	end
	return b3.SUCCESS;
end
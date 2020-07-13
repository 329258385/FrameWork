require 'behavior3_lua.core.Composite'
--并行执行所有任务 全部成功则返回成功 有一个失败则返回失败 有一个在running则返回running
--下次直接执行所有running状态的任务，若没有则从头开始执行所有任务。
local memParallel = b3.Class("MemParallel", b3.Composite)
b3.MemParallel = memParallel

function memParallel:ctor()
	b3.Composite.ctor(self)
	self.children = {}
	self.childrenCount = 0
	self.tChildStatus = {}
	self.tRunningChild = {}
	self.name = "MemParallel"
end

function memParallel:open(tick)
	self.tRunningChild = {}
	self.childrenCount = #self.children
end

function memParallel:tick(tick)
	
	local resultStatus = b3.SUCCESS
	local bFailerStatus = false
	--如果有running状态的任务 则继续执行running状态的任务
	local curRuningChild = {}
	if(#self.tRunningChild > 0)then
		for i = 1, #self.tRunningChild do
			local vIndex = self.tRunningChild[i];
			local v = self.children[vIndex];
			local status = v:_execute(tick)
			self.tChildStatus[vIndex] = status;
			if status ~= b3.SUCCESS then
				if(status == b3.FAILURE)then
					bFailerStatus = true
				elseif(status == b3.RUNNING)then
					curRuningChild[#curRuningChild+1] = vIndex;
				else					
					resultStatus = status--此时状态可能是ERROR
				end
			end
		end
		self.tRunningChild = curRuningChild;
		--并行任务有一个失败的则返回失败
		if(bFailerStatus)then
			return b3.FAILURE
		end
		if(#self.tRunningChild > 0)then
			return b3.RUNNING;
		end
		return resultStatus;
	end
	--如果没有running状态的任务则从头开始执行所有任务
	self.tRunningChild = {}
	for i = 1, self.childrenCount do
		local v = self.children[i];
		local status = v:_execute(tick)
		self.tChildStatus[i] = status;
		if status ~= b3.SUCCESS then
			if(status == b3.FAILURE)then
				bFailerStatus = true
			elseif(status == b3.RUNNING)then
				self.tRunningChild[#self.tRunningChild+1] = i;
			else				
				resultStatus = status--此时状态可能是ERROR
			end
		end
	end
	--并行任务有一个失败的则返回失败
	if(bFailerStatus)then
		return b3.FAILURE;
	end
	if(#self.tRunningChild > 0)then
		return b3.RUNNING;
	end
	return resultStatus;--这里返回的状态只可能是ERROR或者SUCCESS
end
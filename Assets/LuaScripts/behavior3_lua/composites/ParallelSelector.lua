require 'behavior3_lua.core.Composite'
--并行执行所有任务 有一个成功则返回成功 没有成功的有一个在running则返回running  全部失败则返回失败 
local parallelSelector = b3.Class("ParallelSelector", b3.Composite)
b3.ParallelSelector = parallelSelector

function parallelSelector:ctor()
	b3.Composite.ctor(self)
	self.children = {}
	self.childrenCount = 0
	self.tChildStatus = {}
	self.tRunningChild = {}
	self.name = "ParallelSelector"
end

function parallelSelector:open(tick)
	self.tRunningChild = {}
	self.childrenCount = #self.children
end

function parallelSelector:tick(tick)
	local resultStatus = b3.FAILURE
	local bSuccessStatus = false
	--如果有running状态的任务 则继续执行running状态的任务
	local curRuningChild = {}
	if(#self.tRunningChild > 0)then
		for i = 1, #self.tRunningChild do
			local vIndex = self.tRunningChild[i];
			local v = self.children[vIndex];
			local status = v:_execute(tick)
			self.tChildStatus[vIndex] = status;
			if status == b3.SUCCESS then
				bSuccessStatus = true
			elseif status == b3.RUNNING then
				curRuningChild[#curRuningChild+1] = vIndex;--记录下正在running的任务
				resultStatus = b3.RUNNING
			end
		end
		self.tRunningChild = {}
		self.tRunningChild = curRuningChild;
		--并行任务有一个失败的则返回失败
		if(bSuccessStatus)then
			return b3.SUCCESS
		end
		return resultStatus;
	end
	
	--从头开始执行所有任务
	for i = 1, self.childrenCount do
		local v = self.children[i];
		local status = v:_execute(tick)
		self.tChildStatus[i] = status;
		if status == b3.SUCCESS then
			bSuccessStatus = true
		elseif status == b3.RUNNING then
			self.tRunningChild[#self.tRunningChild+1] = i;--记录下正在running的任务
			resultStatus = b3.RUNNING
		end
		
	end
	--并行任务有一个成功的则返回成功
	if(bSuccessStatus)then
		return b3.SUCCESS
	end
	return resultStatus;--这里返回的状态只可能是RUNNING或者FAILURE
	
	
	--[[local resultStatus = b3.FAILURE
	for i = 1, self.childrenCount do
		local v = self.children[i];
		local status = v:_execute(tick)
		self.tChildStatus[i] = status;
		if status == b3.SUCCESS then
			resultStatus = b3.SUCCESS
		end
	end
	if(resultStatus == b3.SUCCESS)then
		return b3.SUCCESS;
	else
		for i = 1, self.tChildStatus do
			local status = self.tChildStatus[i];
			if status == b3.RUNNING then
				return b3.RUNNING
			end
		end
		return b3.FAILURE;
	end--]]
end
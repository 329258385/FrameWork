require 'behavior3_lua.core.Composite'
--并行执行所有任务 全部成功则返回成功 有一个失败则返回失败 有一个在running则返回running
--下次直接从头开始执行所有任务。
local parallel = b3.Class("Parallel", b3.Composite)
b3.Parallel = parallel

function parallel:ctor()
	b3.Composite.ctor(self)
	self.children = {}
	self.childrenCount = 0
	self.tChildStatus = {}
	self.name = "Parallel"
end

function parallel:open(tick)
	self.childrenCount = #self.children
end

function parallel:tick(tick)
	
	local resultStatus = b3.SUCCESS
	local bFailerStatus = false
	local bRunningStatus = false
	
	--从头开始执行所有任务
	for i = 1, self.childrenCount do
		local v = self.children[i];
		local status = v:_execute(tick)
		self.tChildStatus[i] = status;
		if status ~= b3.SUCCESS then
			if(status == b3.FAILURE)then
				bFailerStatus = true
			elseif(status == b3.RUNNING)then
				bRunningStatus = true
			else				
				resultStatus = status--此时状态可能是ERROR
			end
		end
	end
	--并行任务有一个失败的则返回失败
	if(bFailerStatus)then
		return b3.FAILURE;
	end
	if(bRunningStatus)then
		return b3.RUNNING;
	end
	return resultStatus;--这里返回的状态只可能是ERROR或者SUCCESS
end
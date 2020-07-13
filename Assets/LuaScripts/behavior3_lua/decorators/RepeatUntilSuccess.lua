require "behavior3_lua.core.Decorator"
--重复执行 直到成功  如果填了最大重复执行次数 都执行完了也没有成功则返回ERROR
local repeatUntilSuccess = b3.Class("RepeatUntilSuccess", b3.Decorator)
b3.RepeatUntilSuccess = repeatUntilSuccess

function repeatUntilSuccess:ctor(params)
	b3.Decorator.ctor(self)

	if not params then
		params = {}
	end

	self.name = "RepeatUntilSuccess"
	self.title = "Repeat Until Success"
	self.properties = {maxLoop = -1}
end

function repeatUntilSuccess:open(tick)
	tick.blackboard:set("i", 0, tick.tree.id, self.id)
	self.maxLoop = self.properties.maxLoop
end

function repeatUntilSuccess:tick(tick)
	if not self.child then
		return b3.ERROR
	end

	local i = tick.blackboard:get("i", tick.tree.id , self.id)
	local status = b3.ERROR

	while(self.maxLoop < 0 or i < self.maxLoop)
	do
		status = self.child:_execute(tick)
		if status == b3.FAILURE then
			i = i + 1
			tick.blackboard:set("i", i, tick.tree.id, self.id)
			if( self.maxLoop > 0 and i >= self.maxLoop)then
				return b3.ERROR
			else
				return b3.RUNNING
			end
		else
			break
		end
	end
	return status
	
	
	
	--[[if not self.child then
		return b3.ERROR
	end

	local i = tick.blackboard.get("i", tick.tree.id , self.id)
	local status = b3.ERROR

	while(self.maxLoop < 0 or i < self.maxLoop)
	do
		local status = self.child:_execute(tick)

		if status == b3.FAILURE then
			i = i + 1
		else
			break
		end
	end

	i = tick.blackboard.set("i", i, tick.tree.id, self.id)
	return status--]]
end

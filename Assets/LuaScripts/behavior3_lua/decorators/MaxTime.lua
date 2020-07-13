require 'behavior3_lua.core.Decorator'

local maxTime = b3.Class("MaxTime", b3.Decorator)
b3.MaxTime = maxTime

function maxTime:ctor(params)
	b3.Decorator.ctor(self)

	self.name = "MaxTime"
	self.title = "Max <maxTime>ms"
	self.parameters = {maxTime = 0}

	if not params or not params.maxTime then
		print("maxTime parameter in MaxTime decorator is an obligatory parameter")
		return
	end

	self.maxTime = params.maxTime
end

function maxTime:initialize(params)

end

function maxTime:open(tick)
	if not self.child then
		return b3.ERROR
	end

	local currTime = Time.time
	local startTime = tick.blackboard.get("startTime", tick.tree.id, self.id)
	if(startTime == nil)then
		startTime = currTime;
		tick.blackboard.set("startTime", currTime, tick.tree.id, self.id)
	end
	local status = self.child:_execute(tick)
	if currTime - startTime > self.maxTime then
		return b3.FAILURE
	end

	return status
end

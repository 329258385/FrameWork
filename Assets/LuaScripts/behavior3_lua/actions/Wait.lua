require 'behavior3_lua.core.Action'

local wait = b3.Class("Wait", b3.Action)
b3.Wait = wait

function wait:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "Wait"
	self.title = "Wait <milliseconds>ms"
	self.properties = {milliseconds = 0,}
	
	settings = settings or {}
end

function wait:open(tick)
	local startTime = Time.time
	tick.blackboard:set('startTime',startTime, tick.tree.id, self.id)
	self.waitDuration = self.properties.milliseconds/1000
	--print("wait:open self.waitDuration ="..self.waitDuration.." startTime ="..startTime)
end

function wait:tick(tick)
	local currTime = Time.time
	local startTime = tick.blackboard:get("startTime", tick.tree.id, self.id)
	local endTime = startTime + self.waitDuration;
	--print("wait:tick currTime ="..currTime.." endTime ="..endTime);
	if startTime + self.waitDuration  < Time.time then
		return b3.SUCCESS
	end
	return b3.RUNNING
end

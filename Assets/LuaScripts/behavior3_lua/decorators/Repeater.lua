require "behavior3_lua.core.Decorator"
--�����ظ�ִ����������ֱ������������ָ���Ĵ�����������ѡ�����ִ�������񣬼�ʹ�����񷵻�ʧ�ܡ�
local repeater = b3.Class("Repeater", b3.Decorator)
b3.Repeater = repeater

function repeater:ctor(params)
	b3.Decorator.ctor(self)

	if not params then
		params = {}
	end

	self.name = "Repeater"
	self.title = "Repeater <maxLoop>x"
	self.properties = {maxLoop = -1}
end

function repeater:initialize(params)

end

function repeater:open(tick)
	tick.blackboard:set("i", 0, tick.tree.id, self.id)
	self.maxLoop = self.properties.maxLoop
end

function repeater:tick(tick)
	if not self.child then
		return b3.ERROR
	end
	local i = tick.blackboard:get("i", tick.tree.id , self.id)
	local status = b3.SUCCESS
	
	while(self.maxLoop < 0 or i < self.maxLoop)
	do
		status = self.child:_execute(tick)
		if status == b3.SUCCESS or status == b3.FAILURE then
			i = i + 1
			tick.blackboard:set("i", i, tick.tree.id, self.id)
			if(self.maxLoop < 0 or i < self.maxLoop )then
				status = b3.RUNNING
				break
			end
		else
			break
		end
	end
	return status
	
	
	
	--[[if not self.child then
		return b3.ERROR
	end

	local i = tick.blackboard:get("i", tick.tree.id , self.id)
	local status = b3.SUCCESS

	while(self.maxLoop < 0 or i < self.maxLoop)
	do
		local status = self.child:_execute(tick)
		if status == b3.SUCCESS or status == b3.FAILURE then
			i = i + 1
		else
			break
		end
	end

	i = tick.blackboard:set("i", i, tick.tree.id, self.id)
	return status--]]
end

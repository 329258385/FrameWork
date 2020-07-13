require 'behavior3_lua.core.Decorator'
--����ѭ����ÿ�ζ�����RUNNING��ִ��һ��������
local forever = b3.Class("Forever", b3.Decorator)
b3.Forever = forever

function forever:ctor()
	b3.Decorator.ctor(self)

	self.name = "Forever"
end

function forever:tick(tick)
	if not self.child then
		return b3.ERROR
	end
	local status = self.child:_execute(tick)
	return b3.RUNNING
	--tick.blackboard:set('forever',1)
	--[[if(status == b3.RUNNING)then
		return b3.RUNNING
	end--]]
	--return b3.SUCCESS
end

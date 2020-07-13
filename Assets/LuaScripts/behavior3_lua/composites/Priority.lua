require 'behavior3_lua.core.Composite'
-- ˳��ѡ��ִ�������񣬵������񷵻�b3.FAILUREֱ�ӷ�����һ��������
--ֱ�����������񶼷���b3.FAILURE,�Ŵӵ�ǰ���񷵻�b3.FAILURE��
--�������񷵻�b3.SUCCESS����ֱ�Ӵӵ�ǰ���񷵻�b3.SUCCESS �������߼���ĸ��
local priority = b3.Class("Priority", b3.Composite)
b3.Priority = priority

function priority:ctor()
	b3.Composite.ctor(self)

	self.name = "Priority"
end

function priority:tick(tick)
	for i,v in pairs(self.children) do
		local status = v:_execute(tick)

		if status ~= b3.FAILURE then
			return status
		end
	end

	return b3.FAILURE
end


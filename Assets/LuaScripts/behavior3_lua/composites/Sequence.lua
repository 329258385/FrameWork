require 'behavior3_lua.core.Composite'
--˳��ִ�������� �������񷵻�b3.SUCCESSֱ�ӷ�����һ��������
--ֱ�����������񶼷���b3.SUCCESS,�Ŵӵ�ǰ���񷵻�b3.SUCCESS��
--�������񷵻�b3.FAILURE����ֱ�Ӵӵ�ǰ���񷵻�b3.FAILURE �������߼���ĸ��
local sequence = b3.Class("Sequence", b3.Composite)
b3.Sequence = sequence

function sequence:ctor()
	b3.Composite.ctor(self)

	self.name = "Sequence"
end

function sequence:tick(tick)
	for i = 1,#(self.children) do
		local v = self.children[i]
		local status = v:_execute(tick)
		--print(i,v)
		if status ~= b3.SUCCESS then
			return status
		end
	end
	return b3.SUCCESS
end

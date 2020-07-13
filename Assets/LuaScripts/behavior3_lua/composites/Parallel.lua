require 'behavior3_lua.core.Composite'
--����ִ���������� ȫ���ɹ��򷵻سɹ� ��һ��ʧ���򷵻�ʧ�� ��һ����running�򷵻�running
--�´�ֱ�Ӵ�ͷ��ʼִ����������
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
	
	--��ͷ��ʼִ����������
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
				resultStatus = status--��ʱ״̬������ERROR
			end
		end
	end
	--����������һ��ʧ�ܵ��򷵻�ʧ��
	if(bFailerStatus)then
		return b3.FAILURE;
	end
	if(bRunningStatus)then
		return b3.RUNNING;
	end
	return resultStatus;--���ﷵ�ص�״ֻ̬������ERROR����SUCCESS
end
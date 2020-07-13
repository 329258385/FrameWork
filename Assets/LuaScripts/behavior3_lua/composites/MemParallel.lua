require 'behavior3_lua.core.Composite'
--����ִ���������� ȫ���ɹ��򷵻سɹ� ��һ��ʧ���򷵻�ʧ�� ��һ����running�򷵻�running
--�´�ֱ��ִ������running״̬��������û�����ͷ��ʼִ����������
local memParallel = b3.Class("MemParallel", b3.Composite)
b3.MemParallel = memParallel

function memParallel:ctor()
	b3.Composite.ctor(self)
	self.children = {}
	self.childrenCount = 0
	self.tChildStatus = {}
	self.tRunningChild = {}
	self.name = "MemParallel"
end

function memParallel:open(tick)
	self.tRunningChild = {}
	self.childrenCount = #self.children
end

function memParallel:tick(tick)
	
	local resultStatus = b3.SUCCESS
	local bFailerStatus = false
	--�����running״̬������ �����ִ��running״̬������
	local curRuningChild = {}
	if(#self.tRunningChild > 0)then
		for i = 1, #self.tRunningChild do
			local vIndex = self.tRunningChild[i];
			local v = self.children[vIndex];
			local status = v:_execute(tick)
			self.tChildStatus[vIndex] = status;
			if status ~= b3.SUCCESS then
				if(status == b3.FAILURE)then
					bFailerStatus = true
				elseif(status == b3.RUNNING)then
					curRuningChild[#curRuningChild+1] = vIndex;
				else					
					resultStatus = status--��ʱ״̬������ERROR
				end
			end
		end
		self.tRunningChild = curRuningChild;
		--����������һ��ʧ�ܵ��򷵻�ʧ��
		if(bFailerStatus)then
			return b3.FAILURE
		end
		if(#self.tRunningChild > 0)then
			return b3.RUNNING;
		end
		return resultStatus;
	end
	--���û��running״̬���������ͷ��ʼִ����������
	self.tRunningChild = {}
	for i = 1, self.childrenCount do
		local v = self.children[i];
		local status = v:_execute(tick)
		self.tChildStatus[i] = status;
		if status ~= b3.SUCCESS then
			if(status == b3.FAILURE)then
				bFailerStatus = true
			elseif(status == b3.RUNNING)then
				self.tRunningChild[#self.tRunningChild+1] = i;
			else				
				resultStatus = status--��ʱ״̬������ERROR
			end
		end
	end
	--����������һ��ʧ�ܵ��򷵻�ʧ��
	if(bFailerStatus)then
		return b3.FAILURE;
	end
	if(#self.tRunningChild > 0)then
		return b3.RUNNING;
	end
	return resultStatus;--���ﷵ�ص�״ֻ̬������ERROR����SUCCESS
end
require 'behavior3_lua.core.Composite'
--����ִ����������  ��һ����running�򷵻�running ����ǿ�Ʒ��سɹ�
--�´�ֱ��ִ������running״̬��������û�����ͷ��ʼִ����������
local memParallelTrue = b3.Class("MemParallelTrue", b3.Composite)
b3.MemParallelTrue = memParallelTrue

function memParallelTrue:ctor()
	b3.Composite.ctor(self)
	self.children = {}
	self.childrenCount = 0
	self.tChildStatus = {}
	self.tRunningChild = {}
	self.name = "MemParallelTrue"
end

function memParallelTrue:open(tick)
	self.tRunningChild = {}
	self.childrenCount = #self.children
end

function memParallelTrue:tick(tick)
	
	--�����running״̬������ �����ִ��running״̬������
	local curRuningChild = {}
	if(#self.tRunningChild > 0)then
		for i = 1, #self.tRunningChild do
			local vIndex = self.tRunningChild[i];
			local v = self.children[vIndex];
			local status = v:_execute(tick)
			self.tChildStatus[vIndex] = status;
			if(status == b3.RUNNING)then
				curRuningChild[#curRuningChild+1] = vIndex;
			end
		end
		self.tRunningChild = curRuningChild;
		if(#self.tRunningChild > 0)then
			return b3.RUNNING;
		end
		return b3.SUCCESS;
	end
	--���û��running״̬���������ͷ��ʼִ����������
	self.tRunningChild = {}
	for i = 1, self.childrenCount do
		local v = self.children[i];
		local status = v:_execute(tick)
		self.tChildStatus[i] = status;
		if(status == b3.RUNNING)then
			self.tRunningChild[#self.tRunningChild+1] = i;
		end
	end
	if(#self.tRunningChild > 0)then
		return b3.RUNNING;
	end
	return b3.SUCCESS;
end
require 'behavior3_lua.core.Composite'
--����ִ���������� ��һ���ɹ��򷵻سɹ� û�гɹ�����һ����running�򷵻�running  ȫ��ʧ���򷵻�ʧ�� 
local parallelSelector = b3.Class("ParallelSelector", b3.Composite)
b3.ParallelSelector = parallelSelector

function parallelSelector:ctor()
	b3.Composite.ctor(self)
	self.children = {}
	self.childrenCount = 0
	self.tChildStatus = {}
	self.tRunningChild = {}
	self.name = "ParallelSelector"
end

function parallelSelector:open(tick)
	self.tRunningChild = {}
	self.childrenCount = #self.children
end

function parallelSelector:tick(tick)
	local resultStatus = b3.FAILURE
	local bSuccessStatus = false
	--�����running״̬������ �����ִ��running״̬������
	local curRuningChild = {}
	if(#self.tRunningChild > 0)then
		for i = 1, #self.tRunningChild do
			local vIndex = self.tRunningChild[i];
			local v = self.children[vIndex];
			local status = v:_execute(tick)
			self.tChildStatus[vIndex] = status;
			if status == b3.SUCCESS then
				bSuccessStatus = true
			elseif status == b3.RUNNING then
				curRuningChild[#curRuningChild+1] = vIndex;--��¼������running������
				resultStatus = b3.RUNNING
			end
		end
		self.tRunningChild = {}
		self.tRunningChild = curRuningChild;
		--����������һ��ʧ�ܵ��򷵻�ʧ��
		if(bSuccessStatus)then
			return b3.SUCCESS
		end
		return resultStatus;
	end
	
	--��ͷ��ʼִ����������
	for i = 1, self.childrenCount do
		local v = self.children[i];
		local status = v:_execute(tick)
		self.tChildStatus[i] = status;
		if status == b3.SUCCESS then
			bSuccessStatus = true
		elseif status == b3.RUNNING then
			self.tRunningChild[#self.tRunningChild+1] = i;--��¼������running������
			resultStatus = b3.RUNNING
		end
		
	end
	--����������һ���ɹ����򷵻سɹ�
	if(bSuccessStatus)then
		return b3.SUCCESS
	end
	return resultStatus;--���ﷵ�ص�״ֻ̬������RUNNING����FAILURE
	
	
	--[[local resultStatus = b3.FAILURE
	for i = 1, self.childrenCount do
		local v = self.children[i];
		local status = v:_execute(tick)
		self.tChildStatus[i] = status;
		if status == b3.SUCCESS then
			resultStatus = b3.SUCCESS
		end
	end
	if(resultStatus == b3.SUCCESS)then
		return b3.SUCCESS;
	else
		for i = 1, self.tChildStatus do
			local status = self.tChildStatus[i];
			if status == b3.RUNNING then
				return b3.RUNNING
			end
		end
		return b3.FAILURE;
	end--]]
end
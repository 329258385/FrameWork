require 'behavior3_lua.core.Composite'
--�߼���Priorityһ�����������漰��һ��b3.RUNNING�ķ���ֵ��
--���������񲢲�֪��ִ�н���ǳɹ�����ʧ�ܣ��ҵ���һ����ܸ����㣬����δ�����������֣�
--����Priority���񣬵����ٴα����������ʱ�򣬻��Ǵӵ�һ��������ʼ������
--����MemPriorityȴ���ס֮ǰ��Running��������,Ȼ���´α�����ʱ��
--ֱ�Ӵ�Running���Ǹ�������ʼ������Ժܷ���ļ���֮ǰ�жϵ��߼����ߡ�
local memPriority = b3.Class("MemPriority", b3.Composite)
b3.MemPriority = memPriority

function memPriority:ctor()
	b3.Composite.ctor(self)
	self.children = {}
	self.childrenCount = 0
	self.name = "MemPriority"
end

function memPriority:open(tick)
	tick.blackboard:set("runningChild", 0, tick.tree.id, self.id)
	self.childrenCount = #self.children
end

function memPriority:tick(tick)	
	local currentChildIndex = tick.blackboard:get("runningChild", tick.tree.id, self.id)
	if(currentChildIndex == 0 or currentChildIndex > self.childrenCount)then
		currentChildIndex = 1;
	end
	for i = currentChildIndex, self.childrenCount do
		local v = self.children[i];
		local status = v:_execute(tick)

		if status ~= b3.FAILURE then
			if status == b3.RUNNING then
				tick.blackboard:set("runningChild", i, tick.tree.id, self.id)
			end
			
			return status
		end
	end
	return b3.FAILURE
	--[[local child = tick.blackboard:get("runningChild", tick.tree.id, self.id)
	for i,v in pairs(self.children) do
		local status = v:_execute(tick)

		if status ~= b3.FAILURE then
			if status == b3.RUNNING then
				tick.blackboard:set("runningChild", i, tick.tree.id, self.id)
			end
			
			return status
		end
	end

	return b3.FAILURE--]]
end
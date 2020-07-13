require 'behavior3_lua.core.Composite'
--�߼���Sequenceһ�������������漰��һ��b3.RUNNING�ķ���ֵ��
--���������񲢲�֪��ִ�н���ǳɹ�����ʧ�ܣ��ҵ���һ����ܸ����㣬����δ�����������֣�
--����Sequence���񣬵����ٴα����������ʱ�򣬻��Ǵӵ�һ��������ʼ������
--����MemSequenceȴ���ס֮ǰ��Running��������,Ȼ���´α�����ʱ��
--ֱ�Ӵ�Running���Ǹ�������ʼ������Ժܷ���ļ���֮ǰ�жϵ��߼����ߡ�
local memSequence = b3.Class("MemSequence", b3.Composite)
b3.MemSequence = memSequence

function memSequence:ctor()
	b3.Composite.ctor(self)
	self.children = {}
	self.childrenCount = 0
	self.name = "MemSequence"
end

function memSequence:open(tick)
	tick.blackboard:set("runningChild", 0, tick.tree.id, self.id)
	self.childrenCount = #self.children
end

function memSequence:tick(tick)
	local currentChildIndex = tick.blackboard:get("runningChild", tick.tree.id, self.id)
	if(currentChildIndex == 0 or currentChildIndex > self.childrenCount)then
		currentChildIndex = 1;
	end
	for i = currentChildIndex, self.childrenCount do
		local v = self.children[i];
		local status = v:_execute(tick)

		if status ~= b3.SUCCESS then
			if status == b3.RUNNING then
				tick.blackboard:set("runningChild", i, tick.tree.id, self.id)
			end

			return status
		end
	end
	return b3.SUCCESS
	--[[local child = tick.blackboard:get("runningChild", tick.tree.id, self.id)
	for i,v in pairs(self.children) do
		
		local status = v:_execute(tick)

		if status ~= b3.SUCCESS then
			if status == b3.RUNNING then
				tick.blackboard:set("runningChild", i, tick.tree.id, self.id)
			end

			return status
		end
	end

	return b3.SUCCESS--]]
end
require 'behavior3_lua.core.Composite'
--����ִ�У������ж������Ƿ�Ϊ�棬�������Ϊ�棬��ôִ��"��ʱִ��"��֧������ִ��"��ʱִ��"��֧�����ؽ�����ݾ���ִ�з�֧��ִ�н����������
--��ִ�е�һ���ڵ� ִ�н�������سɹ���ִ�еڶ����ڵ� ����ִ�е������ڵ�
local switch = b3.Class("Switch", b3.Composite)
b3.Switch = switch

function switch:ctor()
	b3.Composite.ctor(self)
	self.children = {}
	self.name = "Switch"
end

function switch:open(tick)
	tick.blackboard:set("runningChild", 0, tick.tree.id, self.id)
	self.childrenCount = #self.children
end

function switch:tick(tick)
	if(self.childrenCount == 3)then
		--����RUNNING״̬���ӽڵ�
		local currentChildIndex = tick.blackboard:get("runningChild", tick.tree.id, self.id)
		if(currentChildIndex == 2 or currentChildIndex == 3)then
			local v = self.children[i];
			local status = v:_execute(tick)
			if status == b3.RUNNING then
					tick.blackboard:set("runningChild", i, tick.tree.id, self.id)
			end
			return status
		end
		------------
		--��һ���ڵ�Ϊ�жϽڵ� ִ�е�һ���ڵ㷵�سɹ���ִ�еڶ����ڵ� ����ʧ����ִ�е������ڵ�
		local status = self.children[1]:_execute(tick)
		if(status == b3.SUCCESS)then
			status = self.children[2]:_execute(tick)
		else
			status = self.children[3]:_execute(tick)
		end
		return status;
	else
		--switch����±����������ڵ� ����ֱ�ӷ��ش���״̬
		return b3.ERROR
	end
end

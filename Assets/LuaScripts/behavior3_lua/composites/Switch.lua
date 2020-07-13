require 'behavior3_lua.core.Composite'
--条件执行，首先判断条件是否为真，如果条件为真，那么执行"真时执行"分支；否则，执行"假时执行"分支。返回结果根据具体执行分支的执行结果来决定。
--先执行第一个节点 执行结果若返回成功则执行第二个节点 否则执行第三个节点
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
		--处理RUNNING状态的子节点
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
		--第一个节点为判断节点 执行第一个节点返回成功则执行第二个节点 返回失败则执行第三个节点
		local status = self.children[1]:_execute(tick)
		if(status == b3.SUCCESS)then
			status = self.children[2]:_execute(tick)
		else
			status = self.children[3]:_execute(tick)
		end
		return status;
	else
		--switch组合下必须有三个节点 否则直接返回错误状态
		return b3.ERROR
	end
end

require 'behavior3_lua.core.Composite'
--逻辑和MemSequence一样，但是这里会打乱子任务的顺序 不是按照从左到右从上到下的排列顺序执行了
local randomMemSequence = b3.Class("RandomMemSequence", b3.Composite)
b3.RandomMemSequence = randomMemSequence

function randomMemSequence:ctor()
	b3.Composite.ctor(self)
	self.children = {}
	self.childrenCount = 0
	self.shuffleChilden = {}
	self.name = "RandomMemSequence"
end

function randomMemSequence:open(tick)		
	tick.blackboard:set("runningChild", 0, tick.tree.id, self.id)
	self.childrenCount = #self.children
	local randomArr = DeepCopy(self.children)
	self.shuffleChilden = RandomTable(randomArr)
end

function randomMemSequence:tick(tick)
	local currentChildIndex = tick.blackboard:get("runningChild", tick.tree.id, self.id)
	if(currentChildIndex == 0 or currentChildIndex > self.childrenCount)then
		currentChildIndex = 1;
	end
	for i = currentChildIndex, self.childrenCount do
		local v = self.shuffleChilden[i];
		local status = v:_execute(tick)

		if status ~= b3.SUCCESS then
			if status == b3.RUNNING then
				tick.blackboard:set("runningChild", i, tick.tree.id, self.id)
			end
			return status
		end
	end
	return b3.SUCCESS
end


--[[local function WeightRandom(arr)
	local tempArr = arr;
	local arrCount = #tempArr;
	local sum = 0;
	for  i = 1, arrCount then
		sum += arrCount[i];
	end
	int randVal = math.random(sum)--random(sum);
	printf("%d\n",randVal);
	int grade = 0;
	for (int i = 0;i < length; i++)
	{
		if (randVal <= a[i])
		{
			grade = i+1;
			break;
		}
		randVal -= a[i];
	}
end--]]
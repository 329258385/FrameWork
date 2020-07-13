require 'behavior3_lua.core.Action'
--巡逻 强制返回success
--patrolType 1范围取当前对象的出生点和巡逻半径 2取配置中的巡逻坐标点
--speed巡逻速度
--angularSpeed 转角度的速度
--arriveDistance 自身和巡逻点的距离小于此值认为到达
--randomPatrol 当patrolType为2时有效。为true表示随机巡逻点 为false表示按照配置顺序一个一个走,走完循环回到第一个 
--waypoints 对象配置身上没有的情况下使用此配置巡逻点,每个巡逻点坐标用";"隔开  如： "120,2,100;105,2,100,90,2,100;105,2,115,105,2,85" 
local patrolInRange = b3.Class("PatrolInRange", b3.Action)
b3.PatrolInRange = patrolInRange

local PatrolType= {
    CircleRegion = 1 ,    ---圆形区域巡逻 只用出生点和巡逻半径 
    FixedPoint = 2   		---固定点巡逻 需要巡逻点的列表

}

function patrolInRange:ctor(settings)
	b3.Action.ctor(self, settings)
	self.name = "PatrolInRange"
	self.title = "在范围内巡逻"
	self.properties = {patrolType = "1", speed = "0",angularSpeed = "0",arriveDistance = "", randomPatrol = "",waypoints = ""}
	settings = settings or {}
	--解析巡逻点配置
	self.waypoints = {}
	local wayP = string.split(settings.waypoints,";")
	for i = 1, #wayP do
		local tmpPos = string.split(wayP[i],",");
		local v3Pos = Vector3.New(tonumber(tmpPos[1]),tonumber(tmpPos[2]),tonumber(tmpPos[3]));
		table.insert(self.waypoints,v3Pos)
	end
	
	self.wayPointCount = #self.waypoints;
	self.pathPending = false --是否正在赶往目标点
	self.waypointIndex = 1 --当前的目标点索引
end

function patrolInRange:open(tick)
	if(self.properties.patrolType == PatrolType.CircleRegion)then
		--如果是圆形巡逻区域就要取得出生点和巡逻半径
		self.birthPos = tick.targetLua.birthPos;
		self.patrolRadius = tick.targetLua.patrolRadius;
	else
		if(tick.targetLua.patrolPosList ~= nil)then
			--如果对象上有巡逻点列表 就直接用对象身上的
			self.waypoints = tick.targetLua.patrolPosList;
		end
	end
end

function patrolInRange:tick(tick)
	if(self.pathPending)then
		local direction = self.targetPos - tick.target.transform.position;
		--print("patrolInRange self.targetPos.x="..self.targetPos.x.." self.targetPos.y="..self.targetPos.y.." self.targetPos.z"..self.targetPos.z)
		--print("patrolInRange tick.target.transform.position.x="..tick.target.transform.position.x.." tick.target.transform.position.y="..tick.target.transform.position.y.." tick.target.transform.position.z"..tick.target.transform.position.z)
		--print("patrolInRange direction.x="..direction.x.." direction.y="..direction.y.." direction.z"..direction.z)
		direction.y = 0;
		local sqrPos = Vector3.SqrMagnitude(direction);
		self.targetRotation = Quaternion.LookRotation(direction, Vector3.up)
		--print("patrolInRange self.targetRotation.x ="..self.targetRotation.x.." self.targetRotation.y="..self.targetRotation.y.." self.targetRotation.z="..self.targetRotation.z);
		if not IsNull(self.targetRotation) then
			--print("patrolInRange tick.target.transform.rotation.x ="..tick.target.transform.rotation.x.." tick.target.transform.rotation.y="..tick.target.transform.rotation.y.." tick.target.transform.rotation.z="..tick.target.transform.rotation.z);
			tick.target.transform.rotation = Quaternion.Slerp(tick.target.transform.rotation , self.targetRotation, self.properties.angularSpeed)
		end
		--print("patrolInRange sqrPos ="..sqrPos)
		if (sqrPos < self.properties.arriveDistance * self.properties.arriveDistance)then
			tick.target.transform.rotation = self.targetRotation;
			self.pathPending = false;
		else			
			tick.target.transform:Translate(Vector3.forward * Time.deltaTime * self.properties.speed)
		end
		
	else
		if(self.properties.patrolType == PatrolType.CircleRegion)then
			--[[math.randomseed(tostring(os.clock()):reverse():sub(1, 6))
			local curRadius = math.sqrt(math.random(0,1))  * self.patrolRadius;
			local radin = math.random(0,1)*2*math.pi;
			local posx = self.birthPos.x + curRadius * math.cos(radin);  -- math.random(self.birthPos.x-self.patrolRadius, self.birthPos.x + self.patrolRadius);
			local posz = self.birthPos.z + curRadius * math.sin(radin); --math.random(self.birthPos.z-self.patrolRadius, self.birthPos.z + self.patrolRadius);
			--]]
			local posx = 0;
			local posz = 0;
			while true do
				posx = math.random(self.birthPos.x-self.patrolRadius, self.birthPos.x + self.patrolRadius);
				posz = math.random(self.birthPos.z-self.patrolRadius, self.birthPos.z + self.patrolRadius);
				self.targetPos = Vector3.New(posx,self.birthPos.y,posz);
				local direction = self.targetPos - self.birthPos;
				local sqrPos = Vector3.SqrMagnitude(direction);
				if (sqrPos <= self.patrolRadius * self.patrolRadius)then
					break;
				end
			end
		else
			local curIndex = self.waypointIndex;
			if(self.properties.randomPatrol == "true")then
				--随机选取巡逻点
				self.waypointIndex = math.random(1, self.wayPointCount);
			else
				--按配置顺序巡逻
				if(self.waypointIndex + 1 <= self.wayPointCount)then
					self.waypointIndex = self.waypointIndex + 1;
				else
					self.waypointIndex = (self.waypointIndex + 1)-self.wayPointCount;
				end
			end
			self.targetPos = self.waypoints[self.waypointIndex];
		end
		tick.blackboard:set("lastPatrolPos", self.targetPos)
		self.pathPending = true
		local forwVector = self.targetPos - tick.target.transform.position
		if(forwVector.x == 0 and forwVector.z == 0)then
			self.pathPending = false
		end
	end
	return b3.SUCCESS
end

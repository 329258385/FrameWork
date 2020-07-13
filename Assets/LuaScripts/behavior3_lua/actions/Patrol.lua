require 'behavior3_lua.core.Action'
--巡逻
--speed巡逻速度
--angularSpeed 转角度的速度
--arriveDistance 自身和巡逻点的距离小于此值认为到达
--randomPatrol 为true表示随机巡逻点 为false表示按照配置顺序一个一个走,走完循环回到第一个
--waypoints 巡逻点,每个巡逻点坐标用";"隔开  如： "120,2,100;105,2,100,90,2,100;105,2,115,105,2,85" 
local patrol = b3.Class("Patrol", b3.Action)
b3.Patrol = patrol

function patrol:ctor(settings)
	b3.Action.ctor(self, settings)
	self.name = "Patrol"
	self.title = "巡逻"
	self.properties = {speed = "0",angularSpeed = "0",arriveDistance = "", randomPatrol = "",waypoints = ""}
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

function patrol:tick(tick)
	if(self.pathPending)then
		local direction = self.targetPos - tick.target.transform.position;
		self.targetRotation = Quaternion.LookRotation(direction, Vector3.up)
		if not IsNull(self.targetRotation) then
			tick.target.transform.rotation = Quaternion.Slerp(tick.target.transform.rotation , self.targetRotation, self.properties.angularSpeed)
		end
		local sqrPos = Vector3.SqrMagnitude(direction);
		if (sqrPos < self.properties.arriveDistance)then
			self.pathPending = false;
		else			
			tick.target.transform:Translate(Vector3.forward * Time.deltaTime * self.properties.speed)
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
		self.pathPending = true
		local forwVector = self.targetPos - tick.target.transform.position
		if(forwVector.x == 0 and forwVector.y == 0)then
			self.pathPending = false
		end
		--[[local forwVector = self.targetPos - tick.target.transform.position;
		forwVector.y = 0;
		if(forwVector.x == 0 and forwVector.y == 0)then
			self.pathPending = false
		else
			self.targetRotation = Quaternion.LookRotation(forwVector, Vector3.up)
			if not IsNull(self.targetRotation) then
				tick.target.transform.rotation = Quaternion.Slerp(tick.target.transform.rotation , self.targetRotation, self.properties.angularSpeed)
			end
		end--]]
	end
	return b3.RUNNING
end

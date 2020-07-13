local StateMachine = require "GameLogic.Battle.State.StateMachine"
local skillPool = require "GameLogic.Battle.SkillPool"
local Monstar = BaseClass("Monstar", Updatable)
local base = Updatable


local MonsterState= {
    STAND = 1 ,     
    CHECK = 2,     
    WALK = 3,      
    WARN = 4,     
    CHASE = 5,      
    RETURN =6,
    ATTACK= 7     

}

-- 技能配置
local skill1 = {
    id = 1,
    solt = 1,
    state = "Attack1",
    coolDownTime = 1,
    curCDTime = 0,
    isCDing = false,
    damage = 100
}

local skill2 = {
    id = 2,
    solt = 2,
    state = "Attack2",
    coolDownTime = 2,
    curCDTime = 0,
    isCDing = false,
    damage = 200
}

local function file_load(filename)
	local file
	if filename == nil then
		file = io.stdin
	else
		local err
		file, err = io.open(filename, "rb")
		if file == nil then
			error(("Unable to read '%s': %s"):format(filename, err))
		end
	end
	local data = file:read("*a")

	if filename ~= nil then
		file:close()
	end

	if data == nil then
		error("Failed to read " .. filename)
	end

	return data
end

local function Start(self, chara_go)
    -- 角色gameObject
    self.chara_go = chara_go
    -- 角色控制器
    self.chara_ctrl = chara_go:GetComponentInChildren(typeof(CS.UnityEngine.CapsuleCollider))
    -- 动画控制器
    self.anim_ctrl = chara_go:GetComponentInChildren(typeof(CS.UnityEngine.Animation))
    -- Animator
    self.anim_m = chara_go:GetComponentInChildren(typeof(CS.UnityEngine.Animator))
    -- assert(not IsNull(self.chara_ctrl), "chara_ctrl null")
    --assert(not IsNull(self.anim_ctrl), "anim_ctrl null")

    self.id = "123456"
    self.nickName = "Who?"
    self.gameObject = chara_go
    self.transform = chara_go.transform


    self.initialPosition = chara_go.transform.position
    self.alertRadius = 30
    self.defendRadius = Mathf.Min(self.alertRadius,25)
    self.chaseRadius = 20 --追捕半径
    self.attackRange = 4 --Mathf.Min(self.defendRadius,2) -- 攻击距离
    self.wanderRadius = Mathf.Min(self.chaseRadius,8)

    self.walkSpeed = 5
    self.runSpeed = 2
    self.turnSpeed =0.1

    self.currentState = MonsterState.STAND
    self.actionWeight = { 3000, 3000, 4000 }
    self.actRestTme = 5
    self.playerUnit = CharacterManager:GetInstance().MainPlayer
    self.lastActime = Time.time
    self:randomAction(self)
	-- 注册技能
    self.skillPool = skillPool.New()

    self.skillPool:AddSkillData(skill1)
    self.skillPool:AddSkillData(skill2)
	
    -- self.__stateMachine = StateMachine.New()

    --UIBoardManager:GetInstance():Open(self, EBoardType.PLAYER)
	--行为树必须要的几个数据 字段名称也要求是如下固定的
	self.hatredTargets = {};--仇恨目标列表 {{targetTf,targetLua},...}
	self.buffInfos = {};--buff列表 {1001 = {},1002 = {},...}
	self.birthPos = self.initialPosition--{105, 2, 100};--出生点{0,0,0}
	self.patrolRadius = 25;--巡逻半径
	--self.patrolPosList = {};--巡逻点列表(和巡逻半径只用一个)
	self.curHpPer = 1;--当前血量百分比
	self.maxHp = 10000;--最大血量
	self.curHp = 10000;--当前血量
	
	self.behaviorTree = b3.BehaviorTree.new()
	self.blackBoard = b3.Blackboard.new()
	
	self.behaviorTree:load(file_load("Assets/LuaScripts/behavior3_lua/new_MonsterBehavior.json"), {})
end

local function Update(self)
	if(self.behaviorTree ~= null)then
		self.behaviorTree:tick(self.chara_go, self.blackBoard, self)
	end
	
	self:UpdateHatredList(self)


    -- Logger.Log("currentState--"..self.currentState)

    --[[if self.currentState == MonsterState.STAND then
       if Time.time - self.lastActime > self.actRestTme   then
           self:randomAction(self)
       end
       self:enemyDistanceCheck(self)
    end   
    
    if self.currentState ==  MonsterState.CHECK then
        local animTime = self.anim_m:GetCurrentAnimatorStateInfo(0).length
        if Time.time - self.lastActime > animTime  then
            self:randomAction(self)
        end
        self:enemyDistanceCheck(self)
    end       

    if self.currentState == MonsterState.WALK then
        self.chara_go.transform:Translate(Vector3.forward * Time.deltaTime * self.walkSpeed)
        if not IsNull(self.targetRotation) then
            self.chara_go.transform.rotation = Quaternion.Slerp(self.chara_go.transform.rotation, self.targetRotation, self.turnSpeed)
        end
        if Time.time - self.lastActime > self.actRestTme  then
            self:randomAction(self)
        end
        self:wanderRadiusCheck(self)
    end

    if self.currentState == MonsterState.WARN  then

        if self.is_Warned ~= true then
            -- self.anim_m:SetTrigger("Warn")
            self.is_Warned = true;
            self.anim_m:SetTrigger("Attack1")
        end

        local forwVector = self.playerUnit.transform.position - self.chara_go.transform.position
		local pos = self.playerUnit.transform.position - self.chara_go.transform.position;
		pos.y = 0;
        self.targetRotation = Quaternion.LookRotation(pos, Vector3.up)
        --self.targetRotation = Quaternion.LookRotation(self.playerUnit.transform.position - self.chara_go.transform.position, Vector3.up)
        self.chara_go.transform.rotation = Quaternion.Slerp(self.chara_go.transform.rotation , self.targetRotation, self.turnSpeed)

        self:warningCheck(self)
    end    

    if self.currentState == MonsterState.CHASE then

        if self.is_Running == false then
            self.anim_m:SetInteger("State", 11)
            self.is_Running = true
        end

        self.chara_go.transform:Translate(Vector3.forward * Time.deltaTime * self.runSpeed)
        if not IsNull(self.targetRotation) then
            self.chara_go.transform.rotation = Quaternion.Slerp(self.chara_go.transform.rotation, self.targetRotation, self.turnSpeed)
        end

        self:chaseRadiusCheck(self)
    end    

    if  self.currentState == MonsterState.RETURN then
        -- body
		local pos = self.initialPosition - self.chara_go.transform.position;
		pos.y = 0;
        self.targetRotation = Quaternion.LookRotation(pos, Vector3.up)
        --self.targetRotation = Quaternion.LookRotation(self.initialPosition - self.chara_go.transform.position, Vector3.up)
        self.chara_go.transform:Translate(Vector3.forward * Time.deltaTime * self.runSpeed )
        if not IsNull(self.targetRotation) then
            self.chara_go.transform.rotation = Quaternion.Slerp(self.chara_go.transform.rotation, self.targetRotation, self.turnSpeed)
        end
        self:returnCheck(self)

    end

    if self.currentState == MonsterState.ATTACK then
        self.anim_m:SetTrigger("Attack2")
		local playerAnim_m = self.playerUnit:GetComponentInChildren(typeof(CS.UnityEngine.Animator))
		playerAnim_m:SetTrigger("TriggerHit")
		local pos = self.playerUnit.transform.position - self.chara_go.transform.position;
		pos.y = 0;
        self.targetRotation = Quaternion.LookRotation(pos, Vector3.up)
        self.chara_go.transform.rotation = Quaternion.Slerp(self.chara_go.transform.rotation , self.targetRotation, self.turnSpeed)
        self.attackRadiusCheck(self)

    end--]]

       
end


local function randomAction(self)

    self.lastActime = Time.time
    self.is_Running = false

    local number = math.random( 0, self.actionWeight[1] + self.actionWeight[2] + self.actionWeight[3])

    if number <= self.actionWeight[1]  then
        self.currentState = MonsterState.STAND
        self.anim_m:SetInteger("State", 16)

    elseif self.actionWeight[1] < number and number<= self.actionWeight[2] + self.actionWeight[3] then
            self.currentState = MonsterState.CHECK
            self.anim_m:SetInteger("State", 16)
    end

    local weight = self.actionWeight[1] + self.actionWeight[2] + self.actionWeight[3]
    if (self.actionWeight[1] +  self.actionWeight[2]) <= number and number<= weight then

            self.currentState = MonsterState.WALK
            self.targetRotation = Quaternion.Euler(0, math.random( 1,5)*90,0)
            self.anim_m:SetInteger("State", 11)
    end

end


local function enemyDistanceCheck(self)
    self.diatanceToPlayer = Vector3.Distance(self.playerUnit.transform.position, self.chara_go.transform.position)
    if  self.diatanceToPlayer < self.attackRange then
        self.currentState = MonsterState.ATTACK
    -- elseif self.diatanceToPlayer < self.defendRadius and  self.diatanceToPlayer > self.attackRange then
    --     self.currentState = MonsterState.WARN
    elseif self.diatanceToPlayer < self.alertRadius  then
        self.currentState = MonsterState.CHASE
   
    end
    -- body
end

local function wanderRadiusCheck(self)
    self.diatanceToPlayer = Vector3.Distance(self.playerUnit.transform.position, self.chara_go.transform.position)
    self.diatanceToInitial = Vector3.Distance(self.chara_go.transform.position, self.initialPosition)

    if self.diatanceToPlayer < self.attackRange then
        self.currentState = MonsterState.ATTACK
    elseif self.diatanceToPlayer < self.defendRadius then
        self.currentState = MonsterState.CHASE
    elseif self.diatanceToPlayer < self.alertRadius then
        self.currentState = MonsterState.CHASE  
    end

    if self.diatanceToInitial > self.wanderRadius  then
		local pos = self.initialPosition - self.chara_go.transform.position;
		pos.y = 0;
        self.targetRotation = Quaternion.LookRotation(pos, Vector3.up)
        --self.targetRotation = Quaternion.LookRotation(self.initialPosition-self.chara_go.transform.position,Vector3.up)
        self.currentState = MonsterState.RETURN  
        -- body
    end


end


local function warningCheck(self)
    self.diatanceToPlayer = Vector3.Distance(self.playerUnit.transform.position, self.chara_go.transform.position)

    Logger.Log("self.diatanceToPlayer"..self.diatanceToPlayer)
    if self.diatanceToPlayer < self.defendRadius and self.diatanceToPlayer > self.attackRange then
        self.is_Warned = false
        self.currentState = MonsterState.CHASE
    elseif self.diatanceToPlayer < self.attackRange then
        self.is_Warned = false
        self.is_Attacked = false
        self.is_Running  = false
        self.currentState = MonsterState.ATTACK
    end

    if self.diatanceToPlayer > self.alertRadius then
        self.is_Warned = false
        self.is_Running = false
        self.currentState = MonsterState.CHECK
        self:randomAction()
        -- body
    end

    -- body
end

local function chaseRadiusCheck(self)
    self.diatanceToPlayer = Vector3.Distance(self.playerUnit.transform.position, self.chara_go.transform.position)
    self.diatanceToInitial = Vector3.Distance(self.chara_go.transform.position, self.initialPosition)
    
    if self.diatanceToPlayer < self.attackRange then
        self.is_Warned = false
        self.is_Attacked = false
        self.currentState = MonsterState.ATTACK
    end

    if (self.diatanceToInitial > self.chaseRadius or self.diatanceToPlayer > self.alertRadius) then
        self.currentState = MonsterState.RETURN
    end

    -- body
end


local function  attackRadiusCheck(self)

    self.diatanceToPlayer = Vector3.Distance(self.playerUnit.transform.position, self.chara_go.transform.position)
    if self.diatanceToPlayer < self.defendRadius and self.diatanceToPlayer > self.attackRange then
        self.is_Warned = false
        self.is_Running = false
        self.currentState = MonsterState.CHASE
    elseif self.diatanceToPlayer < self.attackRange then
        self.is_Warned = false
        self.is_Attacked = false
        self.is_Running = false
        self.currentState = MonsterState.ATTACK
    end

    if self.diatanceToPlayer > self.alertRadius  then
        self.is_Warned = false
        self.is_Running = false
        self.currentState = MonsterState.CHECK
        self:randomAction()
    end

    -- body
end

local function returnCheck(self)
    self.diatanceToInitial = Vector3.Distance(self.chara_go.transform.position, self.initialPosition)
    if self.diatanceToInitial  < 0.5 then
        -- body
        self.is_Running = false
        self:randomAction()
    end


    -- body
end

local function PlaySkillAttack(self,skillid)
	print("Monstar PlaySkillAttack skillid = "..skillid);
    if self.skillPool:IsSkillCanRelase(skillid) then
		self.skillPool:ReleaseSkill(skillid)
		return true;
	end
	return false;
end

local function UpdateHatredList(self)
    self.diatanceToPlayer = Vector3.Distance(self.playerUnit.transform.position, self.chara_go.transform.position)
    if self.diatanceToPlayer < self.attackRange then
		if(#self.hatredTargets <= 0)then
			local targetinfo = {targetTf = self.playerUnit, targetLua = CharacterManager:GetInstance().MainPlayerLua}
			table.insert(self.hatredTargets, targetinfo)
		end
	elseif(self.diatanceToPlayer > self.chaseRadius)then		
		self.hatredTargets = {};
    end
end


local function __delete(self)
    base.__delete(self)
    UIBoardManager:GetInstance():Destroy(self, EBoardType.PLAYER)
end

Monstar.Start = Start
Monstar.Update = Update
Monstar.randomAction = randomAction
Monstar.enemyDistanceCheck = enemyDistanceCheck
Monstar.warningCheck = warningCheck
Monstar.chaseRadiusCheck = chaseRadiusCheck
Monstar.returnCheck = returnCheck
Monstar.wanderRadiusCheck = wanderRadiusCheck
Monstar.attackRadiusCheck = attackRadiusCheck
Monstar.PlaySkillAttack = PlaySkillAttack
Monstar.UpdateHatredList = UpdateHatredList

return Monstar
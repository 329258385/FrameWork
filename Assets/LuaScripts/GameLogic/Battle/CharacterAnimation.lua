--[[-- added by wsh @ 2018-02-26
-- 临时Demo：战斗场景角色动画控制脚本
--]]
local skillPool = require "GameLogic.Battle.SkillPool"
local CharacterAnimation = BaseClass("CharacterAnimation", Updatable)
local base = Updatable

--控制器内的参数
local animatorParameters =
{
	FMoveSpeed = CS.UnityEngine.Animator.StringToHash("FMoveSpeed"),
	IsQc = CS.UnityEngine.Animator.StringToHash("IsQc"),
	AttackID = CS.UnityEngine.Animator.StringToHash("AttackID"),
	TriggerAttack = CS.UnityEngine.Animator.StringToHash("TriggerAttack"),
	IsDead = CS.UnityEngine.Animator.StringToHash("IsDead"),
	TurnAround = CS.UnityEngine.Animator.StringToHash("TurnAround"),
	VerticalSpeed = CS.UnityEngine.Animator.StringToHash("VerticalSpeed"),
	IsGrounded = CS.UnityEngine.Animator.StringToHash("IsGrounded"),
	TriggerHit = CS.UnityEngine.Animator.StringToHash("TriggerHit"),
	IsDefense = CS.UnityEngine.Animator.StringToHash("IsDefense"),
	Result = CS.UnityEngine.Animator.StringToHash("Result"),
	TriggerResult = CS.UnityEngine.Animator.StringToHash("TriggerResult")
};
-- 武器配置
local weapon = {
    path = "Models/m_01/m_01_weapon.prefab"
}

-- 技能配置
local skill1 = {
    id = 1001,
    solt = 1,
    state = "Attack1",
    coolDownTime = 1,
    curCDTime = 0,
    isCDing = false,
    damage = 100
}

local skill2 = {
    id = 1002,
    solt = 2,
    state = "Attack2",
    coolDownTime = 2,
    curCDTime = 0,
    isCDing = false,
    damage = 200
}

local skill3 = {
    id = 1003,
    solt = 3,
    state = "Attack3",
    coolDownTime = 1,
    curCDTime = 0,
    isCDing = false,
    damage = 180
}

local function Start(self, chara_go)
    -- 角色gameObject
    self.chara_go = chara_go
    -- 角色控制器
    self.chara_ctrl = chara_go:GetComponentInChildren(typeof(CS.UnityEngine.CharacterController))
    -- 动画控制器
    self.anim_ctrl = chara_go:GetComponentInChildren(typeof(CS.UnityEngine.Animation))
    -- Animator
    self.anim_m = chara_go:GetComponentInChildren(typeof(CS.UnityEngine.Animator))
    assert(not IsNull(self.chara_ctrl), "chara_ctrl null")
    --assert(not IsNull(self.anim_ctrl), "anim_ctrl null")
    -- MainCamera
    self.mainCamera = GameObject.FindGameObjectWithTag("MainCamera")
    -- 武器挂点
    self.weaponRoot = chara_go.transform:Find("HP_weapon_R")
    -- 武器
    self.weapon = CharacterManager:GetInstance():LoadWeapon(weapon.path, self.weaponRoot)

    -- 展示人物名片
    self.id = "123456"
    self.nickName = "Who?"
    self.gameObject = chara_go
    self.transform = chara_go.transform
    --UIBoardManager:GetInstance():Open(self, EBoardType.PLAYER)
    -- 注册动画事件
    --self:InitAnimationEventData(self)
    -- 注册技能
    self.skillPool = skillPool.New()

    self.skillPool:AddSkillData(skill1)
    self.skillPool:AddSkillData(skill2)
    self.skillPool:AddSkillData(skill3)
end

local function Update(self)
    self:UpdateGravity(self)
end

local isJump = false
local curMoveSpeed = 0
local curYAxisDelta = 0
local walkSpeed = 3
local runSpeed = 6
local rotateSpeed = 5
local jumpSpeed = 10
local jumpCount = 2
local curJumpCount = 0
local curQuaternion = nil

local curRandomIdleTime = 0
local thisRandomIdleTime = 0
local idleStateIndex = {14, 16}

local isSkillRelease = false
local function LateUpdate(self)
    if IsNull(self.chara_ctrl) or IsNull(self.anim_m) then
        return
    end
	local fHorizontal = CS.ETCInput.GetAxis("Horizontal");
	local fVertical = CS.ETCInput.GetAxis("Vertical");
    --待机
    if self.chara_ctrl.isGrounded and fHorizontal == 0 and fVertical == 0 then
        self.anim_m:SetFloat(animatorParameters.FMoveSpeed,0)
        curMoveSpeed = 0
    end

    --移动从走到跑
    if (fHorizontal ~= 0 or fVertical ~= 0) then
        local moveSpeed = 0
        local axisX = fHorizontal
        local axisY = fVertical
        if (axisX * axisX + axisY * axisY) > 0.81 then
            --跑
            moveSpeed = runSpeed
        else
            --走
            moveSpeed = walkSpeed
        end
        curMoveSpeed = Mathf.Lerp(curMoveSpeed,moveSpeed,Time.deltaTime * 5)
        self.anim_m:SetFloat(animatorParameters.FMoveSpeed,curMoveSpeed/runSpeed)
        -- Logger.Log("转向前player forward: ".. self.transform.forward.x .."  "..self.transform.forward.z)
        --转向摄像机并移动
        local targetDirecton = Vector3.New(fHorizontal, 0, -fVertical)
        local targetEularAngle = math.atan( targetDirecton.z,targetDirecton.x )--Vector2.Angle(Vector2.New(CS.ETCInput.GetAxis("Horizontal"),CS.ETCInput.GetAxis("Vertical")),Vector2.New(1,0))
        if targetEularAngle > math.pi or targetEularAngle < -math.pi then -- pi精度问题
            targetEularAngle = 180
        elseif targetEularAngle < 0 then
            targetEularAngle = 360 + targetEularAngle / math.pi * 180
        else
            targetEularAngle = targetEularAngle / math.pi * 180
        end
        -- Logger.Log("player forward: ".. targetEularAngle)
        local cameraAngle = self.mainCamera.transform.rotation.eulerAngles.y
        targetEularAngle = targetEularAngle + cameraAngle + 90 - 360
        -- Logger.Log("player forward: ".. targetEularAngle .. " camera forward: "..cameraAngle)
        targetEularAngle = Mathf.LerpAngle(self.transform.rotation.eulerAngles.y, targetEularAngle, Time.deltaTime * rotateSpeed )
        curQuaternion = Quaternion.AngleAxis(targetEularAngle,Vector3.up)
        self.transform.rotation = curQuaternion
       
        self.chara_ctrl:Move(self.transform.forward * curMoveSpeed * Time.deltaTime)
        
    end

    --跳跃
    if CS.ETCInput.GetButtonDown("Jump") then
        Logger.Log("Jump")
        if self:IsCanContinueJump(self) then
            isJump = true
            curYAxisDelta = jumpSpeed
            curJumpCount = curJumpCount + 1
			self.anim_m:SetFloat(animatorParameters.VerticalSpeed,jumpSpeed)
			self.anim_m:SetBool(animatorParameters.IsGrounded,false)
			self.anim_m:CrossFadeInFixedTime("JumpStart",0.1)
        end
    end

    --技能
    if CS.ETCInput.GetButtonDown("Attack1") then
		self:PlaySkillAttack(tonumber(skill1.id));
    end
    if CS.ETCInput.GetButtonDown("Attack2") then
		self:PlaySkillAttack(tonumber(skill2.id));
    end
    if CS.ETCInput.GetButtonDown("Attack_Jump") then
        self:PlaySkillAttack(tonumber(skill3.id));
    end

end

local function PlaySkillAttack(self,skillid)
	print("111PlaySkillAttack skillid = "..skillid);
    if self.skillPool:IsSkillCanRelase(skillid) then
		self.skillPool:ReleaseSkill(skillid)
		self.anim_m:SetInteger(animatorParameters.AttackID, skillid)
		self.anim_m:SetTrigger(animatorParameters.TriggerAttack)
	end
end

local Gravity = 9.81
local YAxisMove = nil

local function UpdateGravity(self)
    if not isJump then
        YAxisMove = Vector3.up:Mul(-Gravity)
        self.chara_ctrl:Move(YAxisMove:Mul(Time.deltaTime))
    else
        curYAxisDelta = curYAxisDelta - Gravity * Time.deltaTime
		self.anim_m:SetFloat(animatorParameters.VerticalSpeed,curYAxisDelta)
        YAxisMove = Vector3.up:Mul(curYAxisDelta)
        self.chara_ctrl:Move(YAxisMove:Mul(Time.deltaTime))
    end

    if self.chara_ctrl.isGrounded then
        if isJump then
            Logger.Log("jump end!!!!!!!!!!!!")
        end
		self.anim_m:SetBool(animatorParameters.IsGrounded,true)
        isJump = false
        curYAxisDelta = 0
        curJumpCount = 0
    end
end

local function IsCanContinueJump()
    return curJumpCount < jumpCount
end

local function __delete(self)
    base.__delete(self)
    UIBoardManager:GetInstance():Destroy(self, EBoardType.PLAYER)
end

local function InitAnimationEventData(self)
    if self.anim_m then
        local clips = self.anim_m.runtimeAnimatorController.animationClips
        for i = 0, clips.Length - 1 do
            self:RegisterAnimationEvent(clips[i], 0.1 * i, "1|1234")
        end
    end
end

-- 注册动画事件
local function RegisterAnimationEvent(clip, time, param)
    local _event = CS.UnityEngine.AnimationEvent()
    _event.functionName = "AnimationEventCallBack"
    -- 例如 类型|参数
    _event.stringParameter = param
    _event.time = time
    clip:AddEvent(_event)
end

-- 动画事件的回调
local function AnimationEventCallBack(stringParameter)
    Logger.Log("From Character Base")
    Logger.Log(stringParameter)
end

CharacterAnimation.Start = Start
CharacterAnimation.Update = Update
CharacterAnimation.LateUpdate = LateUpdate
CharacterAnimation.UpdateGravity = UpdateGravity
CharacterAnimation.IsCanContinueJump = IsCanContinueJump
CharacterAnimation.PlaySkillAttack = PlaySkillAttack
CharacterAnimation.__delete = __delete
CharacterAnimation.InitAnimationEventData = InitAnimationEventData
CharacterAnimation.RegisterAnimationEvent = RegisterAnimationEvent

return CharacterAnimation

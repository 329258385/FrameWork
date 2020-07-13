--[[技能池]]
local SkillPool = BaseClass("SkillPool", Updatable)
local base = Updatable

local function __init(self)
    Logger.Log("SkillPool.__init")
    self:OnCreate()
end

local function __delete(self)
    self:OnDestroy()
end

local function OnCreate(self)
    Logger.Log("SkillPool.OnCreate")
    self.__skillTable = {}
end

local function OnDestroy()
    self.__skillTable = nil
end

local function AddSkillData(self, skillData)
    self.__skillTable[#self.__skillTable + 1] = skillData
end

local function GetSkillData(id)
    for i = 1, #self.__skillTable do
        local skillData = self.__skillTable[i]
        if skillData.id == id then
            return skillData
        end
    end
end

local function IsSkillCanRelase(self, id)
    for i = 1, #self.__skillTable do
        local skillData = self.__skillTable[i]
        if skillData.id == id then
            return not skillData.isCDing
        end
    end
end

local function ReleaseSkill(self, id)
    for i = 1, #self.__skillTable do
        local skillData = self.__skillTable[i]
        if skillData.id == id then
            skillData.isCDing = true
        end
    end
end

local function Update(self)
    for i = 1, #self.__skillTable do
        local skillData = self.__skillTable[i]
        if skillData.isCDing and skillData.curCDTime < skillData.coolDownTime then
            skillData.curCDTime = skillData.curCDTime + Time.deltaTime
        elseif skillData.isCDing and skillData.curCDTime >= skillData.coolDownTime then
            skillData.curCDTime = 0
            skillData.isCDing = false
        end
    end
end

SkillPool.__init = __init
SkillPool.__delete = __delete
SkillPool.OnCreate = OnCreate
SkillPool.OnDestroy = OnDestroy
SkillPool.AddSkillData = AddSkillData
SkillPool.IsSkillCanRelase = IsSkillCanRelase
SkillPool.ReleaseSkill = ReleaseSkill
SkillPool.Update = Update

return SkillPool

--[[状态机]]
local StateMachine = BaseClass("StateMachine", Updatable)
local base = Updatable

local function __init(self)
    Logger.Log("StateMachine.__init")
    --初始化状态列表
    self.__stateTable = {}
end

--添加或设置状态
local function AddState(self, state)
    self.__stateTable[#self.__stateTable + 1] = state
end

--改变状态
local function SetState(self, stateIndex)
    for i = 1, table.count(self.__stateTable) do
        if self.__stateTable[i].__typeIndex == stateIndex then
            self.__stateTable[i]:Execute(self)
        end
    end
end

--获得当前状态
local function GetCurrentState(self)
end

local function Update(self)
    if not IsNull(self.__stateTable) and #self.__stateTable ~= 0 then
    --Logger.Log("StateMachine.Update")
    end
end

StateMachine.__init = __init
StateMachine.AddState = AddState
StateMachine.SetState = SetState
StateMachine.GetCurrentState = GetCurrentState
StateMachine.Update = Update

return StateMachine

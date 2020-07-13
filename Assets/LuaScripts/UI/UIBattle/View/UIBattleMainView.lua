--[[
-- added by wsh @ 2018-02-26
-- UIBattleMain视图层
--]]
local UIBattleMainView = BaseClass("UIBattleMainView", UIBaseView)
local base = UIBaseView

-- 各个组件路径
local back_btn_path = "BackBtn"
local skil1_mask_path = "Attack1/Mask"
local skill2_mask_path = "Attack2/Mask"
local skill3_mask_path = "Attack_Jump/Mask"

local function OnCreate(self)
    base.OnCreate(self)

    -- 控制角色
    self.chara = nil
    --[[
    -- 退出按钮
    self.back_btn = self:AddComponent(Button, back_btn_path)
    self.back_btn:SetOnClick(
        function()
            self.ctrl:Back()
        end
    )
    -- 技能CDMask
    self.skil1_mask = self:GetComponent(Image, skil1_mask_path)
    self.skil2_mask = self:GetComponent(Image, skill2_mask_path)
    self.skill3_mask = self:GetComponent(Image, skill3_mask_path)
    --]]
end

local function OnEnable(self)
    base.OnEnable(self)
end

local function LateUpdate(self)
    if IsNull(self.chara) then
        self.chara = CS.UnityEngine.GameObject.FindGameObjectWithTag("Player")
    end

    if IsNull(self.chara) then
        return
    end

    --local isSwipe = CS.ETCInput.GetControlSwipeIn("FreeLookTouchPad")
    --CS.ETCInput.SetControlSwipeIn("FreeLookTouchPad", isSwipe)

    local axisXValue = CS.ETCInput.GetAxis("Horizontal")
    local axisYValue = CS.ETCInput.GetAxis("Vertical")
    if Time.frameCount % 30 == 0 then
    --print("ETCInput : "..axisXValue..", "..axisYValue)
    end

    -- 说明：这里根据获取的摇杆输入向量控制角色移动
    -- 示例代码略
end

local function OnDestroy(self)
    base.OnDestroy(self)
end

UIBattleMainView.OnCreate = OnCreate
UIBattleMainView.OnEnable = OnEnable
UIBattleMainView.LateUpdate = LateUpdate
UIBattleMainView.OnDestroy = OnDestroy

return UIBattleMainView

require 'behavior3_lua.core.Action'
--给全局变量赋值
--paramName 全局变量的名字 如：MoveSpeed
--paramScript 算式字符串 如：1+2*2-3
--useGlobalName 算式中引用的全局变量 如算式：2*MoveSpeed  其中MoveSpeed就是全局变量
--useGlobalName内可以包含多个全局变量 用";"隔开 如："MoveSpeed; angleSpeed"
local setMathValue = b3.Class("SetMathValue", b3.Action)
b3.SetMathValue = setMathValue

function setMathValue:ctor(settings)
	b3.Action.ctor(self, settings)

	self.name = "SetMathValue"
	self.title = "给全局变量赋值"
	self.properties = {paramName = "",paramScript = "",useGlobalName = ""}
	settings = settings or {}
	self.curParamScript = settings.paramScript;
	self.globalParamStr = {}--全局参数名
	local params = string.split(settings.useGlobalName,";")
	for i = 1, #params do
		local tmpPar = params[i];
		table.insert(self.globalParamStr,tmpPar)
	end
end

--[[function setMathValue:open(tick)
	self.curParamScript = self.properties.paramScript;
	for i = 1, #self.globalParamStr do
		local tmpStr = self.globalParamStr[i];
		if(string.find(self.curParamScript , tmpStr))then
			local tmpValue = tick.blackboard:get(tmpStr)
			if(tmpValue == nil)then
				tmpValue = ""
			end
			self.curParamScript = string.gsub(self.curParamScript, tmpStr, tostring(tmpValue))
		end
	end
end--]]

function setMathValue:tick(tick)
	if( self.properties.paramName ~= "" and self.properties.paramName ~= nil )then
		self.curParamScript = self.properties.paramScript;
		for i = 1, #self.globalParamStr do
			local tmpStr = self.globalParamStr[i];
			if(string.find(self.curParamScript , tmpStr))then
				local tmpValue = tick.blackboard:get(tmpStr)
				if(tmpValue == nil)then
					print("SetMathValue : not find global param "..tostring(tmpStr));
					tmpValue = ""
					return b3.ERROR
				end
				self.curParamScript = string.gsub(self.curParamScript, tmpStr, tostring(tmpValue))
			end
		end
		local script = "return "..self.curParamScript;
		local paramValue = load(script)();
		tick.blackboard:set(self.properties.paramName, paramValue)
	end
	return b3.SUCCESS
end

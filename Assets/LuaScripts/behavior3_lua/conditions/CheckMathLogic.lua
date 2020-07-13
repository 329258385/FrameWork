require 'behavior3_lua.core.Condition'
--逻辑运算
--'判断运算结果，如果符合逻辑判断返回true，否则返回false
--逻辑判断类型： == , != , < , <= , > , >= , or , and , not
--数学运算函数："+" , "-" , "*" , "/" , math.random
--数据类型包含： int、float、bool、string、nil
--算式，  比如CheckMathLogic( math.random(0,1)+1 < 1.5 )表示在[0,1]区间随机取一个数，加上1，如果小于1.5返回true，否则返回false
--param |  string
local checkMathLogic = b3.Class("CheckMathLogic", b3.Condition)
b3.CheckMathLogic = checkMathLogic

function checkMathLogic:ctor(settings)
	b3.Condition.ctor(self, settings)
	self.name = "CheckMathLogic"
	self.title = "逻辑运算"
	self.properties = {param = ""}
	settings = settings or {}
end

function checkMathLogic:tick(tick)
	if( self.properties.param == "" or self.properties.param == nil)then
		return b3.FAILURE
	end 
	local script = "return "..self.properties.param;
	local status = load(script)();
	if(status == true)then
		print("checkMathLogic true")
		return b3.SUCCESS
	else
		print("checkMathLogic false")
		return b3.FAILURE	
	end
end

require 'behavior3_lua.core.Action'

local failer = b3.Class("Failer", b3.Action)
b3.Failer = failer

function failer:ctor()
	print("failer:ctor")
	b3.Action.ctor(self)
	
	self.name = "Failer"
end

function failer:tick()
	print("failer:tick")
	return b3.FAILURE
end

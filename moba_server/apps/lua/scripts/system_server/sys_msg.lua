local Respones = require("Respones")
local Stype = require("Stype")
local Cmd = require("Cmd")
local mysql_game = require("database/mysql_game")

local sys_msg_data = {} -- 如果你加载进来，就会存放到这个表里面
local sys_msg_version = 0 -- 什么时候时候加载的，搞一个时间戳，可以作为我们版本号;

function load_sys_msg() 
	mysql_game.get_sys_msg(function (err, ret)
		if err then
			Scheduler.once(load_sys_msg, 5000) 
			return
		end

		sys_msg_version = Utils.timestamp()
		--
		if ret == nil or #ret <= 0 then
			sys_msg_data = {}
			return
		end 
		-- 

		sys_msg_data = ret

		-- 过了晚上1点，在更新一下
		local tormorow = Utils.timestamp_today() + 25 * 60 * 60
		Scheduler.once(load_sys_msg, (tormorow - sys_msg_version) * 1000) 
		--[[
		local k, v 
		for k,v in pairs(sys_msg_data) do
			print(v) 
		end
		]]
	end)
end


Scheduler.once(load_sys_msg, 5000)

-- {stype, ctype, utag, body}
function get_sys_msg(s, req)
	local uid = req[3]
	local body = req[4]
	

	if (body ~= nil and body.ver_num == sys_msg_version) then
		local msg = {Stype.System, Cmd.eGetSysMsgRes, uid, {
			status = Respones.OK,
			ver_num = sys_msg_version,
		}}

		Session.send_msg(s, msg)
		return
	end

	local msg = {Stype.System, Cmd.eGetSysMsgRes, uid, {
		status = Respones.OK,
		ver_num = sys_msg_version,
		sys_msgs = sys_msg_data,
	}}
	
	Session.send_msg(s, msg)
end

local sys_msg = {
	get_sys_msg = get_sys_msg,
}

return sys_msg

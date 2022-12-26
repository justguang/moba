local Respones = require("Respones")
local Stype = require("Stype")
local Cmd = require("Cmd")
local mysql_game = require("database/mysql_game")
local login_bonues = require("system_server/login_bonues")
local redis_game = require("database/redis_game")
local redis_rank = require("database/redis_rank")

-- {stype, ctype, utag, body}
function get_ugame_info(s, req)
	local uid = req[3];
	print("get_ugame_info ".. uid)
	mysql_game.get_ugame_info(uid, function (err, ugame_info)
		if err then -- 告诉客户端系统错误信息;
			local msg = {Stype.System, Cmd.eGetUgameInfoRes, uid, {
				status = Respones.SystemErr,
			}}

			Session.send_msg(s, msg)
			return
		end

		if ugame_info == nil then -- 没有游戏信息
			mysql_game.insert_ugame_info(uid, function(err, ret)
				if err then -- 告诉客户端系统错误信息;
					local msg = {Stype.System, Cmd.eGetUgameInfoRes, uid, {
						status = Respones.SystemErr,
					}}

					Session.send_msg(s, msg)
					return
				end

				get_ugame_info(s, req)
			end)
			return
		end

		-- 读取到了
		-- 找到了我们gkey所对应的游客数据;
		if ugame_info.ustatus ~= 0 then --账号被查封
			local msg = {Stype.System, Cmd.eGetUgameInfoRes, uid, {
				status = Respones.UserIsFreeze,
			}}

			Session.send_msg(s, msg)
			return
		end
		-- end 

		-- 更新reidis数据库里面的数据;
		redis_game.set_ugame_info_inredis(uid, ugame_info)
		-- end 

		-- 刷新一下世界排行榜
		redis_rank.flush_world_rank_with_uchip_inredis(uid, ugame_info.uchip)
		-- end

		-- 检查登陆奖励
		login_bonues.check_login_bonues(uid, function (err, bonues_info)
			if err then -- 告诉客户端系统错误信息;
				local msg = {Stype.System, Cmd.eGetUgameInfoRes, uid, {
					status = Respones.SystemErr,
				}}

				Session.send_msg(s, msg)
				return
			end

			-- 返回给客户端
			local msg = { Stype.System, Cmd.eGetUgameInfoRes, uid, {
				status = Respones.OK,
				uinfo = {
					uchip = ugame_info.uchip,
					uexp = ugame_info.uexp,
					uvip  = ugame_info.uvip,
					uchip2  = ugame_info.uchip2,
					uchip3 = ugame_info.uchip3, 
					udata1  = ugame_info.udata1,
					udata2  = ugame_info.udata2,
					udata3 = ugame_info.udata3,

					bonues_status = bonues_info.status,
					bonues = bonues_info.bonues,
					days = bonues_info.days,
				}
			}}
			Session.send_msg(s, msg)
		end)
		--end
	end)
end

local ugame = {
	get_ugame_info = get_ugame_info,
}
return ugame
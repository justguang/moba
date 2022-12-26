local mysql_center = require("database/mysql_auth_center")
local redis_center = require("database/redis_center")

local Respones = require("Respones")
local Stype = require("Stype")
local Cmd = require("Cmd")


-- {stype, ctype, utag, body}
function login(s, req)
	local utag = req[3]
	local uname_login_req = req[4]
	
	-- 检查参数
	if uname_login_req.uname ~= nil and string.len(uname_login_req.uname) <= 0 or 
	   uname_login_req.upwd ~= nil and string.len(uname_login_req.upwd) ~= 32 then 
	   	local msg = {Stype.Auth, Cmd.eUnameLoginRes, utag, {
			status = Respones.InvalidParams,
		}}

		Session.send_msg(s, msg)
		return
	end

	-- 检查用户名和密码是否正确
	mysql_center.get_uinfo_by_uname_upwd(uname_login_req.uname, uname_login_req.upwd, function (err, uinfo)
		if err then -- 告诉客户端系统错误信息;
			local msg = {Stype.Auth, Cmd.eUnameLoginRes, utag, {
				status = Respones.SystemErr,
			}}

			Session.send_msg(s, msg)
			return
		end

		if uinfo == nil then -- 没有查到对应的 用户,返回不存在用户，或密码错误
			local msg = {Stype.Auth, Cmd.eUnameLoginRes, utag, {
				status = Respones.UnameOrUpwdError,
			}}

			Session.send_msg(s, msg)
			return
		end

		if uinfo.status ~= 0 then --账号被查封
			local msg = {Stype.Auth, Cmd.eUnameLoginRes, utag, {
				status = Respones.UserIsFreeze,
			}}

			Session.send_msg(s, msg)
			return
		end
		-- end

		redis_center.set_uinfo_inredis(uinfo.uid, uinfo)

		local msg = { Stype.Auth, Cmd.eUnameLoginRes, utag, {
			status = Respones.OK,
			uinfo = {
				unick = uinfo.unick,
				uface = uinfo.uface,
				usex  = uinfo.usex,
				uvip  = uinfo.uvip,
				uid = uinfo.uid, 
			}
		}}
		Session.send_msg(s, msg)
	end)
	--end
end

local uname_login = {
	login = login,
}

return uname_login

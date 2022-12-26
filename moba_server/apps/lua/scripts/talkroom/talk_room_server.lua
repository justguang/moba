-- 聊天室server
local session_set = {} -- 保存所有客户端进入到聊天室的集合
function broadcast_except(except_session,msg)
	for i = 1, #session_set do 
		local tmp_s = session_set[i];
		if  tmp_s ~= except_session then 
			Session.send_msg(tmp_s, msg);
		end
	end
end


function on_recv_login_cmd(s)
	for	i=1, #session_set do
		if s == session_set[i] then
			local msg={1,2,0,{status = -1}}
			Session.send_msg(s,msg); -- status=-1,客户端已存在session_set
			return;
		end
	end
	
	table.insert(session_set,s);
	local msg={1,2,0,{status = 1}}
	Session.send_msg(s,msg); -- status = 1,客户端成功加入到session_set
	
	local s_ip,s_port = Session.get_address(s);
	msg = {1,7,0,{ip = s_ip,port = s_port}}
	broadcast_except(s,msg);-- 广播其他客户端
end

function on_recv_exit_cmd(s)
	for i = 1, #session_set do
		if s == session_set[i] then
			table.remove(session_set,i);
			local msg = {1,4,0,{status = 1}} -- status=1 表示退出聊天室成功
			Session.send_msg(s,msg);			
					
			local s_ip,s_port = Session.get_address(s);
			msg = {1,8,0,{ip = s_ip,port = s_port}}
			broadcast_except(s,msg);-- 广播其他客户端
			return;
		end
	end
	
	local msg = {1,4,0,{status = -1}} -- status=-1 表示退出时不在聊天室里
	Session.send_msg(s,msg);
	
end

function on_recv_send_msg_cmd(s,str)
	for i = 1, #session_set do
		if s == session_set[i] then
			local msg = {1,6,0,{status = 1}} -- status=1 表示发送消息成功
			Session.send_msg(s,msg);			
					
			local s_ip, s_port = Session.get_address(s)
			msg = {1, 9, 0, {ip = s_ip,port = s_port,content = str}}
			broadcast_except(s,msg)
			return;
		end
	end
	local msg = {1,6,0,{status = -1}} -- status=-1 表示发送消息失败
	Session.send_msg(s,msg);
end

-- (msg[1,2,3,4] => stype,ctype,utag,body)
function talk_room_recv_cmd(s, msg)
	local ctype = msg[2];
	local body = msg[4];
	if ctype == 1 then
		on_recv_login_cmd(s);
	elseif ctype ==3 then
		on_recv_exit_cmd(s);
	elseif ctype ==5 then
		on_recv_send_msg_cmd(s,body.content);
	end
end

function talk_room_session_disconnect(s)
	local s_ip,s_port = Session.get_address(s);
	for i = 1, #session_set do
		if s == session_set[i] then
			print("remove session from talk room: "..s_ip.." : "..s_port);
			table.remove(session_set,i);
			
			local msg = {1,8,0,{ip = s_ip,port = s_port}}
			broadcast_except(s,msg);-- 广播其他客户端
		end
	end
end


local talk_room_service = {
  on_session_recv_cmd = talk_room_recv_cmd,
  on_session_disconnect = talk_room_session_disconnect,
}

local talk_room_server = {
  stype = 1,
  service = talk_room_service,
}

return talk_room_server;

local Stype = require("Stype")

local remote_servers = {}

-- 注册我们的服务所部署的IP地址和端口
remote_servers[Stype.Auth] = {
	stype = Stype.Auth,
	ip = "127.0.0.1",
	port = 8000,
	desic = "Auth server",
}

remote_servers[Stype.System] = {
	stype = Stype.System,
	ip = "127.0.0.1",
	port = 8001,
	desic = "System server",
}

remote_servers[Stype.Logic] = {
	stype = Stype.Logic,
	ip = "127.0.0.1",
	port = 8002, 
	desic = "Logic Server"
}


local game_config = {
	gateway_tcp_ip = "127.0.0.1",
	gateway_tcp_port = 6080,

	gateway_ws_ip = "127.0.0.1",
	gateway_ws_port = 6081,

	servers = remote_servers,
	
	-- auth数据库配置
	auth_mysql = {
		host = "127.0.0.1",
		port = 3306,
		db_name = "auth_center",
		uname = "root",
		upwd = "Cxcz1902",
	},
	
	-- game数据库配置
	game_mysql = {
		host = "127.0.0.1",
		port = 3306,
		db_name = "moba_game",
		uname = "root",
		upwd = "Cxcz1902",
	},
	
	center_redis = {
		host = "127.0.0.1", -- redis 所在的host
		port = 6379, -- redis 端口
		db_index = 1, -- 数据库 1
	},
	
	game_redis = {
		host = "127.0.0.1", -- redis所在的host
		port = 6379, -- reidis 端口
		db_index = 2, -- 数据库2
	},
	
	-- 做排行榜的redis服务器
	rank_redis = {
		host = "127.0.0.1", -- redis所在的host
		port = 6379, -- reidis 端口
		db_index = 3, -- 数据库3
	},
	
	-- logic server udp
	logic_udp = {
		host = "127.0.0.1",
		port = 8800,
	},
}

return game_config

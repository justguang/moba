local game_config = require("game_config")
local redis_conn = nil

local function is_connected()
	if not redis_conn then 
		return false
	end

	return true
end

function redis_connect_to_rank()
	local host = game_config.rank_redis.host
	local port = game_config.rank_redis.port
	local db_index = game_config.rank_redis.db_index

	Redis.connect(host, port, function (err, conn)
		if err ~= nil then
			Logger.error(err)
			Scheduler.once(redis_connect_to_rank, 5000)
			return
		end

		redis_conn = conn
		Logger.debug("connect to redis rank db success!!!!")
		Redis.query(redis_conn, "select " .. db_index, function (err, ret)
		end)
	end)

end

redis_connect_to_rank()

-- redis 有序集合, key WOLD_CHIP_RANK 专门用来做世界排行的有序集合
-- 做好友，每个人都有它好友的一个 FRIREND_CHIP_RANK_UID() 有序集合;

local WOLD_CHIP_RANK = "WOLD_CHIP_RANK"

function flush_world_rank_with_uchip_inredis(uid, uchip)
	if redis_conn == nil then 
		Logger.error("redis rank disconnected")
		return
	end

	local redis_cmd = "zadd WOLD_CHIP_RANK " .. uchip .. " " .. uid
	Redis.query(redis_conn, redis_cmd, function (err, ret)
		if err then
			
			return 
		end
	end)
end


-- n：要刷的排行榜的数目,
-- ret_handler: 回掉函数
function get_world_rank_with_uchip_inredis(n, ret_handler)
	if redis_conn == nil then 
		Logger.error("redis rank disconnected")
		return
	end

	-- zrange 是由小到大的排列;
	-- zrevrange 由大到小
	-- local redis_cmd = "zrange WOLD_CHIP_RANK 0 " .. n
	local redis_cmd = "zrevrange WOLD_CHIP_RANK 0 " .. n
	Redis.query(redis_conn, redis_cmd, function (err, ret)
		if err then
			if ret_handler then
				ret_handler("zrange WOLD_CHIP_RANK inredis error", nil)
			end
			return 
		end

		-- 排行榜没有任何数据
		if ret == nil or #ret <= 0 then 
			ret_handler(nil, nil)
			return
		end

		-- 获取得到了排行班的数据
		local rank_info = {}
		local k, v

		for k, v in pairs(ret) do
			rank_info[k] = tonumber(v) 
		end
		-- end 
		
		if ret_handler then
			ret_handler(nil, rank_info)
		end
	end)
end

local redis_rank = {
	flush_world_rank_with_uchip_inredis = flush_world_rank_with_uchip_inredis,
	get_world_rank_with_uchip_inredis = get_world_rank_with_uchip_inredis,
	is_connected = is_connected,
}

return redis_rank




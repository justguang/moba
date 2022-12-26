## 目录结构：
3rd：第三方代码
apps：放各个服务器的代码【gateway、center_server、game_server、system_server、test】
build：放跨平台的编译工程
netbus：基本的框架
database：mysql、redis相关
lua_wrapper：lua相关
utils：自己扩展的工具函数


## 引用第三方：
libuv-1.44.2:[https://dist.libuv.org/dist/]
API_doc:[https://docs.libuv.org/en/v1.x/api.html]

mysql-connectot-c 6.1.11:[https://downloads.mysql.com/archives/c-c/]
API_doc:[https://dev.mysql.com/doc/c-api/8.0/en/]

protobuf-3.18.0:[https://github.com/protocolbuffers/protobuf/releases/tag/v3.18.0]

redis-win-3.0.504:[https://github.com/microsoftarchive/redis]

lua-5.3.4:[https://www.lua.org/ftp/]

还有md5、base64、sha1加密解密，http协议解析，tolua，json

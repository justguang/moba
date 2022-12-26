#ifndef __LUA_WRAPER_H__
#define __LUA_WRAPER_H__

#include <string>
#include "lua.hpp"

using namespace std;

class lua_wrapper {
public:
	static void init();
	static void exit();
	static bool do_file(std::string& lua_file);
	static lua_State* lua_state();
	static void reg_func2lua(const char* name, int(*c_func)(lua_State* L));
	static int execute_script_handler(int nHandler, int numArgs);
	static void remove_script_handler(int nHandler);
	static void add_search_path(std::string& path);

};

#endif


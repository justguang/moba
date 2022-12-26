#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include <iostream>
#include <string>
using namespace std;


#include "../../netbus/netbus.h"
#include "../../netbus/proto_man.h"
#include"proto/pf_cmd_map.h"
#include"../../utils/logger.h"
#include"../../utils/time_list.h"
#include"../../utils/timestamp.h"
#include "../../database/mysql_wrapper.h"
#include "../../database/redis_wrapper.h"

static void on_logger_timer(void* udata) {
	log_debug("hello guang");
}

static void on_mysql_query_cb(const char* err, MYSQL_RES* result,void* udata) {
	if (err != NULL) {
		printf("%s\n", err);
		return;
	}
	printf("mysql query success\n");
}

static void on_redis_query_cb(const char* err, redisReply* result,void* udata) {
	if (err != NULL) {
		printf("%s\n", err);
		return;
	}
	printf("redis query success\n");
}

static void on_mysql_open_cb(const char* err, void* context,void* udata) {
	if (err != NULL) {
		printf("%s\n", err);
		return;
	}

	printf("connect mysql success!\n");

	//mysql_wrapper::query(context, (char*)"update ulevel set ulevel=13 where id=12", query_cb);
	mysql_wrapper::query(context, (char*)"select * from ulevel", on_mysql_query_cb);
	mysql_wrapper::close(context);
}

static void on_redis_open_cb(const char* err, void* context,void* udata) {
	if (err != NULL) {
		printf("%s\n", err);
		return;
	}

	printf("connect redis success!\n");

	redis_wrapper::query(context, (char*)"select 1", on_redis_query_cb);
	redis_wrapper::close_redis(context);
}

static void test_db() {
	mysql_wrapper::connect((char*)"127.0.0.1", 3306, (char*)"moba_game", (char*)"root", (char*)"Cxcz1902", on_mysql_open_cb);
}

static void test_redis() {
	redis_wrapper::connect((char*)"127.0.0.1", 6379, on_redis_open_cb);
}

int main(int argc, char** argv) {

	proto_man::init(PROTO_BUF);//google protoclbuf
	init_pf_cmd_map();
	logger::init((char*)"logger/gateway", (char*)"gateway", true);

	//test logger
	log_debug("��ǰʱ�����%d", timestamp());//��ǰʱ���
	log_debug("�����賿0��ʱ�����%d", timestamp_today());//�����賿0��ʱ���
	log_debug("ָ������ʱ��תʱ�����%d", date2timestamp("%Y-%m-%d %H:%M:%S", "2022-11-16 00:00:00"));//ָ������ʱ��תʱ���

	unsigned long yesterdy = timestamp_yesterday();
	char out_buf[64];
	timestamp2date(yesterdy, (char*)"%Y-%m-%d %H:%M:%S", out_buf, sizeof(out_buf));//ʱ���ת����ʱ��
	log_debug("ʱ���ת����ʱ�䣺%s", out_buf);
	//test logger end

	//schedule(on_logger_timer, NULL, 3000, -1);//��ʱ����ÿ3�����һ��on_logger_timer��������������

	netbus::instance()->init();
	netbus::instance()->tcp_listen(6080);//tcp
	netbus::instance()->udp_listen(8001);//udp
	netbus::instance()->ws_listen(8002);//websocket

	netbus::instance()->run();
	return 0;
}
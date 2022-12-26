#ifndef __SESSION_UV_H__
#define __SESSION_UV_H__
#define RECV_LEN 4096//接收常规数据的buf容量大小

//socket 类型
enum {
	TCP_SOCKET,
	WS_SOCKET,
};

class uv_session :public session {
public:
	uv_tcp_t tcp_handler;
	char c_address[32];
	int c_port;
	uv_shutdown_t shutdown;
	bool is_shutdown;//是否已经shutdown
	char recv_buf[RECV_LEN];//接收数据用的buf
	int recved;//接收到的数据大小
	int socket_type;//socket类型：tcp、web
	int is_ws_shake;//websocket 是否握手【0表示没有经过握手】
	char* long_pkg;//接收长数据包用的buf【接收的数据长度超过 RECV_LEN ，就用此长包接收】
	int long_pkg_size;//接收到的数据大小
	static uv_session* create();
	static void destroy(uv_session* s);
	virtual void close();
	virtual void send_data(unsigned char* body, int len);
	virtual const char* get_address(int* client_port);
	virtual void send_msg(struct cmd_msg* msg);
	virtual void send_raw_cmd(struct raw_cmd* raw);
	void* operator new(size_t size);
	void operator delete(void* mem);

private:
	void init();
	void exit();
};

void init_session_allocer();
#endif

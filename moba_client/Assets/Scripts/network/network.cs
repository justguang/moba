using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;

public class network : UnitySingleton<network>
{
    #region Tcp
    private string server_ip = "127.0.0.1";
    private int port = 6080;

    private Socket client_socket = null;
    private bool is_connect = false;
    private Thread recv_thread = null;
    private const int RECV_LEN = 8192;
    private byte[] recv_buf = new byte[RECV_LEN];
    private int recved;
    private byte[] long_pkg = null;
    private int long_pkg_size = 0;
    #endregion

    #region Udp  
    private string udp_server_ip = "127.0.0.1";
    private int udp_port = 8800;
    private IPEndPoint udp_remote_point = null;
    private Socket udp_socket = null;
    private Thread udp_recv_thread = null;
    private byte[] udp_recv_buf = new byte[60 * 1024];
    public int local_udp_port = 8888;
    #endregion

    //接收到的所有网络消息
    private Queue<cmd_msg> net_event = new Queue<cmd_msg>();
    //事件监听表 key:stype =》 value:callback
    private Dictionary<int, Action<cmd_msg>> event_listens = new Dictionary<int, Action<cmd_msg>>();


    // Use this for initialization
    void Start()
    {
        this.connect_to_server();
        this.udp_socket_init();

        //test udp
        //this.InvokeRepeating("test_udp", 5, 5);
        //end
    }

    void test_udp()
    {
        logic_service_proxy.Instance.send_udp_test("hello world！！！");
    }


    void OnDestroy()
    {
        this.close();
        this.udp_close();
    }

    void OnApplicationQuit()
    {
        this.close();
        this.udp_close();
    }

    // Update is called once per frame
    void Update()
    {
        lock (this.net_event)
        {
            while (this.net_event.Count > 0)
            {
                cmd_msg msg = this.net_event.Dequeue();
                if (this.event_listens.ContainsKey(msg.stype))
                {
                    this.event_listens[msg.stype](msg);
                }
            }
        }
    }

    void on_conntect_timeout()
    {
    }

    void on_connect_error(string err)
    {
    }

    void connect_to_server()
    {
        try
        {
            this.client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse(this.server_ip);
            IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, this.port);

            IAsyncResult result = this.client_socket.BeginConnect(ipEndpoint, new AsyncCallback(this.on_connected), this.client_socket);
            bool success = result.AsyncWaitHandle.WaitOne(5000, true);
            if (!success)
            { // timeout;
                this.on_conntect_timeout();
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            this.on_connect_error(e.ToString());
        }
    }

    void on_recv_cmd(byte[] data, int start, int data_len)
    {
        cmd_msg msg;
        proto_man.unpack_cmd_msg(data, start, data_len, out msg);
        if (msg != null)
        {
            //LoginRes res = proto_man.protobuf_deserialize<LoginRes>(msg.body);
            //Debug.Log("########### res=" + res.Status);
            lock (this.net_event)
            {
                this.net_event.Enqueue(msg);
            }
        }
    }

    void on_recv_tcp_data()
    {
        byte[] pkg_data = (this.long_pkg != null) ? this.long_pkg : this.recv_buf;
        while (this.recved > 0)
        {
            int pkg_size = 0;
            int head_size = 0;
            if (!tcp_packer.read_header(pkg_data, this.recved, out pkg_size, out head_size))
            {
                break;
            }

            if (this.recved < pkg_size)
            {
                break;
            }

            int raw_data_start = head_size;
            int raw_data_len = pkg_size - head_size;
            on_recv_cmd(pkg_data, raw_data_start, raw_data_len);

            if (this.recved > pkg_size)
            {
                this.recv_buf = new byte[RECV_LEN];
                Array.Copy(pkg_data, pkg_size, this.recv_buf, 0, this.recved - pkg_size);
                pkg_data = this.recv_buf;
            }
            this.recved -= pkg_size;
            if (this.recved == 0 && this.long_pkg != null)
            {
                //清理长包
                this.long_pkg = null;
                this.long_pkg_size = 0;
            }
        }
    }
    void thread_recv_worker()
    {
        if (this.is_connect == false)
        {
            return;
        }

        while (true)
        {
            if (!this.client_socket.Connected)
            {
                break;
            }

            try
            {
                int recv_len = 0;
                if (this.recved < RECV_LEN)
                {
                    //接收正常tcp数据包
                    recv_len = this.client_socket.Receive(this.recv_buf, this.recved, RECV_LEN - this.recved, SocketFlags.None);
                }
                else
                {
                    //接收长数据包
                    if (this.long_pkg == null)
                    {
                        int pkg_size;
                        int head_size;
                        tcp_packer.read_header(this.recv_buf, this.recved, out pkg_size, out head_size);
                        this.long_pkg_size = pkg_size;
                        this.long_pkg = new byte[pkg_size];
                        Array.Copy(this.recv_buf, 0, this.long_pkg, 0, this.recved);
                    }
                    recv_len = this.client_socket.Receive(this.long_pkg, this.recved, this.long_pkg_size - this.recved, SocketFlags.None);
                }

                if (recv_len > 0)
                {
                    this.recved += recv_len;
                    this.on_recv_tcp_data();
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
                this.client_socket.Disconnect(true);
                this.client_socket.Shutdown(SocketShutdown.Both);
                this.client_socket.Close();
                this.is_connect = false;
                break;
            }
        }
    }

    void on_connected(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket)iar.AsyncState;
            client.EndConnect(iar);

            this.is_connect = true;
            this.recv_thread = new Thread(new ThreadStart(this.thread_recv_worker));
            this.recv_thread.Start();

            Debug.Log("connect to server success" + this.server_ip + ":" + this.port + "!");
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            this.on_connect_error(e.ToString());
            this.is_connect = false;
        }
    }

    void close()
    {
        if (!this.is_connect)
        {
            return;
        }

        this.is_connect = false;

        // abort recv thread
        if (this.recv_thread != null)
        {
            this.recv_thread.Interrupt();
            this.recv_thread.Abort();
            this.recv_thread = null;
        }
        // end

        if (this.client_socket != null && this.client_socket.Connected)
        {
            this.client_socket.Close();
            this.client_socket = null;
        }
    }

    //发送网络消息的回调
    void on_send_data(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket)iar.AsyncState;
            client.EndSend(iar);

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    /// <summary>
    /// 发送一个protobuf消息
    /// </summary>
    /// <param name="stype">服务号</param>
    /// <param name="ctype">命令号</param>
    /// <param name="body">protobuf消息本体</param>
    public void send_protobuf_cmd(int stype, int ctype, IMessage body)
    {
        byte[] cmd_data = proto_man.pack_protobuf_cmd(stype, ctype, body);
        if (cmd_data == null)
        {
            return;
        }
        byte[] tcp_pkg = tcp_packer.pack(cmd_data);
        this.client_socket.BeginSend(tcp_pkg, 0, tcp_pkg.Length, SocketFlags.None, new AsyncCallback(this.on_send_data), this.client_socket);
    }

    /// <summary>
    /// 发送json消息
    /// </summary>
    /// <param name="stype">服务号</param>
    /// <param name="ctype">命令号</param>
    /// <param name="json_body">json字符串</param>
    public void send_json_cmd(int stype, int ctype, string json_body)
    {
        byte[] cmd_data = proto_man.pack_json_cmd(stype, ctype, json_body);
        if (cmd_data == null)
        {
            return;
        }
        byte[] tcp_pkg = tcp_packer.pack(cmd_data);
        this.client_socket.BeginSend(tcp_pkg, 0, tcp_pkg.Length, SocketFlags.None, new AsyncCallback(this.on_send_data), this.client_socket);
    }

    /// <summary>
    /// add a event listen
    /// </summary>
    /// <param name="stype">event code</param>
    /// <param name="handler">callback</param>
    public void add_service_listener(int stype, Action<cmd_msg> handler)
    {
        if (this.event_listens.ContainsKey(stype))
        {
            this.event_listens[stype] += handler;
        }
        else
        {
            this.event_listens.Add(stype, handler);
        }
    }

    /// <summary>
    /// delete a event listen
    /// </summary>
    /// <param name="stype">event code</param>
    /// <param name="handler">callback</param>
    public void remove_service_listener(int stype, Action<cmd_msg> handler)
    {
        if (this.event_listens.ContainsKey(stype))
        {
            this.event_listens[stype] -= handler;
            if (this.event_listens[stype] == null)
            {
                this.event_listens.Remove(stype);
            }
        }
    }


    void udp_close()
    {

        // abort recv thread
        if (this.udp_recv_thread != null)
        {
            this.udp_recv_thread.Interrupt();
            this.udp_recv_thread.Abort();
            this.udp_recv_thread = null;
        }
        // end

        if (this.udp_socket != null)
        {
            this.udp_socket.Close();
            this.udp_socket = null;
        }
    }

    void udp_thread_recv_worker()
    {
        while (true)
        {
            EndPoint remote = this.udp_remote_point;
            //Debug.Log("begin udp recv...");
            int recved = this.udp_socket.ReceiveFrom(this.udp_recv_buf, ref remote);
            //Debug.Log("end udp recv!!!");
            this.on_recv_cmd(this.udp_recv_buf, 0, recved);
        }
    }

    void udp_socket_init()
    {
        //udp 远程端
        this.udp_remote_point = new IPEndPoint(IPAddress.Parse(this.udp_server_ip), this.udp_port);

        //new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        this.udp_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //绑定本机收数据端
        IPEndPoint local_point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.local_udp_port);
        this.udp_socket.Bind(local_point);

        //启动线程收数据
        this.udp_recv_thread = new Thread(new ThreadStart(this.udp_thread_recv_worker));
        this.udp_recv_thread.Start();
    }

    void on_udp_send_data(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket)iar.AsyncState;
            client.EndSendTo(iar);

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void udp_send_protobuf_cmd(int stype, int ctype, IMessage body)
    {
        byte[] cmd_data = proto_man.pack_protobuf_cmd(stype, ctype, body);
        if (cmd_data == null)
        {
            return;
        }
        this.udp_socket.BeginSendTo(cmd_data, 0, cmd_data.Length, SocketFlags.None, this.udp_remote_point, new AsyncCallback(this.on_udp_send_data), this.udp_socket);
    }
}

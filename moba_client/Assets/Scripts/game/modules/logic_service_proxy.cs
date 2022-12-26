using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_service_proxy : Singleton<logic_service_proxy>
{
    void on_login_logic_server_return(cmd_msg msg)
    {
        LoginLogicRes res = proto_man.protobuf_deserialize<LoginLogicRes>(msg.body);
        if (res == null) return;
        if (res.Status != Response.OK)
        {
            Debug.Log("login logic server failed. status = " + res.Status);
            return;
        }

        //登录logic服务器成功
        event_manager.Instance.dispatch_event("login_logic_server", null);
    }

    void on_enter_zone_return(cmd_msg msg)
    {
        EnterZoneRes res = proto_man.protobuf_deserialize<EnterZoneRes>(msg.body);
        if (res == null) return;
        if (res.Status != Response.OK)
        {
            Debug.Log("enter zone failed. status = " + res.Status);
            return;
        }

        Debug.Log("enter zone success.");
    }

    void on_enter_match_return(cmd_msg msg)
    {
        EnterMatch res = proto_man.protobuf_deserialize<EnterMatch>(msg.body);
        if (res == null) return;
        //Debug.Log("enter success zid[" + res.Zid + "]  matchid[" + res.Matchid + "]");
        ugame.Instance.zid = res.Zid;
        ugame.Instance.matchid = res.Matchid;
        ugame.Instance.self_seatid = res.Seatid;
        ugame.Instance.self_side = res.Side;
    }

    void on_user_arrived_return(cmd_msg msg)
    {
        UserArrived res = proto_man.protobuf_deserialize<UserArrived>(msg.body);
        if (res == null) return;

        Debug.Log(res.Unick + " user arrived !!!");
        ugame.Instance.other_users.Add(res);
        event_manager.Instance.dispatch_event("user_arrived", res);
    }

    void on_exit_match_return(cmd_msg msg)
    {
        ExitMatchRes res = proto_man.protobuf_deserialize<ExitMatchRes>(msg.body);
        if (res == null) return;
        if (res.Status != Response.OK)
        {
            Debug.Log("exit match failed. status = " + res.Status);
            return;
        }

        event_manager.Instance.dispatch_event("exit_match", null);
    }

    void on_other_user_exit_match(cmd_msg msg)
    {
        UserExitMatch res = proto_man.protobuf_deserialize<UserExitMatch>(msg.body);
        if (res == null) return;
        for (int i = 0; i < ugame.Instance.other_users.Count; i++)
        {
            if (ugame.Instance.other_users[i].Seatid == res.Seatid)
            {
                ugame.Instance.other_users.RemoveAt(i);
                event_manager.Instance.dispatch_event("other_user_exit_match", i);
                return;
            }
        }
    }

    void on_game_start(cmd_msg msg)
    {
        GameStart res = proto_man.protobuf_deserialize<GameStart>(msg.body);
        if (res == null) return;

        ugame.Instance.players_match_info = res.PlayersMatchInfo;
        event_manager.Instance.dispatch_event("game_start", null);
    }

    void on_udp_test(cmd_msg msg)
    {
        UdpTest res = proto_man.protobuf_deserialize<UdpTest>(msg.body);
        if (res == null) return;
        Debug.Log("udp test server return: " + res.Content);
    }

    void on_server_logic_frame(cmd_msg msg)
    {
        LogicFrame res = proto_man.protobuf_deserialize<LogicFrame>(msg.body);
        if (res == null) return;

        //Debug.Log("server return frameid=" + res.Frameid);//当前帧ID，以及玩家没有同步的操作
        event_manager.Instance.dispatch_event("on_logic_update", res);
    }

    void on_logic_server_return(cmd_msg msg)
    {
        switch (msg.ctype)
        {
            case (int)Cmd.ELoginLogicRes:
                this.on_login_logic_server_return(msg);
                break;
            case (int)Cmd.EEnterZoneRes:
                this.on_enter_zone_return(msg);
                break;
            case (int)Cmd.EEnterMatch:
                this.on_enter_match_return(msg);
                break;
            case (int)Cmd.EUserArrived:
                this.on_user_arrived_return(msg);
                break;
            case (int)Cmd.EExitMatchRes:
                this.on_exit_match_return(msg);
                break;
            case (int)Cmd.EUserExitMatch:
                this.on_other_user_exit_match(msg);
                break;
            case (int)Cmd.EGameStart:
                this.on_game_start(msg);
                break;
            case (int)Cmd.EUdpTest:
                this.on_udp_test(msg);
                break;
            case (int)Cmd.ELogicFrame:
                this.on_server_logic_frame(msg);
                break;
        }
    }

    public void init()
    {
        network.Instance.add_service_listener((int)Stype.Logic, this.on_logic_server_return);
    }

    public void login_logic_server()
    {
        LoginLogicReq req = new LoginLogicReq { UdpIp = "127.0.0.1", UdpPort = network.Instance.local_udp_port };
        network.Instance.send_protobuf_cmd((int)Stype.Logic, (int)Cmd.ELoginLogicReq, req);
    }

    public void enter_zone(int zid)
    {
        if (zid != Zone.SGDY && zid != Zone.ASSY) return;
        EnterZoneReq req = new EnterZoneReq { Zid = zid, };
        network.Instance.send_protobuf_cmd((int)Stype.Logic, (int)Cmd.EEnterZoneReq, req);
    }

    public void exit_match()
    {
        network.Instance.send_protobuf_cmd((int)Stype.Logic, (int)Cmd.EExitMatchReq, null);
    }

    public void send_udp_test(string conten)
    {
        UdpTest req = new UdpTest { Content = conten };
        network.Instance.udp_send_protobuf_cmd((int)Stype.Logic, (int)Cmd.EUdpTest, req);
    }

    public void send_next_frame_opts(NextFrameOpts next_frame)
    {
        network.Instance.udp_send_protobuf_cmd((int)Stype.Logic, (int)Cmd.ENextFrameOpts, next_frame);
    }
}

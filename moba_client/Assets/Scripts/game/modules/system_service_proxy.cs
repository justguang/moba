using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class system_service_proxy : Singleton<system_service_proxy>
{
    private int ver_num = 0;
    private IList<string> sys_msgs = null;

    public void init()
    {
        network.Instance.add_service_listener((int)Stype.System, this.on_system_server_return);
    }

    void on_get_ugame_info_return(cmd_msg msg)
    {
        GetUgameInfoRes res = proto_man.protobuf_deserialize<GetUgameInfoRes>(msg.body);
        if (res == null) return;
        if (res.Status != Response.OK)
        {
            Debug.LogError("get ugame info failed. status: " + res.Status);
            return;
        }

        UserGameInfo uinfo = res.Uinfo;
        ugame.Instance.save_uinfo(uinfo);

        event_manager.Instance.dispatch_event("get_ugame_info_success", null);
        event_manager.Instance.dispatch_event("sync_ugame_info", null);
    }

    void on_recv_login_bonues_return(cmd_msg msg)
    {
        RecvLoginBonuesRes res = proto_man.protobuf_deserialize<RecvLoginBonuesRes>(msg.body);
        if (res == null) return;
        if (res.Status != Response.OK)
        {
            Debug.LogError("recv login bonues failed. status: " + res.Status);
            return;
        }

        ugame.Instance.ugame_info.Uchip += ugame.Instance.ugame_info.Bonues;
        ugame.Instance.ugame_info.BonuesStatus = 1;
        event_manager.Instance.dispatch_event("sync_ugame_info", null);
    }

    void on_get_world_uchip_rank_return(cmd_msg msg)
    {
        GetWorldRankUchipRes res = proto_man.protobuf_deserialize<GetWorldRankUchipRes>(msg.body);
        if (res == null) return;
        if (res.Status != Response.OK)
        {
            Debug.LogError("get world uchip rank failed. status: " + res.Status);
            return;
        }

        IList<WorldChipRankInfo> lst = res.RankInfo;
        for (int i = 0; i < lst.Count; i++)
        {
            Debug.Log(lst[i].Unick + " : " + lst[i].Uchip);
        }
        event_manager.Instance.dispatch_event("get_rank_list", res.RankInfo);
    }

    void on_get_sys_msg_return(cmd_msg msg)
    {
        GetSysMsgRes res = proto_man.protobuf_deserialize<GetSysMsgRes>(msg.body);
        if (res == null)
        {
            return;
        }

        if (res.Status != Response.OK)
        {
            Debug.Log("get system msg status: " + res.Status);
            return;
        }

        Debug.Log("get system msg success");
        if (this.ver_num == res.VerNum)
        { // 本地和服务器的一样，使用本地的数据;
            Debug.Log("the use local data");
        }
        else
        {
            this.ver_num = res.VerNum;
            this.sys_msgs = res.SysMsgs;
            Debug.Log("sync server data");
        }

        if (this.sys_msgs != null)
        {
            for (int i = 0; i < this.sys_msgs.Count; i++)
            {
                Debug.Log(this.sys_msgs[i]);
            }
            event_manager.Instance.dispatch_event("get_sys_email", this.sys_msgs);
        }
    }

    void on_system_server_return(cmd_msg msg)
    {
        switch (msg.ctype)
        {
            case (int)Cmd.EGetUgameInfoRes:
                this.on_get_ugame_info_return(msg);
                break;
            case (int)Cmd.ERecvLoginBonuesRes:
                this.on_recv_login_bonues_return(msg);
                break;
            case (int)Cmd.EGetWorldRankUchipRes:
                this.on_get_world_uchip_rank_return(msg);
                break;
            case (int)Cmd.EGetSysMsgRes:
                this.on_get_sys_msg_return(msg);
                break;
        }
    }


    public void load_user_ugame_info()
    {
        network.Instance.send_protobuf_cmd((int)Stype.System, (int)Cmd.EGetUgameInfoReq, null);
    }

    public void recv_login_bonues()
    {
        network.Instance.send_protobuf_cmd((int)Stype.System, (int)Cmd.ERecvLoginBonuesReq, null);
    }

    public void get_world_uchip_rank_info()
    {
        network.Instance.send_protobuf_cmd((int)Stype.System, (int)Cmd.EGetWorldRankUchipReq, null);
    }

    public void get_sys_msg()
    {
        GetSysMsgReq req = new GetSysMsgReq { VerNum = 0, };
        network.Instance.send_protobuf_cmd((int)Stype.System, (int)Cmd.EGetSysMsgReq, req);
    }
}

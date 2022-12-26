using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class user_info
{
    public string unick;
    public int usex;
    public int uface;
}

public class ugame : Singleton<ugame>
{
    public bool is_guest = false;//true 游客
    public string unick = "";
    public int uface = 1;
    public int usex = 0;
    public int uvip = 0;
    public string guest_key = "";

    public int zid = -1;//哪个区间【地图】
    public int matchid = -1;//哪场比赛
    public int self_seatid = -1;//本机玩家座位号
    public int self_side = -1;//本机玩家阵营【left， right】

    public UserGameInfo ugame_info;

    public List<UserArrived> other_users = new List<UserArrived>();
    public IList<PlayerMatchInfo> players_match_info = null;//比赛玩家的信息

    public PlayerMatchInfo get_player_match_info(int seatid)
    {
        int player_count = this.players_match_info.Count;
        for (int i = 0; i < player_count; i++)
        {
            if (this.players_match_info[i].Seatid == seatid)
            {
                return this.players_match_info[i];
            }
        }
        return null;
    }

    public user_info get_user_info(int seatid)
    {
        user_info uinfo = new user_info();
        if (seatid == self_seatid)
        {
            uinfo.unick = this.unick;
            uinfo.usex = this.usex;
            uinfo.uface = this.uface;
            return uinfo;
        }
        else
        {
            int user_count = this.other_users.Count;
            for (int i = 0; i < user_count; i++)
            {
                if (this.other_users[i].Seatid == seatid)
                {
                    uinfo.unick = this.other_users[i].Unick;
                    uinfo.usex = this.other_users[i].Usex;
                    uinfo.uface = this.other_users[i].Uface;
                    return uinfo;
                }
            }
        }
        return null;
    }

    public void save_uinfo(UserCenterInfo uinfo, bool is_guest, string guest_key = "")
    {
        this.unick = uinfo.Unick;
        this.uface = uinfo.Uface;
        this.usex = uinfo.Usex;
        this.uvip = uinfo.Uvip;
        this.is_guest = is_guest;
        this.guest_key = guest_key;
    }

    public void save_uinfo(UserGameInfo ugame_info)
    {
        this.ugame_info = ugame_info;
    }

    public void save_uinfo(string unick, int uface, int usex)
    {
        this.unick = unick;
        this.uface = uface;
        this.usex = usex;
    }

    public void user_login_out()
    {
        //登出账号
    }
}

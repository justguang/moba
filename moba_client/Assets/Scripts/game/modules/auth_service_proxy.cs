using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class auth_service_proxy : Singleton<auth_service_proxy>
{
    private string g_key = null;
    private bool is_save_gkey = false;
    private EditProfileReq tmp_edit_profile_req = null;

    public void init()
    {
        network.Instance.add_service_listener((int)Stype.Auth, this.on_auth_server_return);
    }

    void on_guest_login_return(cmd_msg msg)
    {
        GuestLoginRes res = proto_man.protobuf_deserialize<GuestLoginRes>(msg.body);
        if (res == null)
        {
            return;
        }

        if (res.Status != Response.OK)
        {
            Debug.LogError("Guest Login Status : " + res.Status);
            return;
        }

        if (this.is_save_gkey)
        {
            PlayerPrefs.SetString("justguang_guest_key", this.g_key);
        }

        //登录成功
        UserCenterInfo uinfo = res.Uinfo;
        ugame.Instance.save_uinfo(uinfo, true, this.g_key);

        event_manager.Instance.dispatch_event("login_success", null);
        event_manager.Instance.dispatch_event("sync_uinfo", null);
    }

    void on_edit_profile_return(cmd_msg msg)
    {
        EditProfileRes res = proto_man.protobuf_deserialize<EditProfileRes>(msg.body);
        if (res == null) return;

        if (res.Status != Response.OK)
        {
            Debug.LogError("edit profile error. status: " + res.Status);
            return;
        }

        Debug.Log("edit profile success. status: " + res.Status);
        ugame.Instance.save_uinfo(tmp_edit_profile_req.Unick, tmp_edit_profile_req.Uface, tmp_edit_profile_req.Usex);
        this.tmp_edit_profile_req = null;
        event_manager.Instance.dispatch_event("sync_uinfo", null);
    }

    void on_account_upgrade_return(cmd_msg msg)
    {
        AccountUpgradeRes res = proto_man.protobuf_deserialize<AccountUpgradeRes>(msg.body);
        if (res == null) return;
        if (res.Status != Response.OK)
        {
            Debug.LogError("guest account upgrade failed. " + res.Status);
            return;
        }

        //游客成功升级为账号
        PlayerPrefs.DeleteKey("justguang_guest_key");
        ugame.Instance.is_guest = false;
        event_manager.Instance.dispatch_event("upgrade_account_return", res.Status);
    }

    void on_uname_login_return(cmd_msg msg)
    {
        UnameLoginRes res = proto_man.protobuf_deserialize<UnameLoginRes>(msg.body);
        if (res == null) return;
        if (res.Status != Response.OK)
        {
            Debug.LogError("uname login failed. status: " + res.Status);
            return;
        }

        UserCenterInfo uinfo = res.Uinfo;
        ugame.Instance.save_uinfo(uinfo, false);

        event_manager.Instance.dispatch_event("login_success", null);
        event_manager.Instance.dispatch_event("sync_uinfo", null);
    }

    void on_user_login_out_return(cmd_msg msg)
    {
        LoginOutRes res = proto_man.protobuf_deserialize<LoginOutRes>(msg.body);
        if (res == null) return;
        if (res.Status != Response.OK)
        {
            Debug.LogError("user login out failed. status: " + res.Status);
            return;
        }

        //登出账号 成功
        ugame.Instance.user_login_out();
        //end

        event_manager.Instance.dispatch_event("user_login_out", null);
    }

    void on_auth_server_return(cmd_msg msg)
    {
        switch (msg.ctype)
        {
            case (int)Cmd.EGuestLoginRes:
                on_guest_login_return(msg);
                break;
            case (int)Cmd.EEditProfileRes:
                on_edit_profile_return(msg);
                break;
            case (int)Cmd.EAccountUpgradeRes:
                on_account_upgrade_return(msg);
                break;
            case (int)Cmd.EUnameLoginRes:
                on_uname_login_return(msg);
                break;
            case (int)Cmd.ELoginOutRes:
                on_user_login_out_return(msg);
                break;
        }
    }

    public void guest_login()
    {
        this.g_key = PlayerPrefs.GetString("justguang_guest_key");
        //this.g_key = null;
        this.is_save_gkey = false;
        if (string.IsNullOrWhiteSpace(this.g_key) || this.g_key.Length != 32)
        {
            this.g_key = utils.rand_str(32);
            Debug.Log("guest login request. rand g_key=" + this.g_key);
            //this.g_key = "DIt3t8ThehliWldQAgVrkdLjpwzCNUXR";//for test
            this.is_save_gkey = true;
        }

        GuestLoginReq req = new GuestLoginReq
        {
            GuestKey = this.g_key
        };
        network.Instance.send_protobuf_cmd((int)Stype.Auth, (int)Cmd.EGuestLoginReq, req);
    }

    public void uname_login(string uname, string upwd)
    {
        string upwd_md5 = utils.md5(upwd);
        //Debug.Log("uname :" + uname + "  upwd: " + upwd + " upwd_md5:" + upwd_md5);

        UnameLoginReq req = new UnameLoginReq
        {
            Uname = uname,
            Upwd = upwd_md5,
        };
        network.Instance.send_protobuf_cmd((int)Stype.Auth, (int)Cmd.EUnameLoginReq, req);
    }

    public void edit_profile(string unick, int uface, int usex)
    {
        if (string.IsNullOrWhiteSpace(unick)
            || uface <= 0
            || uface > 9
            || (usex != 0 && usex != 1))
        {
            return;
        }

        EditProfileReq req = new EditProfileReq
        {
            Unick = unick,
            Uface = uface,
            Usex = usex,
        };
        this.tmp_edit_profile_req = req;
        network.Instance.send_protobuf_cmd((int)Stype.Auth, (int)Cmd.EEditProfileReq, req);
    }

    public void ao_account_upgrade(string uname, string upwd_md5)
    {
        AccountUpgradeReq req = new AccountUpgradeReq
        {
            Uname = uname,
            UpwdMd5 = upwd_md5,
        };

        network.Instance.send_protobuf_cmd((int)Stype.Auth, (int)Cmd.EAccountUpgradeReq, req);
    }

    public void user_login_out()
    {
        network.Instance.send_protobuf_cmd((int)Stype.Auth, (int)Cmd.ELoginOutReq, null);
    }
}

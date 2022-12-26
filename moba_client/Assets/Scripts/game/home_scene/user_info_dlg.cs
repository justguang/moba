using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class user_info_dlg : MonoBehaviour
{
    [SerializeField] private InputField unick_edit;
    [SerializeField] private Transform guest_upgrade;
    [SerializeField] private Image uface_img;
    [SerializeField] private Sprite[] uface_res;
    [SerializeField] private Transform man_check;
    [SerializeField] private Transform woman_check;
    [SerializeField] private Transform uface_select_dlg;
    [SerializeField] private Transform account_upgrade;
    [SerializeField] private InputField uname_edit;
    [SerializeField] private InputField upwd_edit;
    [SerializeField] private InputField upwd_again_edit;

    private int usex = 0;
    private int uface = 1;

    void Start()
    {
        if (this.guest_upgrade != null) this.guest_upgrade.gameObject.SetActive(ugame.Instance.is_guest);

        if (this.unick_edit != null && !string.IsNullOrWhiteSpace(ugame.Instance.unick)) this.unick_edit.text = ugame.Instance.unick;

        if (ugame.Instance.uface > 0 && ugame.Instance.uface < 10) this.uface = ugame.Instance.uface;
        if (this.uface_img != null) this.uface_img.sprite = this.uface_res[this.uface - 1];

        if (ugame.Instance.usex == 0 || ugame.Instance.usex == 1) this.usex = ugame.Instance.usex;
        if (this.man_check != null) this.man_check.gameObject.SetActive(this.usex == 0);
        if (this.woman_check != null) this.woman_check.gameObject.SetActive(this.usex == 1);

        //监听事件
        event_manager.Instance.add_event_listener("upgrade_account_return", this.on_upgrade_account_return);
        //end
    }

    void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("upgrade_account_return", this.on_upgrade_account_return);
    }

    void on_upgrade_account_return(string event_name, object udata)
    {
        int status = (int)udata;
        Debug.Log("upgrade account return status: " + status);
        if (status == Response.OK)
        {
            this.on_hide_account_upgrade();
            if (this.guest_upgrade != null) this.guest_upgrade.gameObject.SetActive(false);
        }
    }


    public void on_show_account_upgrade()
    {
        if (this.account_upgrade != null) this.account_upgrade.gameObject.SetActive(true);
    }
    public void on_hide_account_upgrade()
    {
        if (this.account_upgrade != null) this.account_upgrade.gameObject.SetActive(false);
    }

    public void on_do_account_upgrade()
    {
        if (!ugame.Instance.is_guest) return;
        if (string.IsNullOrWhiteSpace(this.uname_edit.text)
            || string.IsNullOrWhiteSpace(this.upwd_edit.text)
            || !this.upwd_edit.text.Equals(this.upwd_again_edit.text))
        {
            return;
        }

        string md5_pwd = utils.md5(this.upwd_edit.text);
        auth_service_proxy.Instance.ao_account_upgrade(this.uname_edit.text, md5_pwd);
    }

    public void on_usex_change(int target_usex)
    {
        this.usex = target_usex;
        if (this.man_check != null) this.man_check.gameObject.SetActive(this.usex == 0);
        if (this.woman_check != null) this.woman_check.gameObject.SetActive(this.usex == 1);
    }

    public void on_uface_click_show()
    {
        if (this.uface_select_dlg != null) this.uface_select_dlg.gameObject.SetActive(true);
    }
    public void on_uface_click_hide()
    {
        if (this.uface_select_dlg != null) this.uface_select_dlg.gameObject.SetActive(false);
    }

    public void on_uface_changed(int target_face)
    {
        this.uface = target_face;
        if (this.uface_img != null) this.uface_img.sprite = this.uface_res[this.uface - 1];
        if (this.uface_select_dlg != null) this.uface_select_dlg.gameObject.SetActive(false);
    }

    public void on_uinfo_dlg_click_hide()
    {
        //this.gameObject.SetActive(false);
        GameObject.Destroy(this.gameObject);
    }

    public void on_edit_profile_commit()
    {
        if (string.IsNullOrWhiteSpace(this.unick_edit.text))
        {
            this.on_uinfo_dlg_click_hide();
            return;
        }

        //提交修改资料求情给服务器
        //Debug.Log("unick : " + this.unick_edit.text);
        //Debug.Log("uface : " + this.uface);
        Debug.Log("usex : " + this.usex);

        auth_service_proxy.Instance.edit_profile(this.unick_edit.text, this.uface, this.usex);
        this.on_uinfo_dlg_click_hide();
    }

    public void on_click_user_login_out()
    {
        auth_service_proxy.Instance.user_login_out();
        this.on_uinfo_dlg_click_hide();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class home_scene : MonoBehaviour
{
    [SerializeField] private Text unick;
    [SerializeField] private Image uface;
    [SerializeField] private Text uchip_txt;
    [SerializeField] private Text diamond_txt;
    [SerializeField] private Text ulevel_txt;
    [SerializeField] private Text express_txt;
    [SerializeField] private Image express_img;
    [SerializeField] private Transform home_page;
    [SerializeField] private Transform war_page;
    [SerializeField] private Transform loading_page;
    [SerializeField] private Image[] tab_btn_imgs;

    [SerializeField] private Sprite[] normal_btn;
    [SerializeField] private Sprite[] hightlight_btn;
    [SerializeField] private Sprite[] uface_res;
    [SerializeField] private GameObject uinfo_dlg_prefab;
    [SerializeField] private GameObject login_bonues_prefab;
    [SerializeField] private GameObject rank_list_prefab;
    [SerializeField] private GameObject email_list_prefab;
    [SerializeField] private GameObject team_match_prefab;

    public void on_uinfo_show()
    {
        GameObject uinfo_dlg = GameObject.Instantiate(this.uinfo_dlg_prefab);
        uinfo_dlg.transform.SetParent(this.transform, false);
    }

    public void on_click_sgyd()
    {
        GameObject match_dlg = GameObject.Instantiate(this.team_match_prefab);
        match_dlg.transform.SetParent(this.transform, false);
        ugame.Instance.zid = Zone.SGDY;
    }

    public void on_click_get_rank()
    {
        GameObject rank_list = GameObject.Instantiate(this.rank_list_prefab);
        rank_list.transform.SetParent(this.transform, false);
    }

    public void on_click_get_sys_msg()
    {
        GameObject sys_email = GameObject.Instantiate(this.email_list_prefab);
        sys_email.transform.SetParent(this.transform, false);
    }
    public void on_click_login_bonues()
    {
        GameObject login_bonues = GameObject.Instantiate(this.login_bonues_prefab);
        //有登录奖励待领取
        login_bonues.SetActive(true);
        login_bonues.GetComponent<login_bonues>().show_login_bonues(ugame.Instance.ugame_info.Days);
        login_bonues.transform.SetParent(this.transform, false);
    }

    public void on_click_home_page()
    {
        this.home_page.gameObject.SetActive(true);
        this.war_page.gameObject.SetActive(false);
        this.tab_btn_imgs[0].sprite = this.hightlight_btn[0];
        this.tab_btn_imgs[1].sprite = this.normal_btn[1];
    }
    public void on_click_war_page()
    {
        this.home_page.gameObject.SetActive(false);
        this.war_page.gameObject.SetActive(true);
        this.tab_btn_imgs[0].sprite = this.normal_btn[0];
        this.tab_btn_imgs[1].sprite = this.hightlight_btn[1];
    }

    void sync_uinfo(string event_name, object udata)
    {
        //同步user data
        if (this.unick != null) this.unick.text = ugame.Instance.unick;

        if (this.uface != null) this.uface.sprite = uface_res[ugame.Instance.uface - 1];
    }

    void sync_ugame_info(string event_name, object udata)
    {
        if (this.uchip_txt != null) this.uchip_txt.text = ugame.Instance.ugame_info.Uchip.ToString();

        if (this.diamond_txt != null) this.diamond_txt.text = ugame.Instance.ugame_info.Uchip2.ToString();

        //计算等级信息并显示
        int now_level_exp;
        int next_level_exp;
        int level = ulevel.Instance.get_level_info(ugame.Instance.ugame_info.Uexp, out now_level_exp, out next_level_exp);
        if (this.ulevel_txt != null)
        {
            this.ulevel_txt.text = "LV\n" + level;
        }
        if (this.express_txt != null)
        {
            this.express_txt.text = now_level_exp + " / " + next_level_exp;
        }
        if (this.express_img != null)
        {
            this.express_img.fillAmount = (float)now_level_exp / (float)next_level_exp;
        }

        //同步登录奖励信息
        if (ugame.Instance.ugame_info.BonuesStatus == 0)
        {
            GameObject login_bonues=  GameObject.Instantiate(this.login_bonues_prefab);
            //有登录奖励待领取
            login_bonues.SetActive(true);
            login_bonues.GetComponent<login_bonues>().show_login_bonues(ugame.Instance.ugame_info.Days);
            login_bonues.transform.SetParent(this.transform,false);
        }
    }

    void on_user_login_out(string event_name, object udata)
    {
        SceneManager.LoadScene("login");
    }

    void on_game_start(string event_name, object udata)
    {
        Debug.Log("game start !!!");
        this.loading_page.gameObject.SetActive(true);//显示加载页面
    }

    void Start()
    {
        event_manager.Instance.add_event_listener("sync_uinfo", this.sync_uinfo);
        event_manager.Instance.add_event_listener("user_login_out", this.on_user_login_out);
        event_manager.Instance.add_event_listener("sync_ugame_info", this.sync_ugame_info);
        event_manager.Instance.add_event_listener("game_start", this.on_game_start);
        this.on_click_home_page();
        this.sync_uinfo("sync_uinfo", null);
        this.sync_ugame_info("sync_ugame_info", null);

    }

    void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("sync_uinfo", this.sync_uinfo);
        event_manager.Instance.remove_event_listener("user_login_out", this.on_user_login_out);
        event_manager.Instance.remove_event_listener("sync_ugame_info", this.sync_ugame_info);
        event_manager.Instance.remove_event_listener("game_start", this.on_game_start);
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class team_match : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollview;
    [SerializeField] private GameObject opt_prefab;
    [SerializeField] private Sprite[] uface_img;

    private int member_count;

    void on_user_arrived(string event_name, object udata)
    {
        UserArrived user_info = (UserArrived)udata;
        this.member_count++;

        GameObject user = GameObject.Instantiate(opt_prefab);
        user.transform.SetParent(this.scrollview.content, false);
        this.scrollview.content.sizeDelta = new Vector2(0, this.member_count * 106f);

        user.transform.Find("name").GetComponent<Text>().text = user_info.Unick;
        user.transform.Find("header/avator").GetComponent<Image>().sprite = uface_img[user_info.Uface - 1];
        user.transform.Find("sex").GetComponent<Text>().text = user_info.Usex == 0 ? "男" : "女";
    }

    void on_self_exit_match(string event_name, object udata)
    {
        ugame.Instance.zid = -1;
        GameObject.Destroy(this.gameObject);
    }

    void on_other_user_exit_match(string event_name, object udata)
    {
        int index = (int)udata;
        this.member_count--;
        GameObject.Destroy(this.scrollview.content.GetChild(index).gameObject);
        this.scrollview.content.sizeDelta = new Vector2(0, this.member_count * 106f);
    }


    void on_game_start(string event_name, object udata)
    {
        GameObject.Destroy(this.gameObject);
    }

    void Start()
    {
        event_manager.Instance.add_event_listener("user_arrived", this.on_user_arrived);
        event_manager.Instance.add_event_listener("exit_match", this.on_self_exit_match);
        event_manager.Instance.add_event_listener("other_user_exit_match", this.on_other_user_exit_match);
        event_manager.Instance.add_event_listener("game_start", this.on_game_start);
    }

    void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("user_arrived", this.on_user_arrived);
        event_manager.Instance.remove_event_listener("exit_match", this.on_self_exit_match);
        event_manager.Instance.remove_event_listener("other_user_exit_match", this.on_other_user_exit_match);
        event_manager.Instance.remove_event_listener("game_start", this.on_game_start);
    }

    public void on_click_begin_match()
    {
        int zid = ugame.Instance.zid;
        logic_service_proxy.Instance.enter_zone(zid);
    }

    public void on_click_exit_match()
    {
        logic_service_proxy.Instance.exit_match();
    }

}



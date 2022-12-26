using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class login_scene : MonoBehaviour
{
    [SerializeField] private InputField uname_edit;
    [SerializeField] private InputField upwd_edit;

    void Start()
    {
        event_manager.Instance.add_event_listener("login_success", this.on_login_success);
        event_manager.Instance.add_event_listener("get_ugame_info_success", this.on_get_ugame_info_success);
        event_manager.Instance.add_event_listener("login_logic_server", this.ont_login_logic_server_success);
    }

    void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("login_success", this.on_login_success);
        event_manager.Instance.remove_event_listener("get_ugame_info_success", this.on_get_ugame_info_success);
        event_manager.Instance.remove_event_listener("login_logic_server", this.ont_login_logic_server_success);
    }


    void ont_login_logic_server_success(string event_name,object obj)
    {
        SceneManager.LoadScene("home_scene");
    }

    void on_login_success(string event_name, object udata)
    {
        //SceneManager.LoadScene("home_scene");
        Debug.Log("load game data.");
        system_service_proxy.Instance.load_user_ugame_info();
    }
    void on_get_ugame_info_success(string event_name, object udata)
    {
        //SceneManager.LoadScene("home_scene");
        logic_service_proxy.Instance.login_logic_server();
    }

    public void on_click_guest_login()
    {
        auth_service_proxy.Instance.guest_login();
    }

    public void on_click_uname_login()
    {
        if (this.uname_edit == null
            || this.upwd_edit == null
            || string.IsNullOrWhiteSpace(this.uname_edit.text)
            || string.IsNullOrWhiteSpace(this.upwd_edit.text))
        {
            return;
        }

        auth_service_proxy.Instance.uname_login(this.uname_edit.text, this.upwd_edit.text);
    }
}

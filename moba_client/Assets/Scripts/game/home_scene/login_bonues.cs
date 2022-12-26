using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class login_bonues : MonoBehaviour
{
    [SerializeField] private Transform[] fingerprint;
    [SerializeField] private Transform recv_button;

    void Start()
    {

    }

    public void show_login_bonues(int day)
    {
        int i;
        for (i = 0; i < day; i++)
        {
            this.fingerprint[i].gameObject.SetActive(true);
        }

        for (; i < 7; i++)
        {
            this.fingerprint[i].gameObject.SetActive(false);
        }

        if (ugame.Instance.ugame_info.BonuesStatus == 0)
        {
            this.recv_button.gameObject.SetActive(true);
        }
        else
        {
            this.recv_button.gameObject.SetActive(false);
        }
    }

    public void on_click_recv_login_bonues()
    {
        //领取登录奖励
        system_service_proxy.Instance.recv_login_bonues();
        on_click_login_bonues_close();
    }

    public void on_click_login_bonues_close()
    {
        //this.gameObject.SetActive(false);
        GameObject.Destroy(this.gameObject);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class email_info : MonoBehaviour
{
    [SerializeField]private GameObject opt_prefab;

    [SerializeField] private ScrollRect scollview;
    void Start()
    {
        // 监听一下拉取下来系统消息的事件;
        event_manager.Instance.add_event_listener("get_sys_email", this.on_get_sys_email_data);
        system_service_proxy.Instance.get_sys_msg();
    }
    void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("get_sys_email", this.on_get_sys_email_data);
    }

    void on_get_sys_email_data(string name, object udata)
    {
        // 显示我们系统邮件列表
        IList<string> sys_msgs = (IList<string>)udata;
        if (sys_msgs == null || sys_msgs.Count <= 0)
        {
            return;
        }
        // end 

        scollview.content.sizeDelta = new Vector2(0, sys_msgs.Count * 160);
        for (int i = 0; i < sys_msgs.Count; i++)
        {
            // Debug.Log(rank_info[i].unick + " " + rank_info[i].uchip);
            GameObject opt = GameObject.Instantiate(this.opt_prefab);
            opt.transform.SetParent(scollview.content, false);

            opt.transform.Find("order").GetComponent<Text>().text = "" + (i + 1);
            opt.transform.Find("msg_content").GetComponent<Text>().text = sys_msgs[i];
        }
    }

    public void on_close_click()
    {
        GameObject.Destroy(this.gameObject);
    }
}

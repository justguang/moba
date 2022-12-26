using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rank_list : MonoBehaviour
{
    [SerializeField] private GameObject rank_opt_prefab;
    [SerializeField] private GameObject content_root;
    [SerializeField] private ScrollRect rank;

    [SerializeField] private Sprite[] uface_img;

    void on_get_rank_list_data(string event_name, object udata)
    {
        IList<WorldChipRankInfo> rank_info = (IList<WorldChipRankInfo>)udata;

        this.rank.content.sizeDelta = new Vector2(0, rank_info.Count * 170);
        // 获取得到排行榜的数据
        for (int i = 0; i < rank_info.Count; i++)
        {
            // Debug.Log(rank_info[i].unick + " " + rank_info[i].uchip);
            GameObject opt = GameObject.Instantiate(this.rank_opt_prefab);
            opt.transform.SetParent(content_root.transform, false);

            opt.transform.Find("order").GetComponent<Text>().text = "" + (i + 1);
            opt.transform.Find("unick_label").GetComponent<Text>().text = rank_info[i].Unick;
            opt.transform.Find("uchip_label").GetComponent<Text>().text = "" + rank_info[i].Uchip;
            int uface = rank_info[i].Uface - 1;
            if (uface < 0 || uface > 8) uface = 0;
            opt.transform.Find("header/avator").GetComponent<Image>().sprite = this.uface_img[uface];
        }
        // end
    }

    public void on_rank_list_close()
    {
        GameObject.Destroy(this.gameObject);
    }

    void Start()
    {
        event_manager.Instance.add_event_listener("get_rank_list", this.on_get_rank_list_data);
        // 发送拉取排行榜的请求
        system_service_proxy.Instance.get_world_uchip_rank_info();
        // end 
    }
    void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("get_rank_list", this.on_get_rank_list_data);
    }
}

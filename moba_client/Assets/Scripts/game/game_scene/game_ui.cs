using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ui_blood_info
{
    public int blood;
    public int max_blood;
}
public class ui_exp_info
{
    public int exp;
    public int total;
}

public class game_ui : MonoBehaviour
{
    [SerializeField] private Image blood_process;
    [SerializeField] private Image exp_process;
    [SerializeField] private Text blood_label;
    [SerializeField] private Text exp_label;

    void Start()
    {
        event_manager.Instance.add_event_listener("exp_ui_sync", this.on_exp_ui_sync);
        event_manager.Instance.add_event_listener("blood_ui_sync", this.on_blood_ui_sync);
    }

    void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("exp_ui_sync", this.on_exp_ui_sync);
        event_manager.Instance.remove_event_listener("blood_ui_sync", this.on_blood_ui_sync);
    }

    void on_exp_ui_sync(string event_name, object udata)
    {
        ui_exp_info info = (ui_exp_info)udata;
        this.exp_process.fillAmount = (float)info.exp / (float)info.total;
        this.exp_label.text = info.exp + " / " + info.total;
    }

    void on_blood_ui_sync(string event_name, object udata)
    {
        ui_blood_info info = (ui_blood_info)udata;
        this.blood_process.fillAmount = (float)info.blood / (float)info.max_blood;
        this.blood_label.text = info.blood + " / " + info.max_blood;
    }
}

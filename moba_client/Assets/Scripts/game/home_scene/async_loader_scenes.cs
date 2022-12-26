using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//异步加载场景：使用协程
public class async_loader_scenes : MonoBehaviour
{
    [Tooltip("要加载场景的名字")]
    public string scene_name;
    [SerializeField] private Image process;//场景加载进度条

    private AsyncOperation ao;
    void Start()
    {
        this.process.fillAmount = 0f;
        this.StartCoroutine(this.async_load_scene());
    }

    IEnumerator async_load_scene()
    {
        this.ao = SceneManager.LoadSceneAsync(this.scene_name);
        this.ao.allowSceneActivation = false;//设置不自动切换

        yield return this.ao;
    }


    void Update()
    {
        float per = this.ao.progress;//场景加载进度【0~1】
        this.process.fillAmount = per;
        if (per >= 0.9f)//加载完成
        {
            this.process.fillAmount = 1;
            if (this.ao.allowSceneActivation) return;
            this.ao.allowSceneActivation = true;
        }
    }
}

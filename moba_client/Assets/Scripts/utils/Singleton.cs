using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//普通单例
public abstract class Singleton<T> where T : new()
{
    private static object mutex = new object();
    private static T _ins;
    public static T Instance
    {
        get
        {
            if (_ins == null)
            {
                lock (mutex)
                {
                    if (_ins == null)
                    {
                        _ins = new T();
                    }
                }
            }
            return _ins;
        }
    }
}

//Mono
//Unity单例
public class UnitySingleton<T> : MonoBehaviour where T : Component
{
    private static T _ins = null;
    public static T Instance
    {
        get
        {
            if (_ins == null)
            {
                _ins = FindObjectOfType(typeof(T)) as T;
            }
            if (_ins == null)
            {
                GameObject obj = new GameObject();
                _ins = (T)obj.AddComponent(typeof(T));
                obj.hideFlags = HideFlags.DontSave;
                obj.name = typeof(T).Name;
            }

            return _ins;
        }
    }

    public virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (_ins == null)
        {
            _ins = this as T;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}


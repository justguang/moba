using System;
using System.Collections;
using System.Collections.Generic;

public class event_manager : Singleton<event_manager>
{
    private Dictionary<string, Action<string, object>> event_listeners = new Dictionary<string, Action<string, object>>();

    public void init()
    {
        
    }
    public void add_event_listener(string code, Action<string, object> callback)
    {
        if (this.event_listeners.ContainsKey(code))
        {
            this.event_listeners[code] += callback;
        }
        else
        {
            this.event_listeners.Add(code, callback);
        }
    }

    public void remove_event_listener(string code, Action<string, object> callback)
    {
        if (this.event_listeners.ContainsKey(code))
        {
            this.event_listeners[code] -= callback;
            if (this.event_listeners[code] == null) this.event_listeners.Remove(code);
        }
    }

    public void dispatch_event(string code, object obj)
    {
        if (this.event_listeners.ContainsKey(code))
        {
            this.event_listeners[code]?.Invoke(code, obj);
        }
    }
}

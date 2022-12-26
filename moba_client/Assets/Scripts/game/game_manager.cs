using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game_manager : UnitySingleton<game_manager>
{
    void Start()
    {
        event_manager.Instance.init();
        ulevel.Instance.init();
        auth_service_proxy.Instance.init();
        system_service_proxy.Instance.init();
        logic_service_proxy.Instance.init();
    }
}

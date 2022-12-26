using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;

public class game_scene : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        network.Instance.add_service_listener((int)Stype.Auth, this.on_auth_server_return);

        // test
        this.Invoke("test", 3.0f);
        // end 
    }

    void on_auth_server_return(cmd_msg msg)
    {
        switch (msg.ctype)
        {
        }
    }

    void OnDestroy()
    {
        if (network.Instance)
        {
            network.Instance.remove_service_listener((int)Stype.Auth, this.on_auth_server_return);
        }
    }
    void test()
    {
        //network.Instance.send_protobuf_cmd((int)Stype.Auth, (int)Cmd.ELoginReq, null);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

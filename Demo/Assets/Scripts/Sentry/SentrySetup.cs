using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SentrySetup : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        RegisterSentry();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        GameManager.Singleton.UnRegisterSentry(transform.name);
    }
    private void RegisterSentry()
    {
        string name = "Sentry " + GetComponent<NetworkObject>().NetworkObjectId.ToString();
        Sentry sentry = GetComponent<Sentry>();
        GameManager.Singleton.RegisterSentry(name, sentry);
        sentry.SetUp();
    }

}

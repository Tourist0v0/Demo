using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TowerSetup : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        RegisterTower();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        GameManager.Singleton.UnRegisterTower(transform.name);
    }
    private void RegisterTower()
    {
        string name = "Tower " + GetComponent<NetworkObject>().NetworkObjectId.ToString();
        Tower tower = GetComponent<Tower>();
        GameManager.Singleton.RegisterTower(name, tower);
        tower.SetUp();
    }
}

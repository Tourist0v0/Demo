using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour
{
    // 设置禁用列表
    [SerializeField] private Behaviour[] componentsToDisable;
    private Camera sceneCamera;
    // 当游戏对象成功联网时会执行一遍
    // 这里如果用 pubic override void OnNetworkDespawn()会使得场景相机在服务器被改动？无法在客户端修改？
    void Start() 
    {
        // base.OnNetworkSpawn();
        // 如果不是本地玩家则禁用组件
        if (!IsLocalPlayer)
        {
            DisableComponents();
        }
        else
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }

        RegisterPlayer();

    }

    private void RegisterPlayer()
    {
        string name = "Player " + GetComponent<NetworkObject>().NetworkObjectId.ToString();
        Player player = GetComponent<Player>();
        GameManager.Singleton.RegisterPlayer(name, player);
        player.SetUp();
    }
    private void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }
    // 当一个客户端断开连接之前会执行这个操作
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        // 当本地玩家消失时再把场景相机打开
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
        GameManager.Singleton.UnRegisterPlayer(transform.name);
    }
}

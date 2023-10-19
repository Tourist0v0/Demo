using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    public PlayerBullet bullet;
    public TextMeshProUGUI gameOverText;
    public Canvas cameraCrossHair;
    // 设置玩家最大血量
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Behaviour[] componentsToDisable;
    // 存储组件的初始状态
    private bool[] componentsEnabled;
    // 设置需要全局同步的变量
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    private NetworkVariable<bool> isDead = new NetworkVariable<bool>();

    public void SetUp()
    {
        // 存每个组件的初始状态
        componentsEnabled = new bool[componentsToDisable.Length];
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsEnabled[i] = componentsToDisable[i].enabled;
        }
        SetDefaults();
    }

    private void SetDefaults()
    {
        // 玩家重生的时候也会调用这个函数, 把所有组件恢复原状, 即恢复玩家控制权与恢复碰撞检测
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = componentsEnabled[i];
        }
        // 有可能在多个端口调用这个函数，但只在服务器端去修改这个网络同步变量是有效的
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            isDead.Value = false;
        }
    }
    
    // 玩家重生
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);
        SetDefaults();
        if(IsLocalPlayer) transform.position = new Vector3(0f, 0.1f, 0f);
    }
    
    // 在服务器端执行一次 Die()
    private void DieOnServer()
    {
        Die(); 
    }
    // ClientRpc 在服务器端调用, 会自动在每个客户端都执行一遍
    [ClientRpc]
    private void DieClientRpc()
    {
        if(!IsHost) Die();
    }
    
    private void Die()
    {
        // 没收掉玩家的控制权, 即取消组件并取消碰撞检测
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
        // 用一个新的线程来执行重生函数
        StartCoroutine(Respawn());

    }

    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
        cameraCrossHair.gameObject.SetActive(false);
        // 没收掉玩家的控制权, 即取消组件并取消碰撞检测
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }
    // 玩家受到攻击时调用, 且只在服务器端调用
    public void TakeDamage(int damage)
    {
        if (isDead.Value) return; // 如果已经死亡了就不让他再受到伤害了
        // 掉血
        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            isDead.Value = true;
            DieOnServer();
            DieClientRpc(); // 在服务器端调用，会在所有客户端执行，但在服务器端不执行
        }
    }
    // 返回玩家现在的血量
    public int GetCurrentHealth()
    {
        return currentHealth.Value;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet") && other.gameObject.layer != gameObject.layer && other.gameObject.layer != 4)
        {
            shootServerRpc(gameObject.name, bullet.damage);
        }
    }
    
    [ServerRpc]
    private void shootServerRpc(string name, int damage)
    {
        Player player = GameManager.Singleton.GetPlayer(name); // 在服务器端的集合中取出来了 player2 
        player.TakeDamage(damage); // 只有服务器端的 player2 会调用受到伤害的函数，客户端的 player2 是不调用的
    }
}

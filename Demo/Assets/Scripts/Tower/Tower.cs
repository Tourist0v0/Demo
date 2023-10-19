using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Tower : NetworkBehaviour
{
    public PlayerBullet bullet;
    // 设置基地最大血量
    private const int maxHealth = 100;
    // 设置需要全局同步的变量
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    private NetworkVariable<bool> isDead = new NetworkVariable<bool>();
    // 基地的状态：是否无敌？
    private NetworkVariable<bool> isGod = new NetworkVariable<bool>();
    [SerializeField] private Sentry sentry;
    private NetworkVariable<bool> isSentryDead;
    // [SerializeField] private Slider HP;
    public bool GetDeadState()
    {
        return isDead.Value;
    }
    private void Update()
    {
        isSentryDead = sentry.GetHealthState();
        if (isSentryDead.Value) isGod.Value = false;
    }

    public void SetUp()
    {
        SetDefaults();
    }

    private void SetDefaults()
    {
        // 有可能在多个端口调用这个函数，但只在服务器端去修改这个网络同步变量是有效的
        if (IsServer)
        {
            // HP.value = 1;  // Value的值介于0-1之间，且为浮点数
            currentHealth.Value = maxHealth;
            isDead.Value = false;
            isGod.Value = true;
        }
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
        isDead.Value = true;
    }
    
    // 哨兵受到攻击时调用, 且只在服务器端调用
    public void TakeDamage(int damage)
    {
        if (isDead.Value) return; // 如果已经死亡了就不让他再受到伤害了
        // 掉血
        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            DieOnServer();
            DieClientRpc(); // 在服务器端调用，会在所有客户端执行，但在服务器端不执行
            
        }
    }
    // 返回基地现在的血量
    public int GetCurrentHealth()
    {
        return currentHealth.Value;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isGod.Value) return;
        // Debug.Log("Sentry is hitted!" + " currentHealth: " + GetCurrentHealth());
        if (other.gameObject.CompareTag("Bullet"))
        {
            shootServerRpc(gameObject.name, bullet.damage);
        }
    }
    
    [ServerRpc]
    private void shootServerRpc(string name, int damage)
    {
        // GameManager.UpdateInfo(transform.name + " hit " + name);
        Tower tower = GameManager.Singleton.GetTower(name); // 在服务器端的集合中取出来了 player2 
        tower.TakeDamage(damage); // 只有服务器端的 player2 会调用受到伤害的函数，客户端的 player2 是不调用的
    }
    
}

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Sentry : NetworkBehaviour
{
    // 设置哨兵最大血量
    [SerializeField] private int maxHealth = 100;
    // 设置需要全局同步的变量
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    private NetworkVariable<bool> isDead = new NetworkVariable<bool>();
    private NetworkVariable<bool> isWakeUp = new NetworkVariable<bool>();
    public Camera cam;
    public PlayerBullet bullet;
    public NetworkVariable<bool> GetHealthState()
    {
        return isDead;
    }
    public NetworkVariable<bool> GetWakeUpState()
    {
        return isWakeUp;
    }
    // 哨兵初始化
    public void SetUp()
    {
        SetDefaults();
    }

    private void SetDefaults()
    {
        
        // 有可能在多个端口调用这个函数，但只在服务器端去修改这个网络同步变量是有效的
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            isWakeUp.Value = true;
            isDead.Value = false;
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
        // 死亡时解除基地的无敌模式
        Debug.Log("Sentry is dead!");

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
            isDead.Value = true;
            DieOnServer();
            DieClientRpc(); // 在服务器端调用，会在所有客户端执行，但在服务器端不执行
        }
    }
    // 返回哨兵现在的血量
    public int GetCurrentHealth()
    {
        return currentHealth.Value;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            shootServerRpc(gameObject.name, bullet.damage);
            
        }
    }
    
    [ServerRpc]
    private void shootServerRpc(string name, int damage)
    {
        Sentry sentry = GameManager.Singleton.GetSentry(name); // 在服务器端的集合中取出来了 player2 
        sentry.TakeDamage(damage); // 只有服务器端的 player2 会调用受到伤害的函数，客户端的 player2 是不调用的
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletHolder;
    
    [SerializeField] private Camera fpv;
    [SerializeField] private LayerMask mask;
    public PlayerBullet bullet;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            shoot();
        }
    }

    private void OnShoot()
    {
        // launch bullet from the bullet holder
        GameObject bullet = Instantiate(bulletPrefab, bulletHolder.transform.position, bulletPrefab.transform.rotation);
        bullet.layer = gameObject.layer;
        bullet.GetComponent<Rigidbody>().AddForce(fpv.transform.forward * this.bullet.speed);
        Destroy(bullet,5);//销毁子弹物体
    }

    [ServerRpc]
    private void OnShootServerRpc()
    {
        if(!IsHost) OnShoot();
        OnShootClientRpc();

    }

    [ClientRpc]
    private void OnShootClientRpc()
    {
        OnShoot();
    }
    private void shoot()
    {
        OnShootServerRpc();
    }

    [ServerRpc]
    private void shootServerRpc(string name, int damage)
    {
        Player player = GameManager.Singleton.GetPlayer(name); // 在服务器端的集合中取出来了 player2 
        player.TakeDamage(damage); // 只有服务器端的 player2 会调用受到伤害的函数，客户端的 player2 是不调用的
    }
}

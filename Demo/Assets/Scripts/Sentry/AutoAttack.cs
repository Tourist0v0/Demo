using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AutoAttack : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public PlayerBullet bullet;
    public GameObject bulletHolder;
    private float passedTime = 0f; // default 0
    public float targetTime = 0.1f;
    [SerializeField] private Sentry sentry;
    [SerializeField] private Camera fpv;
    private bool isDead;

    private float lookAngle = 60f;

    private float lookAccurate = 5;

    private float Rayrange = 100f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        isDead = sentry.GetHealthState().Value;
        if (!isDead) Repeat();
    }

    void Repeat()
    {
        if (passedTime > targetTime)
        {
            LookServerRpc();
            passedTime = 0; //enter next loop
        }
        passedTime += Time.deltaTime;
    }

    private void OnShoot()
    {
        // launch bullet from the bullet holder
        GameObject bullet = Instantiate(bulletPrefab, bulletHolder.transform.position, bulletPrefab.transform.rotation);
        bullet.layer = 4;
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

    [ServerRpc]
    private void ShootServerRpc(string name, int damage)
    {
        Player player = GameManager.Singleton.GetPlayer(name); // 在服务器端的集合中取出来了 player2 
        player.TakeDamage(damage); // 只有服务器端的 player2 会调用受到伤害的函数，客户端的 player2 是不调用的
    }

    [ServerRpc]
    private void LookServerRpc()
    {
        if(!IsHost) Look();
        LookClientRpc();
    }
    [ClientRpc]
    private void LookClientRpc()
    {
        Look();
    }
    
    //射线检测
    private void Look()
    {
        //一条向前的射线
        LookAround(fpv, Quaternion.identity, Color.green);
        //多一个精确度就多两条对称的射线,每条射线夹角是总角度除与精度
        float subAngle = (lookAngle / 2) / lookAccurate;
        for (int i = 0; i < lookAccurate; i++)
        {
            LookAround(fpv, Quaternion.Euler(0, -1 * subAngle * (i + 1), 0), Color.green);
            LookAround(fpv, Quaternion.Euler(0, subAngle * (i + 1), 0), Color.green);
        }
    }
    //射出射线检测是否有Player
    private void LookAround(Camera fpv, Quaternion eulerAnger,Color DebugColor)
    {
        Debug.DrawRay(fpv.transform.position, eulerAnger * fpv.transform.forward.normalized *  Rayrange, DebugColor);

        RaycastHit hit;
        if (Physics.Raycast(fpv.transform.position, eulerAnger * fpv.transform.forward, out hit) && hit.collider.CompareTag("Player"))
        {
            Debug.Log("Sentry hit Player!");
            OnShootServerRpc();
            ShootServerRpc(hit.collider.name, bullet.damage); // 发到服务器上去执行
        }
    }
}

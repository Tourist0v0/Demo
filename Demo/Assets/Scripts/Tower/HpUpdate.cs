using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HpUpdate : NetworkBehaviour
{
    // 获取到对应基地的当前血量
    // 基地最大血量
    private const int maxHealth = 100;
    // 设置需要全局同步的变量
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    [SerializeField] private Tower tower;
    [SerializeField] private Slider hp;
    // Start is called before the first frame update
    void Start()
    {
        hp.value = 1;
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth.Value = tower.GetCurrentHealth(); 
        hp.value = currentHealth.Value / (float)maxHealth;
    }
}

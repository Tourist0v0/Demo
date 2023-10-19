using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button hostBtn;

    [SerializeField] private Button serverBtn;

    [SerializeField] private Button clientBtn;
    // Start is called before the first frame update
    void Start()
    {
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            DestroyAllBtn();
        });
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            DestroyAllBtn();
        });
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            DestroyAllBtn();
        });
    }
    // 实现当选择了一个按钮之后把所有按钮都删掉
    private void DestroyAllBtn()
    {
        Destroy(hostBtn.gameObject);
        Destroy(serverBtn.gameObject);
        Destroy(clientBtn.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

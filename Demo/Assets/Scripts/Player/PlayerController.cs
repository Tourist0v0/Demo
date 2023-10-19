using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera[] cameraLists;
    private Camera sceneCamera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float turnSpeed = 3.0f;
    private float xMov = 0f;
    private float yMov = 0f;
    private float cameraRotationTotal = 0f;
    [SerializeField] private float cameraRotationLimit = 20f;
    // 旋转：左右是旋转角色，上下是旋转摄像机
    private Vector3 xRotation = Vector3.zero; // x轴动，旋转摄像机
    private Vector3 yRotation = Vector3.zero; // y轴动，旋转角色
    
    public void Move(float _xMov, float _yMov)
    {
        xMov = _xMov;
        yMov = _yMov;
    }
    public void Rotate(Vector3 _xRotation, Vector3 _yRotation)
    {
        yRotation = _yRotation;
        xRotation = _xRotation;
    }
    private void PerformMovement()
    {
        transform.position += -transform.forward * Time.deltaTime * speed * yMov;
        transform.position += -transform.right * Time.deltaTime * turnSpeed * xMov;
    }
    private void PerformRotation()
    {
        if(yRotation != Vector3.zero)
        {
            rb.transform.Rotate(yRotation);
        }

        if(xRotation != Vector3.zero)
        {
            if (cameraLists[0].enabled == true)
            {
                // cameraLists[0].transform.Rotate(xRotation);
                cameraRotationTotal += xRotation.x; // 计算累计转了多少度
                cameraRotationTotal = Mathf.Clamp(cameraRotationTotal,-cameraRotationLimit, cameraRotationLimit);
                cameraLists[0].transform.localEulerAngles = new Vector3(cameraRotationTotal, 180f, 0f);
            }
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetDefaultCam();
    }

    // Update is called once per frame
    private void Update()
    {
        // Fixupdate 更新时间更均匀，常用于模拟物理轨迹
        PerformMovement();
        // 旋转
        PerformRotation();
        // 检查视角
        CheckViewSwitch();
    }

    private void SetDefaultCam()
    {
        sceneCamera = Camera.main;
        cameraLists[0].enabled = true;
        cameraLists[1].enabled = false;
    }
    private void CheckViewSwitch()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // for (int i = 0; i < cameraLists.Length; i++)
            // {
                // cameraLists[i].enabled = !cameraLists[i].enabled;
            // }
            // cameraLists[0].enabled = false;
            // sceneCamera.gameObject.SetActive(true);
            if (cameraLists[0].enabled && !cameraLists[1].enabled)
            {
                cameraLists[0].enabled = false;
                cameraLists[1].enabled = true;
            }
            else if (!cameraLists[0].enabled && cameraLists[1].enabled)
            {
                cameraLists[1].enabled = false;
                sceneCamera.gameObject.SetActive(true);
            }
            else
            {
                sceneCamera.gameObject.SetActive(false);
                cameraLists[0].enabled = true;
            }
            
        }
    }
    
}

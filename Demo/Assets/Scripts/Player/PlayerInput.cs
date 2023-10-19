using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float sensitivity = 5.0f; // 鼠标灵敏度
    [SerializeField] private PlayerController controller; // 把组件的引用拿过来
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float xMov = Input.GetAxisRaw("Horizontal"); // 每一帧都要去获取横向移动命令
        float yMov = Input.GetAxisRaw("Vertical"); // 获取纵方向的移动命令
        controller.Move(xMov, yMov);

        // 获取旋转信息
        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");
        Vector3 yRotation = new Vector3(0f, xMouse, 0f) * sensitivity;
        Vector3 xRotation = new Vector3(-yMouse, 0f, 0f) * sensitivity;
        controller.Rotate(xRotation, yRotation);
    }
}

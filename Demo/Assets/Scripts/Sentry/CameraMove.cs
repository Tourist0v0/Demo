using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraMove : NetworkBehaviour
{
    [SerializeField] private float speed = 2.0f;

    [SerializeField] private float upEnd = 15;

    [SerializeField] private float downEnd = 35;
    [SerializeField] private Sentry sentry;
    private NetworkVariable<bool> isDead;

    [SerializeField] private float cameraRotation = 25f;

    // Update is called once per frame
    void Update()
    {
        isDead = sentry.GetHealthState();
        if(!isDead.Value) ChangingMove();
    }

    private void ChangingMove()
    {
        cameraRotation += speed * Time.deltaTime;
        transform.localEulerAngles = new Vector3(cameraRotation, 85f, 0f);
        if (cameraRotation >= downEnd)
        {
            speed = -2.0f;
        }
        else if(cameraRotation <= upEnd)
        {
            speed = 2.0f;
        }
    }
}

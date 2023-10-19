using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Move : NetworkBehaviour
{
    public float speed = 2.0f;

    public float leftEnd;

    public float rightEnd;
    [SerializeField] private int direction;
    private Sentry sentry;
    private NetworkVariable<bool> isDead;
    private NetworkVariable<bool> isWakeUp;

    private Vector3 moveWay;
    // Start is called before the first frame update
    void Start()
    {
        sentry = GetComponent<Sentry>();
        moveWay = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        isDead = sentry.GetHealthState();
        isWakeUp = sentry.GetWakeUpState();
        if(isWakeUp.Value && !isDead.Value) ChangingMove();
    }

    private void ChangingMove()
    {
        transform.position += moveWay * Time.deltaTime * speed;
        if (transform.position.z >= leftEnd)
        {
            moveWay = -transform.forward * direction;
        } else if (transform.position.z <= rightEnd)
        {
            moveWay = transform.forward * direction;
        }
    }
}

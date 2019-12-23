using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    public GameObject Mouse;
    public float movSpeed;

    private Rigidbody wolfRb;
    private RayPerception rayPerception;
    private Vector3 startPos;


    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Mouse.transform);
        transform.position += transform.forward * movSpeed * Time.deltaTime;

       
    }
    private void OnTriggerEnter(Collider other)
    {
        Mouse.GetComponent<MouseAgent>().AgentReset();
        transform.localPosition = startPos;
        this.wolfRb.angularVelocity = Vector3.zero;
        this.wolfRb.velocity = Vector3.zero;
    }
}

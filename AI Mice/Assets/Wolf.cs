using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    public GameObject Mouse;
    public float movSpeed;

    private Rigidbody wolfRb;
    private Vector3 startPos;


    // Update is called once per frame

    void Start()
    {
        wolfRb = GetComponent<Rigidbody>();
        startPos = transform.localPosition;
    }
    void Update()
    {
        transform.LookAt(Mouse.transform);
        transform.position += transform.forward * movSpeed * Time.deltaTime;

       
    }
    private void OnTriggerEnter(Collider other)
    {
        transform.localPosition = startPos;
        this.wolfRb.angularVelocity = Vector3.zero;
        this.wolfRb.velocity = Vector3.zero;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("mouse"))
        {
            transform.localPosition = startPos;
            this.wolfRb.angularVelocity = Vector3.zero;
            this.wolfRb.velocity = Vector3.zero;
        }
    }
    }

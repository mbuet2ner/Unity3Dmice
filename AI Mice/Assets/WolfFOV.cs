using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfFOV : MonoBehaviour
{
    public float movSpeed;
    public float fovRadius;
    [Range(0, 360)]
    public float fovAngel;
    public LayerMask targetLayer;
    public LayerMask obstLayer;
    private Rigidbody wolfRb;
    private Vector3 startPos;
    public List<Transform> mouseInView = new List<Transform>();
    float baseDirection = 0;

    void Start()
    {
        StartCoroutine("FindMouseEnmu", .2f);
        wolfRb = GetComponent<Rigidbody>();
        startPos = transform.localPosition;
    }

    void Update()
    {
     transform.position += transform.forward * movSpeed * Time.deltaTime;
    }

    IEnumerator FindMouseEnmu(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindMouse();
        }
    }

    void FindMouse()
    {
        mouseInView.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, fovRadius, targetLayer);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform mouse = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (mouse.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < fovAngel / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, mouse.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstLayer))
                {
                    mouseInView.Add(mouse);
                    transform.LookAt(mouseInView[0].position);
                }
            }
        }
    }

    public Vector3 Direction(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("fence"))
        {
            Debug.Log("Hit");
            baseDirection = baseDirection + Random.Range(-30, 30);
        }
        else if (collision.gameObject.CompareTag("mouse"))
        {
            transform.localPosition = startPos;

        }
    }
}
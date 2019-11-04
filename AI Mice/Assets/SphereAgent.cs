using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class SphereAgent : Agent
{
    Rigidbody rBody;
    private Vector3 startPos;
    private bool dead = false;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        startPos = transform.localPosition;
    }

    public Transform Target;
    public override void AgentReset()
    {
        dead = false;
        transform.localPosition = startPos;
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(Target.localPosition);
        AddVectorObs(this.transform.localPosition);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);
    }

    public float speed = 10;
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (dead)
        {
            SetReward(-1f);
            Done();
        }
        else
        {
            SetReward(0.01f);
            // Actions, size = 2
            Vector3 controlSignal = Vector3.zero;
            controlSignal.x = vectorAction[0];
            controlSignal.z = vectorAction[1];
            rBody.AddForce(controlSignal * speed);

            // Rewards
            float distanceToTarget = Vector3.Distance(this.transform.localPosition,
                                                      Target.localPosition);

            // Getting closer to target
            if (distanceToTarget > 1.42f)
            {
                SetReward(-0.001f * distanceToTarget);
            }
            
            // Reached target
            if (distanceToTarget < 1.42f)
            {
                // Set rewards reset all previous rewards
                // SetReward(1.0f);
                SetReward(1.0f);
                Done();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("fence"))
        {
            dead = true;
        }
    }
}
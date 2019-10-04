using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class MouseAgent : Agent
{
    Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;
    public override void AgentReset()
    {
        if (this.transform.localPosition.y < 0)
        {
            // If the Agent fell, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(-11f, 0.4f, -13f);
        }
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
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed * 100);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition,
                                                  Target.localPosition);

        // Reset when it takes to long


        // Getting closer to target
        //if (distanceToTarget > 1.42f)
        //{
        //    AddReward(-0.001f * distanceToTarget);
        //}
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            // Set rewards reset all previous rewards
            // SetReward(1.0f);
            AddReward(1.0f);
            Done();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("fence"))
        {
            AddReward(-0.1f);
        }
    }
}
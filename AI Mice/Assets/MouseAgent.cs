using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class MouseAgent : Agent
{
    public float moveSpeed = 1f;
    public float rotateSpeed = 2f;

    private Rigidbody agentRigidbody;
    private Vector3 startPos;

    void Start()
    {
        agentRigidbody = GetComponent<Rigidbody>();
        startPos = transform.localPosition;
    }

    public Transform Target;
    public override void AgentReset()
    {
        transform.localPosition = startPos;
        this.agentRigidbody.angularVelocity = Vector3.zero;
        this.agentRigidbody.velocity = Vector3.zero;
    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(Target.localPosition);
        AddVectorObs(this.transform.localPosition);

        // Add velocity observation
        Vector3 localVelocity = transform.InverseTransformDirection(agentRigidbody.velocity);
        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
    }

    public float speed = .5f;
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Determine the rotation action
        float rotateAmount = 0;
        if (vectorAction[1] == 1)
        {
            rotateAmount = -rotateSpeed;
        }
        else if (vectorAction[1] == 2)
        {
            rotateAmount = rotateSpeed;
        }

        // Apply the rotation
        Vector3 rotateVector = transform.up * rotateAmount;
        agentRigidbody.MoveRotation(Quaternion.Euler(agentRigidbody.rotation.eulerAngles + rotateVector * rotateSpeed));

        // Determine move action
        float moveAmount = 0;
        if (vectorAction[0] == 1)
        {
            moveAmount = moveSpeed;
        }
        else if (vectorAction[0] == 2)
        {
            moveAmount = moveSpeed * -.5f; // move at half-speed going backwards
        }

        // Apply the movement
        Vector3 moveVector = transform.forward * moveAmount;
        agentRigidbody.AddForce(moveVector * moveSpeed, ForceMode.VelocityChange);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition,
                                                  Target.localPosition);

        // Getting closer to target
        if (distanceToTarget > 1.42f)
        {
            AddReward(1.0f / distanceToTarget);
        }
        // Reached target
        else if (distanceToTarget < 1.42f)
        {
            // Set rewards reset all previous rewards
            // SetReward(1.0f);
            AddReward(1.0f);
            Done();
        }

        // Determine state
        if (GetCumulativeReward() <= -5f)
        {
            // Reward is too negative, give up
            Done();
        }
        else
        {
            // Encourage movement with a tiny time penalty and pdate the score text display
            AddReward(-.001f);
            //agentArea.UpdateScore(GetCumulativeReward());
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("fence"))
        {
            AddReward(-.01f);
        }
    }
}
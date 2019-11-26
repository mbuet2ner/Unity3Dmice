﻿using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class MouseAgent : Agent
{
    [Header("MouseAgent Settings")]
    public Transform Target;
    public float moveSpeed = 1f;
    public float rotateSpeed = 2f;

    private Rigidbody agentRigidbody;
    private RayPerception3D rayPerception;
    private Vector3 startPos;

    private bool signFirstHit = true;

    /// <summary>
    ///  Initialize MouseAgent
    /// </summary>
    void Start()
    {
        agentRigidbody = GetComponent<Rigidbody>();
        rayPerception = GetComponent<RayPerception3D>();
        startPos = transform.localPosition;
    }

    /// <summary>
    /// Resets the agent to starting position
    /// Resets velocity
    /// </summary>
    public override void AgentReset()
    {
        transform.localPosition = startPos;
        this.agentRigidbody.angularVelocity = Vector3.zero;
        this.agentRigidbody.velocity = Vector3.zero;
    }

    /// <summary>
    /// Obervations by the agent
    /// Target and own position, vision of "fence" and "goal"
    /// </summary>
    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(Target.localPosition);
        AddVectorObs(this.transform.localPosition);

        // Add raycast perception observations for stumps and walls
        float rayDistance = 20f;
        float[] rayAngles = { 90f, 180f, 270f, 360f };
        string[] detectableObjects = { "fence", "goal" };
        AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));

        // Add velocity observation
        Vector3 localVelocity = transform.InverseTransformDirection(agentRigidbody.velocity);
        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
    }

    /// <summary>
    /// The actions that MouseAgent is able to do
    /// Moving and rotating
    /// </summary>
    /// <param name="vectorAction"></param>
    /// <param name="textAction"></param>
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

        // Apply the rotation (3D movement and rotation)
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
        }

    }

    /// <summary>
    /// If collision with "goal" or "fence"
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("goal"))
        {
            AddReward(5f);
            Done();
        }
        else if (collision.gameObject.CompareTag("fence"))
        {
            AddReward(-.01f);
        }
        else if (collision.gameObject.CompareTag("sign") && signFirstHit == true)
            AddReward(1f);
        signFirstHit = false;
    }
}
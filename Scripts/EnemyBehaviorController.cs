using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehaviorController : MonoBehaviour
{
    [SerializeField] private float idleTimeout = 5.0f;
    [SerializeField] private float searchTimeout = 15.0f;
    [SerializeField] private float chaseTimeout = 10.0f;
    [SerializeField] private float wanderTimeout = 15.0f;
    private float timer = 0.0f;
    private enum EnemyState
    {
        Chasing,
        Searching,
        Idle,
        Wandering
    }
    private EnemyState state;

    void Awake()
    {
        state = EnemyState.Wandering;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        switch (state)
        {
            case EnemyState.Chasing:
                // Code for when enemy is chasing
                chasing();
                if (timer > chaseTimeout)
                {
                    state = EnemyState.Searching;
                    timer = 0.0f;
                }
                break;
            case EnemyState.Searching:
                // Code for when enemy is searching
                searching();
                if (timer > searchTimeout)
                {
                    if (new System.Random().Next(0, 2) == 0)//50% chance to go back to wandering or idle
                    {
                        state = EnemyState.Wandering;
                    }
                    else
                    {
                        state = EnemyState.Idle;
                    }
                    timer = 0.0f;
                }
                break;
            case EnemyState.Idle:
                // Code for when enemy is idle
                idle();
                if (timer > searchTimeout)
                {
                    state = EnemyState.Wandering;
                    timer = 0.0f;
                }
                break;
            case EnemyState.Wandering:
                // Code for when enemy is wandering
                wandering();
                if (timer > searchTimeout)
                {
                    state = EnemyState.Idle;
                    timer = 0.0f;
                }
                break;
        }
    }

    private void chasing()
    {
        throw new NotImplementedException("chasing() not implemented");
    }

    private void searching()
    {
        throw new NotImplementedException("chasing() not implemented");
    }

    private void idle()
    {
        throw new NotImplementedException("chasing() not implemented");
    }

    private void wandering()
    {
        throw new NotImplementedException("chasing() not implemented");
    }

    
}

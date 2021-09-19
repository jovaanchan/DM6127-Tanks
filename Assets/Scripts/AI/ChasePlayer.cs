using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    private bool tankEnabled = false;
    double minSpeed = 4;

    private void OnEnable()
    {
        // When the tank is turned on, reset the launch force and the UI
        this.tankEnabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        agent = this.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (!this.tankEnabled)
            return;
        
        float distToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distToPlayer < 5f)
        {
          return;
        }
        agent.SetDestination(player.transform.position);
    }

    public void SetSpeed(int points) 
    {
      agent = this.GetComponent<NavMeshAgent>();
      double percentageToIncrease = (Math.Floor((double) points / 5) * 0.1) + 1;
      float newSpeed = (float) (percentageToIncrease * minSpeed);
      // Debug.Log(newSpeed);
      if (agent.speed != newSpeed)
      {
        agent.speed = newSpeed;
      }
    }
}

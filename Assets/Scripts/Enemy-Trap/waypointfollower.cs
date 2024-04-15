using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waypointfollower : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    private int currentWaypointIndex = 0;
    [SerializeField] private float speed = 2f;

    private void Update()
    {
        if (Vector2.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < .1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }

            // Check if sprite needs to flip
            FlipSprite();
        }

        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, Time.deltaTime * speed);
    }

    private void FlipSprite()
    {
        // Calculate direction from current waypoint to next waypoint
        Vector3 direction = (waypoints[currentWaypointIndex].transform.position - transform.position).normalized;

        // Check if direction is to the left
        bool facingLeft = direction.x < 0;

        // Flip sprite if necessary
        if (facingLeft)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f); // Flip horizontally
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f); // Reset to original scale
        }
    }
}

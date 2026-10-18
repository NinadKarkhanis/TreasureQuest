using UnityEngine;

public class waypointfollower : MonoBehaviour // Keep legacy class name to preserve scene references.
{
    [SerializeField] private GameObject[] waypoints;
    [SerializeField] private float speed = 2f;
    private int currentWaypointIndex;
    private Vector3 initialScale;

    private void Awake() { initialScale = transform.localScale; }
    private void Update()
    {
        if (waypoints == null || waypoints.Length == 0 || waypoints[currentWaypointIndex] == null) return;
        Vector3 target = waypoints[currentWaypointIndex].transform.position;
        if (Vector2.Distance(transform.position, target) < .1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            if (waypoints[currentWaypointIndex] == null) return;
            target = waypoints[currentWaypointIndex].transform.position;
        }
        Vector3 scale = initialScale;
        scale.x = Mathf.Abs(scale.x) * (target.x < transform.position.x ? -1 : 1);
        transform.localScale = scale;
        transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * speed);
    }
}

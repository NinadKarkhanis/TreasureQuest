using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothness = 8f;
    private void LateUpdate()
    {
        if (player == null) return;
        Vector3 target = new Vector3(player.position.x, player.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, target, 1f - Mathf.Exp(-smoothness * Time.deltaTime));
    }
}

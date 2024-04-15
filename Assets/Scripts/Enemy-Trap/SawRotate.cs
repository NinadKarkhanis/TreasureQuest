using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawRotate : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private Vector3 scale = new Vector3(13.65f, 13.65f, 13.65f);

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,360 * speed* Time.deltaTime);
        // Set the scale of the object
        transform.localScale = scale;
    }
}

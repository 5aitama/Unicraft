using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraExpoRotation : MonoBehaviour
{
    public float speed = 1f;

    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}

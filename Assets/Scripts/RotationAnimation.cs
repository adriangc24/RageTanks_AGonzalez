using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAnimation : MonoBehaviour
{
    public int speed;
    void Start()
    {
    }

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.back, speed * Time.deltaTime);

    }
}

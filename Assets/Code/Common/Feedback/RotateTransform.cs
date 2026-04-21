using System;
using UnityEngine;

public class RotateTransform : MonoBehaviour
{
    [Header("Configuration")] 
    [SerializeField] private Vector3 rotationVector;

    private void Update()
    {
        transform.Rotate(rotationVector, Space.Self);
    }
}

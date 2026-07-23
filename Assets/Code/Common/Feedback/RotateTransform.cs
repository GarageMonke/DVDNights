using System;
using UnityEngine;

public class RotateTransform : MonoBehaviour, ITransformRotator
{
    [Header("Configuration")] 
    [SerializeField] private Vector3 rotationVector;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool isEnabled = true;
    private int _frameCounter;
    
    [Header("Retro Effect")]
    [SerializeField] [Range(0, 50)] private int frameSkip = 0;

    private void Update()
    {
        if (!isEnabled)
        {
            return;
        }
        
        if (frameSkip == 0)
        {
            SmoothRotate();
        }
        
    }

    private void FixedUpdate()
    {
        if (!isEnabled)
        {
            return;
        }
        
        if (frameSkip > 0)
        {
            FrameRotate();
        }
    }

    private void SmoothRotate()
    {
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(rotationVector);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }

    private void FrameRotate()
    {
        _frameCounter++;

        if (_frameCounter > frameSkip)
        {
            _frameCounter = 0;

            int framesToSimulate = frameSkip + 1;
            for (int i = 0; i < framesToSimulate; i++)
            {
                transform.Rotate(rotationVector * framesToSimulate, Space.Self);
            }
        }
    }
    
    public void EnableRotation()
    {
        isEnabled = true;
    }

    public void DisableRotation()
    {
        isEnabled = false;
    }
}

public interface ITransformRotator
{
    public void EnableRotation();
    public void DisableRotation();
}

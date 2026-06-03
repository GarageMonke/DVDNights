using UnityEngine;

public class RotateTransform : MonoBehaviour
{
    [Header("Configuration")] 
    [SerializeField] private Vector3 rotationVector;
    
    [Header("Retro Effect")]
    [SerializeField] [Range(0, 50)] private int frameSkip; // 0 = smooth
    [SerializeField] private float smoothSpeed = 5f;
    private int _frameCounter;
    
    private void Update()
    {
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(rotationVector);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);

        // _frameCounter++;
        //     
        // if (_frameCounter > frameSkip)
        // {
        //     _frameCounter = 0;
        //     
        //     int framesToSimulate = frameSkip + 1;
        //     for (int i = 0; i < framesToSimulate; i++)
        //     {
        //         transform.Rotate(rotationVector * framesToSimulate, Space.Self);
        //     }
        // }
    }
    
}

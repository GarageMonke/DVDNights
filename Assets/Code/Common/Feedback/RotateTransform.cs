using UnityEngine;

public class RotateTransform : MonoBehaviour
{
    [Header("Configuration")] 
    [SerializeField] private Vector3 rotationVector;
    
    [Header("Retro Effect")]
    [SerializeField] [Range(0, 50)] private int frameSkip; // 0 = smooth
    private int _frameCounter;
    
    private void Update()
    {
        if (frameSkip == 0)
        {
            transform.Rotate(rotationVector, Space.Self);
            return;
        }

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
    
}

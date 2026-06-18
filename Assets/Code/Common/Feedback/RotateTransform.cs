using UnityEngine;

public class RotateTransform : MonoBehaviour, ITransformRotator
{
    [Header("Configuration")] 
    [SerializeField] private Vector3 rotationVector;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool isEnabled = true;
    private int _frameCounter;
    
    private void Update()
    {
        if (isEnabled)
        {
            Quaternion targetRotation = transform.rotation * Quaternion.Euler(rotationVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
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

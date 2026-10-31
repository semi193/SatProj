using UnityEngine;

public class RotateToCam : MonoBehaviour
{
    [SerializeField] Transform _camera;

    private void FixedUpdate()
    {
        transform.rotation = _camera.rotation * Quaternion.Euler(90, 180, 0);
    }
}

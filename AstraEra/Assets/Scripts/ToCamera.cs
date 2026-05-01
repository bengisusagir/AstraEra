using UnityEngine;

public class ToCamera : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        if (Camera.main != null)
            mainCameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (mainCameraTransform == null)
        {
            if (Camera.main != null)
                mainCameraTransform = Camera.main.transform;
            else
                return;
        }

        transform.LookAt(mainCameraTransform);
    }
}

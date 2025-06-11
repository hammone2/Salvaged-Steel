using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public static PlayerCamera instance;
    public Camera cam;
    public CameraShake camShake;

    [SerializeField] private float cameraSmoothSpeed = 90f;
    private Transform targetPos;

    void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (targetPos != null)
            transform.position = Vector3.Lerp(transform.position, targetPos.position, Time.deltaTime * cameraSmoothSpeed);
    }

    public void NewPosition(Transform newPos)
    {
        targetPos = newPos;
    }

    public void RemovePosition()
    {
        targetPos = null;
    }
}

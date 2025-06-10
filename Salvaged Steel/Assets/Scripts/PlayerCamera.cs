using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public static PlayerCamera instance;
    public Camera cam;
    public CameraShake camShake;

    void Start()
    {
        instance = this;


    }
}

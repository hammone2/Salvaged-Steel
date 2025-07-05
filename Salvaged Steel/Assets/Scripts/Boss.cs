using UnityEngine;

public class Boss : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall")
            Destroy(other.gameObject);
    }
}

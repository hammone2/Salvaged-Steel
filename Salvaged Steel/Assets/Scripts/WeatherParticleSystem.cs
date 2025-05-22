using UnityEngine;

public class WeatherParticleSystem : MonoBehaviour
{
    void Update()
    {
        if (GameManager.instance.player == null)
            return;
        transform.position = new Vector3(GameManager.instance.player.gameObject.transform.position.x, 22.2f, GameManager.instance.player.gameObject.transform.position.z);
    }
}

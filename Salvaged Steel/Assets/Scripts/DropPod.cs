using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPod : MonoBehaviour
{

    void Update()
    {
        if (transform.position.y <= 0)
        {
            SpawnTank();
            
            //add particle effect here later
            Destroy(gameObject);
        }
    }
    private void SpawnTank()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DropPod : MonoBehaviourPun
{
    


    void Start()
    {
        
    }

    
    void Update()
    {
        if (transform.position.y <= 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("SpawnTank", RpcTarget.All);
            }
            //add particle effect here later
            Destroy(gameObject);
        }
    }

    [PunRPC]
    private void SpawnTank()
    {

    }
}

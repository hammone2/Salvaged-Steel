using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public PlayerController player;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ammoText;
    void Update()
    {
        scoreText.text = "Score: " + Global.score;
        ammoText.text = "Ammo: " + player.gun.ammo;
    }
}

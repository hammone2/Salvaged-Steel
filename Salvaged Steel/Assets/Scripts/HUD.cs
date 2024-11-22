using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public PlayerController player;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI hullText;
    public TextMeshProUGUI turretText;
    void Update()
    {
        scoreText.text = "Score: " + player.score;
        ammoText.text = "Ammo: " + player.gun.ammo;
        hullText.text = "Hull Health: " + player.propulsion.GetComponent<HealthComponent>().health;
        turretText.text = "Turret Health: " + player.turret.GetComponent<HealthComponent>().health;
    }
}

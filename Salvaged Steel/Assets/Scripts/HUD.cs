using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HUD : MonoBehaviour
{
    private PlayerController player;
    private HealthComponent propulsionHealth;
    private HealthComponent turretHealth;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI hullText;
    public TextMeshProUGUI turretText;
    public Image turretHealthBar;
    public Image propulsionHealthBar;
    public TextMeshProUGUI respawnText;
    public GameObject respawnScreen;
    public GameObject deathScreen;

    public static HUD instance;
    void Awake()
    {
        instance = this;
    }

    public void Initialize(PlayerController localPlayer)
    {
        player = localPlayer;
    }

    public void InitializeValues()
    {
        UpdateScoreText();
        UpdateAmmoText();
        UpdateLivesText();
        UpdatePropulsionPart();
        UpdateTurretPart();
    }

    public void UpdatePropulsionPart()
    {
        propulsionHealth = player.propulsion.GetComponent<HealthComponent>();
        UpdateHullHealth();
    }

    public void UpdateTurretPart()
    {
        turretHealth = player.turret.GetComponent<HealthComponent>();
        UpdateTurretHealth();
    }

    public void UpdateHullHealth()
    {
        hullText.text = "Hull Health: " + propulsionHealth.health;
        propulsionHealthBar.fillAmount = propulsionHealth.health / propulsionHealth.maxHealth;
    }

    public void UpdateTurretHealth()
    {
        turretText.text = "Turret Health: " + turretHealth.health;
        turretHealthBar.fillAmount = turretHealth.health / turretHealth.maxHealth;
    }

    public void UpdateScoreText()
    {
        scoreText.text = "Score: " + player.score;
    }

    public void UpdateAmmoText()
    {
        ammoText.text = "Ammo: " + player.gun.ammo;
    }

    public void UpdateLivesText()
    {
        livesText.text = "" + player.lives;
    }

    /*public void SetWinText(string winnerName)
    {
        winBackground.gameObject.SetActive(true);
        winText.text = winnerName + " wins";
    }*/

}

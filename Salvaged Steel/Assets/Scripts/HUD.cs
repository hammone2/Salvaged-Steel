using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class HUD : MonoBehaviour
{
    private PlayerController player;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI hullText;
    public TextMeshProUGUI turretText;
    public TextMeshProUGUI respawnText;
    public GameObject respawnScreen;
    public GameObject deathScreen;

    // instance
    public static HUD instance;
    void Awake()
    {
        instance = this;
    }

    public void Initialize(PlayerController localPlayer)
    {
        player = localPlayer;
        //InitializeValues();
    }

    public void InitializeValues()
    {
        UpdateScoreText();
        UpdateAmmoText();
        UpdateHullHealth();
        UpdateTurretHealth();
    }
    /*public void UpdateHealthBar()
    {
        healthBar.value = player.curHp;
    }*/

    /*public void UpdatePlayerInfoText()
    {
        playerInfoText.text = "<b>Alive:</b> " + GameManager.instance.alivePlayers + "\n<b > Kills:</ b > " + player.kills;

    }*/

    public void UpdateHullHealth()
    {
        hullText.text = "Hull Health: " + player.propulsion.GetComponent<HealthComponent>().health;
    }

    public void UpdateTurretHealth()
    {
        turretText.text = "Turret Health: " + player.turret.GetComponent<HealthComponent>().health;
    }

    public void UpdateScoreText()
    {
        scoreText.text = "Score: " + player.score;
    }

    public void UpdateAmmoText()
    {
        ammoText.text = "Ammo: " + player.gun.ammo;
    }

    /*public void SetWinText(string winnerName)
    {
        winBackground.gameObject.SetActive(true);
        winText.text = winnerName + " wins";
    }*/

}

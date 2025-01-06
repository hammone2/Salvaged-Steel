using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using static UnityEngine.GraphicsBuffer;

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
    public TextMeshProUGUI wavesSurvivedText;
    public Image turretHealthBar;
    public Image propulsionHealthBar;
    [SerializeField] private Image turretHealthAnimBar;
    [SerializeField] private Image propHealthAnimBar;
    public TextMeshProUGUI respawnText;
    public GameObject respawnScreen;
    public GameObject deathScreen;
    public GameObject loseScreen;

    private float reduceSpeed = 1f;
    public float lastHitTime;
    private float animPause = 0.25f;

    public static HUD instance;
    void Awake()
    {
        instance = this;
    }

    public void Initialize(PlayerController localPlayer)
    {
        player = localPlayer;
    }

    private void Update()
    {
        if (Time.time - lastHitTime >= animPause)
        {
            turretHealthAnimBar.fillAmount = Mathf.MoveTowards(turretHealthAnimBar.fillAmount, turretHealthBar.fillAmount, reduceSpeed * Time.deltaTime);
            propHealthAnimBar.fillAmount = Mathf.MoveTowards(propHealthAnimBar.fillAmount, propulsionHealthBar.fillAmount, reduceSpeed * Time.deltaTime);
        }
            
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
        hullText.text = (Mathf.Round(propulsionHealth.health * 100f) / 100f) + " HP";
        propulsionHealthBar.fillAmount = propulsionHealth.health / propulsionHealth.maxHealth;
    }

    public void UpdateTurretHealth()
    {
        turretText.text = (Mathf.Round(turretHealth.health * 100f) / 100f) + " HP";
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

    public void SetLoseText(int highestWave)
    {
        loseScreen.SetActive(true);
        wavesSurvivedText.SetText("Waves Survived: " + highestWave);
    }
}

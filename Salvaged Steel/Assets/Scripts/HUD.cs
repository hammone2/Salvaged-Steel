using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SocialPlatforms;

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
    public Image gunAmmoBar;
    [SerializeField] private Image turretHealthAnimBar;
    [SerializeField] private Image propHealthAnimBar;
    public TextMeshProUGUI respawnText;
    public GameObject respawnScreen;
    public GameObject deathScreen;
    public GameObject loseScreen;
    public GameObject itemInfo;

    public GameObject shakable;
    public Vector3 shakableAnchorPos;

    public GameObject damageOverlay;
    private Material damageOverlayMat;
    public Image fade;

    private float reduceSpeed = 1f;
    public float lastHitTime;
    private float animPause = 0.25f;

    public float shakeMagnitude = 0f;
    private float shakeFalloff = 0.07f;

    [Header("Mission Widget Stuff")]
    public GameObject timedWidget;
    public GameObject killWidget;
    public TextMeshProUGUI missionName;
    public TextMeshProUGUI missionProgressText;
    public Image missionProgressBar;


    [HideInInspector] public static HUD instance;
    void Awake()
    {
        instance = this;
        fade.gameObject.SetActive(true);

        shakableAnchorPos = shakable.transform.localPosition;
        damageOverlayMat = damageOverlay.GetComponent<Image>().material;
        damageOverlayMat.SetFloat("_VignetteIntensity", 0f);

        StartCoroutine(StartFadeOut());
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

        if (shakeMagnitude > 0)
            shakeMagnitude -= shakeFalloff;
        if (shakeMagnitude < 0)
            shakeMagnitude = 0;

        shakable.transform.localPosition +=  Random.insideUnitSphere * shakeMagnitude;
        shakable.transform.localPosition = Vector3.Lerp(shakable.transform.localPosition, shakableAnchorPos, 10f * Time.deltaTime);

        float maxOffset = 15f; // max distance allowed from anchor per axis
        shakable.transform.localPosition = new Vector3(

            Mathf.Clamp(shakable.transform.localPosition.x, shakableAnchorPos.x - maxOffset, shakableAnchorPos.x + maxOffset),
            Mathf.Clamp(shakable.transform.localPosition.y, shakableAnchorPos.y - maxOffset, shakableAnchorPos.y + maxOffset),
            Mathf.Clamp(shakable.transform.localPosition.z, shakableAnchorPos.z - maxOffset, shakableAnchorPos.z + maxOffset)

            );

        damageOverlayMat.SetFloat("_VignetteIntensity", Mathf.Lerp(damageOverlayMat.GetFloat("_VignetteIntensity"), 0f, 0.5f * Time.deltaTime));
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
        ammoText.text = "" + player.gun.ammo;
        gunAmmoBar.fillAmount = (float)player.gun.ammo / (float)player.gun.maxAmmo; //using float so the fill doesnt dissapear
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

    public void ScreenShake(float magnitude)
    {
        if (magnitude > shakeMagnitude)
            shakeMagnitude = magnitude * 10;
    }

    public void IncreaseOverlayIntensity()
    {
        if (damageOverlayMat.GetFloat("_VignetteIntensity") > 2.5f)
            damageOverlayMat.SetFloat("_VignetteIntensity", 2.5f);
        else
            damageOverlayMat.SetFloat("_VignetteIntensity", damageOverlayMat.GetFloat("_VignetteIntensity") + 0.5f);
    }

    IEnumerator StartFadeOut()
    {
        yield return new WaitForSeconds(0.75f);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        float duration = 1.25f;
        Color originalColor = fade.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsed / duration);
            fade.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure alpha is exactly 0 at the end
        fade.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
}

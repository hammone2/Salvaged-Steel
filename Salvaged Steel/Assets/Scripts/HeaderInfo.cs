using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HeaderInfo : MonoBehaviourPun
{

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image bar;
    [SerializeField] private Image animBar;
    private float maxValue;
    private float target = 1f;
    private float reduceSpeed = 1f;
    private float lastHitTime;
    private float animPause = 0.25f;

    public void Initialize(string text, float maxVal)
    {
        nameText.text = text;
        maxValue = maxVal;
        bar.fillAmount = 1.0f;
    }

    void Update()
    {
        if (Time.time - lastHitTime >= animPause)
            animBar.fillAmount = Mathf.MoveTowards(animBar.fillAmount, target, reduceSpeed * Time.deltaTime);
    }

    [PunRPC]
    void UpdateHealthBar(float value)
    {
        float newValue = value / maxValue;
        bar.fillAmount = newValue;
        target = newValue;
        lastHitTime = Time.time;
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    public Slider scoreSlider;
    public GameObject fillArea;
    public List<GameObject> star = new List<GameObject>();
    public TextMeshProUGUI countdown;
    public TextMeshProUGUI scoreUpdateType;
    private IEnumerator scoreUpdateCoroutine;
    public void SetScoreSliderValue(float curScore, float maxScore)
    {
        scoreSlider.maxValue = maxScore;
        scoreSlider.value = curScore;
        float percentage = curScore / maxScore;
        if (percentage < 0.05f) fillArea.SetActive(false);
        else fillArea.SetActive(true);
        if (percentage > 0.3f) star[0].SetActive(true);
        if (percentage > 0.6f) star[1].SetActive(true);
        if (percentage > 0.8f) star[2].SetActive(true);
    }

    public void SetCountdown(float time)
    {
        countdown.text = Mathf.Ceil(time).ToString();
        if (time <= 0) countdown.gameObject.SetActive(false);
    }

    public void UpdateScoringType(string type, bool isMiss)
    {
        if(scoreUpdateCoroutine != null) StopCoroutine(scoreUpdateCoroutine);
        scoreUpdateType.text = type;
        scoreUpdateCoroutine = ScoringTextFade(isMiss);
        StartCoroutine(scoreUpdateCoroutine);
    }

    private IEnumerator ScoringTextFade(bool isMiss)
    {
        Color color;
        if (isMiss) color = Color.red;
        else color = new Color(0.47f, 0.95f, 0.54f);
        color.a = 1f;
        while(color.a >= 0f)
        {
            scoreUpdateType.color = color;
            color.a -= Time.deltaTime;
            yield return null;
        }
    }

    private void OnEnable()
    {
        countdown.gameObject.SetActive(true);
        star[0].SetActive(false);
        star[1].SetActive(false);
        star[2].SetActive(false);
    }
}

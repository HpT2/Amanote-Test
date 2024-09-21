using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndUI : MonoBehaviour
{
    public Slider scoreSlider;
    public List<GameObject> star;
    public Button menuBtn;

    private void Start()
    {
        menuBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowMenu();
            GameManager.Instance.tilesManager.ClearObj();
        });
        foreach (GameObject go in star)
        {
            go.SetActive(false);
        }
    }

    private void OnEnable()
    {
        scoreSlider.maxValue = GameManager.Instance.scoreManager.maxScore;
        StartCoroutine(FillSlider());
    }

    public IEnumerator FillSlider()
    {
        float fillScore = 0;
        while(fillScore < GameManager.Instance.scoreManager.curScore)
        {
            yield return null;
            fillScore += scoreSlider.maxValue * Time.deltaTime;
            scoreSlider.value = fillScore;
            float percentage = fillScore / scoreSlider.maxValue;
            if (percentage > 0.3f) star[0].SetActive(true);
            if (percentage > 0.6f) star[1].SetActive(true);
            if (percentage > 0.8f) star[2].SetActive(true);

        }
    }
}

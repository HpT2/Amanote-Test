using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject menuUI;
    public GameObject loadingUI;
    public GameObject IngameUI;
    public GameObject gameEndUI;
    private void Start()
    {
        Instance = this;
    }

    public void CloseMenu()
    {
        menuUI.SetActive(false);
    }

    public void ShowMenu()
    {
        menuUI.SetActive(true);
        CloseIngame();
        CloseLoading();
        CloseGameEnd();
    }

    public void CloseLoading()
    {
        loadingUI.SetActive(false);
    }

    public void ShowLoading()
    {
        loadingUI.SetActive(true);
        CloseIngame();
        CloseMenu();
        CloseGameEnd();
    }

    public void CloseIngame()
    {
        IngameUI.GetComponent<IngameUI>().enabled = false;
        IngameUI.SetActive(false);
    }

    public void ShowIngame()
    {
        CloseMenu();
        CloseGameEnd();
        CloseLoading();
        IngameUI.SetActive(true);
        IngameUI.GetComponent<IngameUI>().enabled = true;
    }

    public void ShowGameEnd()
    {
        gameEndUI.SetActive(true);
        gameEndUI.GetComponent<GameEndUI>().enabled = true;
        CloseMenu();
        CloseLoading();
        CloseIngame();
    }

    public void CloseGameEnd()
    {
        gameEndUI.GetComponent<GameEndUI>().enabled = false;
        gameEndUI.SetActive(false);
    }


    public void SetScoreSliderValue(float curScore, float maxScore)
    {
        IngameUI.GetComponent<IngameUI>().SetScoreSliderValue(curScore, maxScore);
    }

    public void SetCountdownIngame(float time)
    {
        IngameUI.GetComponent<IngameUI>().SetCountdown(time);
    }

    public void UpdateScoringType(string type, bool isMiss)
    {
        IngameUI.GetComponent<IngameUI>().UpdateScoringType(type, isMiss);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject GameOverUI;

    [SerializeField]
    private List<TextMeshProUGUI> rankListTexts;

    CharacterController[] runners;

    bool isGameOver = false;

    private void Awake()
    {
        initializeRunnerRank();
        GameOverUI.SetActive(false);
    }
    private void Start()
    {
        runners = GameManager.Instance.Runners;
    }
    private void FixedUpdate()
    {
        updateRunnerRank();
    }
    private void OnEnable()
    {
        GameManager.Instance.TimeUpEvent += GameOverListener;
    }
    private void OnDisable()
    {
        GameManager.Instance.TimeUpEvent -= GameOverListener;
    }

    private void GameOverListener(object sender, EventArgs e)
    {
        isGameOver = true;
        UpdateGameOverUI();
    }

    private void UpdateGameOverUI()
    {
        foreach(TextMeshProUGUI tmp in GameOverUI.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (tmp.name.Equals("WinnerAnnounce"))
            {
                tmp.text = "Winner List\n";
                int counter = 0;
                for (int i = 0; i < runners.Length; i++)
                {
                    if (runners[i].IsFinished)
                    {
                        tmp.text += (i + 1) + ". " + runners[i].name + "\n";
                        counter++;
                    }
                    if (counter > 2)
                        break;
                }
            }
        }

        GameOverUI.SetActive(true);
    }

    private void initializeRunnerRank()
    {
        foreach (TextMeshProUGUI tmpro in rankListTexts)
        {
            tmpro.text = "Waiting ...";
        }
    }

    private void updateRunnerRank()
    {
        if (runners == null)
            return;

        runners = runners.OrderBy(x => x.destinationDistance).ToArray();
        for (int i = 0; i < Mathf.Min(runners.Length, rankListTexts.Count); i++)
        {
            rankListTexts[i].text = (i + 1) + " - " + runners[i].name + " - " + runners[i].GetComponent<CharacterController>().destinationDistance + " Meters";
        }
    }

    public void playAgain()
    {
        GameOverUI.SetActive(false);
    }
    public void exitGame()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class LevelController : MonoBehaviour
{
    //Singleton
    public static LevelController instance;

    void Awake()
    {
        instance = this;
    }



	[SerializeField] private TextMeshProUGUI scoreUI = null;

	[SerializeField] private UnityEvent OnEndLevel = null; 


	private void Start()
	{
		StartLevel();
	}

	public void StartLevel()
	{
		GameManager.IsRunning = true;

		GameManager.CurrentScore = 0;
		GameManager.CurrentLives = 3;
	}

	public void EndLevel()
	{
		GameManager.IsRunning = false;

		GameManager.AddScoreToScoreboard();

		OnEndLevel.Invoke();
	}

	public void AddScore(int score)
	{
		GameManager.CurrentScore += score;

		UpdateScoreUI();
	}

	public void RemoveLife()
	{
		GameManager.CurrentLives--;

		if(GameManager.CurrentLives <= 0)
		{
			EndLevel();
		}
	}

	public void UpdateScoreUI()
	{
		string text = "";

		//Add "0" in front of score
		for (int i = 0; i < 6 - GameManager.CurrentScore.ToString().Length; i++)
		{
			text += "0";
		}

		text += GameManager.CurrentScore.ToString();

		scoreUI.SetText(text);
	}
}

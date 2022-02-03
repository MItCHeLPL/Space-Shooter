using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScoreboardGenerator : MonoBehaviour
{
	[SerializeField] private GameObject scoreRecordPrefab = null;

	[SerializeField] private int scoreRecordAmountToDisplay = 10;

	[SerializeField] private float baseY = -75;
	[SerializeField] private float yDistanceBetweenRecords = -100;

	private void Start()
	{
		GameManager.LoadScoreboard();

		GenerateScoreboard();
	}

	private void GenerateScoreboard()
	{
		int amountToDisplay = GameManager.Scoreboard.Count < scoreRecordAmountToDisplay ? GameManager.Scoreboard.Count : scoreRecordAmountToDisplay;

		for (int i=0; i<amountToDisplay; i++)
		{
			GameObject record = Instantiate(scoreRecordPrefab, transform);

			//position
			RectTransform rectTransform = record.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, baseY + (yDistanceBetweenRecords * i));

			//name
			TextMeshProUGUI name = record.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			name.SetText(GameManager.Scoreboard[i].name);

			//score
			TextMeshProUGUI score = record.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

			//Add "0" in front of score
			string text = "";
			for (int j = 0; j < 6 - GameManager.Scoreboard[i].score.ToString().Length; j++)
			{
				text += "0";
			}
			text += GameManager.Scoreboard[i].score.ToString();

			score.SetText(text);
		}

		//Change content height
		RectTransform parentRectTransform = transform.parent.GetComponent<RectTransform>();
		parentRectTransform.sizeDelta = new Vector2(parentRectTransform.sizeDelta.x, Mathf.Abs((yDistanceBetweenRecords * amountToDisplay) + baseY));
	}
}

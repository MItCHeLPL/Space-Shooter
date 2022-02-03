using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

public class UIController : MonoBehaviour
{
	[SerializeField] private UnityEvent onLoad = null;

	[SerializeField] private UnityEvent onEsc = null;
	

	private void Start()
	{
		onLoad.Invoke();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			onEsc.Invoke();
		}
	}

	public void LoadScene(int sceneId)
	{
		SceneManager.LoadScene(sceneId);
	}

	public void ChangePlayerName(TMP_InputField input)
	{
		GameManager.PlayerName = input.text;
	}
	public void LoadPlayerName(TMP_InputField input)
	{
		input.text = GameManager.PlayerName;
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}

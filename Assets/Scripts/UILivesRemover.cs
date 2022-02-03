using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILivesRemover : MonoBehaviour
{
	[SerializeField] private List<GameObject> lives;

    public void RemoveLife()
	{
		lives[lives.Count-1].SetActive(false);
		lives.RemoveAt(lives.Count - 1);
	}
}

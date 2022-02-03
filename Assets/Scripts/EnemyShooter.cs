using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
	[SerializeField] private EnemySpawner enemySpawner = null;

	[SerializeField] private Transform playerTransform = null;

	[SerializeField] private AudioController audioController = null;

	[Header("Shooting")]
	[SerializeField] private List<Animator> ammunition = new List<Animator>();
	private int lastAmmoId = -1;

	[SerializeField] private float cooldownBetweenShots = 0.5f;


	private void Start()
	{
		StartCoroutine(Shooting());
	}


	private IEnumerator Shooting()
	{
		while (GameManager.IsRunning)
		{
			//check every enemy starting from oldest, if player is near enemy on X axis, shoot at player
			foreach (EnemyController enemy in enemySpawner.spawnedEnemies)
			{
				if(Mathf.Abs(playerTransform.position.x - enemy.transform.position.x) < (enemy.moveToNewPosIn + 0.1f))
				{
					Shoot(enemy.transform.position);

					break;
				}
			}

			yield return new WaitForSeconds(cooldownBetweenShots);
		}
	}

	private void Shoot(Vector3 startPosition)
	{
		//Cycle through ammo
		int ammoId = lastAmmoId + 1 < ammunition.Count ? lastAmmoId + 1 : 0;

		//Get ammo
		Animator ammo = ammunition[ammoId];

		//Shoot ammo
		ammo.transform.position = startPosition;

		ammo.gameObject.SetActive(true);

		ammo.SetTrigger("Shoot");

		audioController.Play("EnemyShoot");

		lastAmmoId = ammoId;
	}
}

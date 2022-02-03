using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[SerializeField] private GameObject enemyPrefab = null;

	[SerializeField] private AudioController audioController = null;

	[HideInInspector] public List<EnemyController> spawnedEnemies = new List<EnemyController>();

	[Header("Spawn")]
	/* spawnBounds: 
	 * x - left side of the bounds
	 * y - right side of the bounds
	 * z - y spawn postiion 
	 */
	[SerializeField] private Vector3 spawnBounds = new Vector3(-8.0f, 8.0f, 4.5f); //bounds, between witch enemy spawns if spawn in random position is enabled

	[SerializeField] private Vector2 spawnPosition = new Vector2(-8.0f, 4.5f); //spawn position, in witch enemy spawns if spawn in random position is disabled

	[SerializeField] private bool spawnInRandomPosition = false;

	[SerializeField] private int enemiesToSpawn = -1;

	[SerializeField] private float spawnRate = 1.0f;


	[Header("Movement")]
	/* movementBounds: 
	 * x - left side of the bounds
	 * y - right side of the bounds
	 * z - bottom side of the bounds
	 */
	[SerializeField] private Vector3 movementBounds = new Vector3(-8.0f, 8.0f, -2.5f); //enemy can move between these bounds

	[SerializeField] private float moveToNewPosIn = 0.5f;
	

	private void Start()
	{
		StartCoroutine(SpawninEnemiesCoroutine()); //Start spawning enemies
	}


	private IEnumerator SpawninEnemiesCoroutine()
	{
		while((enemiesToSpawn > 0 || enemiesToSpawn == -1) && GameManager.IsRunning)
		{
			if(spawnInRandomPosition)
			{
				spawnPosition = new Vector2(Mathf.RoundToInt(Random.Range(spawnBounds.x, spawnBounds.y)), spawnBounds.z);
			}

			//spawn enemy
			GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
			EnemyController enemyController = enemy.GetComponent<EnemyController>();

			//Pass values to EnemyController
			enemyController.movementBounds = movementBounds;
			enemyController.moveToNewPosIn = moveToNewPosIn;
			enemyController.audioController = audioController;

			//move enemy down and start moving it on it's own
			StartCoroutine(enemyController.MoveToPos(spawnPosition + new Vector2(0, -1.0f), moveToNewPosIn));

			spawnedEnemies.Add(enemyController);

			if(enemiesToSpawn > 0)
			{
				enemiesToSpawn--;
			}

			yield return new WaitForSeconds(spawnRate);
		}
	}


	public void ChangeSpeed(float newSpeed)
	{
		moveToNewPosIn = newSpeed;

		foreach(EnemyController enemy in spawnedEnemies)
		{
			enemy.moveToNewPosIn = newSpeed;
		}
	}
}

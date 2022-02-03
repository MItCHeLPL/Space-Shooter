using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	[HideInInspector] public AudioController audioController = null;

	[Header("Settings")]
	[SerializeField] private float disableAfter = 1.5f; //in what time disable after death

	[SerializeField] private int score = 100; //how much score to add on death

	[Header("Components")]
	private Animator anim = null;
	private BoxCollider2D col = null;
	[SerializeField] private SpriteRenderer enemyModel = null;
	private EnemySpawner enemySpawner = null;


	[Header("Movement")]
	/* movementBounds: 
	 * x - left side of the bounds
	 * y - right side of the bounds
	 * z - bottom side of the bounds
	 */
	[HideInInspector] public Vector3 movementBounds = new Vector3(-8.0f, 8.0f, -2.5f); //enemy can move in these bounds, is passed from EnemySpawner

	[HideInInspector] public float moveToNewPosIn = 0.5f; //in what time does enemy move to new position, is passed from EnemySpawner

	private Vector2 previousPosition = Vector2.zero;


	private void Start()
	{
		anim = GetComponent<Animator>();

		col = GetComponent<BoxCollider2D>();

		enemySpawner = transform.parent.GetComponent<EnemySpawner>();
	}


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("PlayerAmmo"))
		{
			collision.gameObject.SetActive(false);

			Die();
		}
	}


	/* movementBounds: 
	 * x - left side of the screen
	 * y - right side of the screen
	 * z - bottom side of the screen
	 */
	public Vector2 GetNewAvailablePosition(Vector3 movementBounds)
	{
		Vector2 newPos = Vector2.zero;

		//Go down if near bound, can go down, didn't go down last time
		newPos = new Vector2(Mathf.RoundToInt(transform.position.x) + Vector2.down.x, Mathf.RoundToInt(transform.position.y) + Vector2.down.y);
		if ((!IsPositionAvailable(Vector2.left, movementBounds) || !IsPositionAvailable(Vector2.right, movementBounds)) && 
			IsPositionAvailable(Vector2.down, movementBounds) && 
			previousPosition.y != (newPos.y + 2)
			)
		{
			return newPos;
		}

		//Go left or right if not near bound and not going back to previous position
		else
		{
			newPos = new Vector2(Mathf.RoundToInt(transform.position.x) + Vector2.left.x, Mathf.RoundToInt(transform.position.y) + Vector2.left.y);
			if (IsPositionAvailable(Vector2.left, movementBounds) && previousPosition != newPos)
			{
				return newPos;
			}

			newPos = new Vector2(Mathf.RoundToInt(transform.position.x) + Vector2.right.x, Mathf.RoundToInt(transform.position.y) + Vector2.right.y);
			if (IsPositionAvailable(Vector2.right, movementBounds) && previousPosition != newPos)
			{
				return newPos;
			}
		}

		//End game if can't move down
		LevelController.instance.EndLevel();

		return newPos;
	}

	/* movementBounds: 
	 * x - left side of the bounds
	 * y - right side of the bounds
	 * z - bottom side of the bounds
	 */
	private bool IsPositionAvailable(Vector2 direction, Vector3 movementBounds)
	{
		bool flag = false;

		Vector2 newPos = new Vector2(Mathf.RoundToInt(transform.position.x) + direction.x, Mathf.RoundToInt(transform.position.y) + direction.y);

		if (newPos.x >= movementBounds.x && newPos.x <= movementBounds.y && newPos.y >= movementBounds.z)
		{
			flag = true;
		}

		return flag;
	}

	public IEnumerator MoveToPos(Vector2 targetPos, float timeToMove)
	{
		float t = 0f;
		Vector2 startPos = transform.position;
		Vector2 newPos = Vector2.zero;

		while (t < 1 && GameManager.IsRunning)
		{
			t += Time.deltaTime / timeToMove;

			newPos = Vector2.Lerp(startPos, targetPos, t);
			transform.position = newPos;

			yield return null;
		}
		
		if(GameManager.IsRunning)
		{
			previousPosition = new Vector2(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y));

			//Start moving to new position
			StartCoroutine(MoveToPos(GetNewAvailablePosition(movementBounds), moveToNewPosIn));
		}
	}


	public void Die()
	{
		enemySpawner.spawnedEnemies.Remove(GetComponent<EnemyController>()); //remove enemy from spawned enemies

		StopAllCoroutines(); //stop movement

		//Fade out and destroy after time
		StartCoroutine(Diyng());

		//Start spin animation
		anim.SetTrigger("Die");

		col.enabled = false;

		audioController.Play("EnemyDeath");

		LevelController.instance.AddScore(score);

		//update ui
		LevelController.instance.UpdateScoreUI();
	}
	private IEnumerator Diyng()
	{
		//Fade out
		float t = 0f;
		Color startColor = enemyModel.color;
		Color newColor = Color.white;

		while (t < 1)
		{
			t += Time.deltaTime / disableAfter;

			newColor = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0), t);
			enemyModel.color = newColor;

			yield return null;
		}

		//Destroy self
		Destroy(gameObject);
	}
}

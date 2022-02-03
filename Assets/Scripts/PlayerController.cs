using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	[Header("Movement")]
	private bool playerControl = true;

    [SerializeField] private float snapIncrement = 1.0f;

    [SerializeField] private Vector2 movementBounds = new Vector2(-8.0f, 8.0f); //player can move between these bounds

	[SerializeField] private float timeToMove = 0.1f;

	private float lastX = 0.0f;

    private Coroutine moveToPosition = null;
    private bool isMovingToPosition = false;


	[Header("Shooting")]
	[SerializeField] private List<Animator> ammunition = new List<Animator>();
	private int lastAmmoId = -1;

	[SerializeField] private float cooldownBetweenShots = 0.5f;
	private bool canShoot = true;


	[Header("Other")]
	[SerializeField] private AudioController audioController = null;

	[SerializeField] private UnityEvent OnHitRecieve = null;


	public void EnablePlayerControl()
	{
		playerControl = true;
	}
	public void DisablePlayerControl()
	{
		playerControl = false;
	}


	private void Update()
	{
		if(playerControl && GameManager.IsRunning)
		{
			//Move right
			if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
			{
				GetNewXPosition(snapIncrement);
			}

			//Move left
			else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
			{
				GetNewXPosition(-snapIncrement);
			}

			//Shoot
			if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				Shoot();
			}
		}
	}

	private void GetNewXPosition(float direction)
	{
		//Stop moving
		if (isMovingToPosition)
		{
			StopCoroutine(moveToPosition);
		}

		//Get new target position
		float newX = lastX + direction;

		//if in bounds
		if (newX >= movementBounds.x && newX <= movementBounds.y)
		{
			//start moving to new target position
			moveToPosition = StartCoroutine(MoveToX(newX));

			//save new target position
			lastX = newX;
		}
	}

	private IEnumerator MoveToX(float targetX)
	{
		isMovingToPosition = true;

		float t = 0f;
		float startX = transform.position.x;
		float newX = 0;

		while (t < 1 && GameManager.IsRunning)
		{
			t += Time.deltaTime / timeToMove;

			newX = Mathf.Lerp(startX, targetX, t);

			transform.position = new Vector2(newX, transform.position.y);

			yield return null;
		}

		isMovingToPosition = false;
	}


	private void Shoot()
	{
		if(canShoot)
		{
			//Cycle through ammo
			int ammoId = lastAmmoId + 1 < ammunition.Count ? lastAmmoId + 1 : 0;

			//Get ammo
			Animator ammo = ammunition[ammoId];

			//Shoot ammo
			ammo.transform.position = transform.position;
			ammo.gameObject.SetActive(true);
			ammo.SetTrigger("Shoot");

			audioController.Play("PlayerShoot");

			lastAmmoId = ammoId;

			//Cooldown
			StartCoroutine(ShootCooldown());
		}
	}

	private IEnumerator ShootCooldown()
	{
		canShoot = false;

		yield return new WaitForSeconds(cooldownBetweenShots);

		canShoot = true;
	}


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("EnemyAmmo"))
		{
			collision.gameObject.SetActive(false);

			audioController.Play("PlayerDamage");

			OnHitRecieve.Invoke();

			LevelController.instance.RemoveLife();
		}
	}
}

/* Written by Kaz Crowe */
/* PlayerHealth.cs ver 1.0 */
using UnityEngine;
using System.Collections;

namespace UltimateStatusBar_SpaceshipExample
{
	public class PlayerHealth : MonoBehaviour
	{
		static PlayerHealth instance;
		public static PlayerHealth Instance { get { return instance; } }
		bool canTakeDamage = true;

		public int maxHealth = 100;
		float currentHealth = 0;
		public float invulnerabilityTime = 0.5f;

		float currentShield = 0;
		public int maxShield = 100;
		float regenShieldTimer = 0.0f;
		public float regenShieldTimerMax = 1.0f;

		public GameObject explosionParticles;

	
		void Awake ()
		{
			// If the instance variable is already assigned, then there are multiple player health scripts in the scene. Inform the user.
			if( instance != null )
				Debug.LogError( "There are multiple instances of the Player Health script. Assigning the most recent one to Instance." );
			
			// Assign the instance variable as the Player Health script on this object.
			instance = GetComponent<PlayerHealth>();
		}

		void Start ()
		{
			currentHealth = maxHealth;
			currentShield = maxShield;

			UltimateStatusBar.UpdateStatus( "Player", "Health", currentHealth, maxHealth );
			UltimateStatusBar.UpdateStatus( "Player", "Shield", currentShield, maxShield );
		}

		void Update ()
		{
			if( currentShield < maxShield && regenShieldTimer <= 0 )
			{
				currentShield += Time.deltaTime * 5;
				UltimateStatusBar.UpdateStatus( "Player", "Shield", currentShield, maxShield );
			}

			if( regenShieldTimer > 0 )
				regenShieldTimer -= Time.deltaTime;
		}

		public void HealPlayer ()
		{
			currentHealth += ( maxHealth / 4 );
			if( currentHealth > maxHealth )
				currentHealth = maxHealth;
			UltimateStatusBar.UpdateStatus( "Player", "Health", currentHealth, maxHealth );
		}

		public void TakeDamage ( int damage )
		{
			if( canTakeDamage == false )
				return;

			if( currentShield > 0 )
			{
				currentShield -= damage;
				if( currentShield < 0 )
				{
					currentHealth -= currentShield * -1;
					currentShield = 0;
				}
			}
			else
				currentHealth -= damage;

			if( currentHealth <= 0 )
			{
				currentHealth = 0;
				Death();
			}

			canTakeDamage = false;

			StartCoroutine( "Invulnerablilty" );

			StartCoroutine( "ShakeCamera" );

			UltimateStatusBar.UpdateStatus( "Player", "Health", currentHealth, maxHealth );
			UltimateStatusBar.UpdateStatus( "Player", "Shield", currentShield, maxShield );

			regenShieldTimer = regenShieldTimerMax;
		}

		public void Death ()
		{
			GameManager.Instance.ShowDeathScreen();
			GetComponent<PlayerController>().canControl = false;

			GameObject explo = ( GameObject )Instantiate( explosionParticles, transform.position, Quaternion.identity );

			Destroy( explo, 2 );

			Destroy( gameObject );
		}

		IEnumerator Invulnerablilty ()
		{
			yield return new WaitForSeconds( invulnerabilityTime );

			canTakeDamage = true;
		}

		IEnumerator ShakeCamera ()
		{
			// Store the original position of the camera.
			Vector2 origPos = Camera.main.transform.position;
			for( float t = 0.0f; t < 1.0f; t += Time.deltaTime * 2.0f )
			{
				// Create a temporary vector2 with the camera's original position modified by a random distance from the origin.
				Vector2 tempVec = origPos + Random.insideUnitCircle;

				// Apply the temporary vector.
				Camera.main.transform.position = tempVec;

				// Yield until next frame.
				yield return null;
			}

			// Return back to the original position.
			Camera.main.transform.position = origPos;
		}
	}
}
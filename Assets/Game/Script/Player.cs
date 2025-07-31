using UnityEngine;
using System.Collections;
namespace PhoenixaStudio
{
	public class Player : MonoBehaviour
	{
		[Header("Setup Submarine")]
		[Range(0, 100)]
		public int health = 100;
		[Tooltip("This value high, the damage is less")]
		public int defendStrength = 100;
		public float force = 35;
		public float rotationSpeed = 2;
		public float rotationMaxAngle = 10;
		public AudioClip soundDamage;
		public AudioClip soundDestroy;
		public GameObject destroyFX;
		[Header("Sound Engine")]
		public AudioClip soundEngine;
		[Range(0, 0.5f)]
		public float volumeOff = 0.2f;
		[Range(0.2f, 1f)]
		public float volumeOn = 0.5f;
		AudioSource soundEngineFX;
		bool isUseEngine = false;

		[Header("Shield")]
		public GameObject Shield;
		public float timeRecharge = 15;
		[Tooltip("time to use Shield from 100 to 0")]
		public float timeUseShield = 5;
		public float shieldEnegry { get; set; }
		float timeBegin;
		bool isUsingShield = false;

		[Header("Magnet")]
		public Magnet Magnet;
		public float timeMagnet = 7;

		[Header("Speed Boost")]
		public float speedBoostMultiplier = 2f;
		public float speedBoostRampUpTime = 1f;    // Time to reach double speed
		public float speedBoostDuration = 10f;      // Duration at double speed
		public float speedBoostRampDownTime = 1f;   // Time to return to normal
		public GameObject speedBoostParticle;       // Particle effect for speed boost
		private bool isSpeedBoosted = false;
		private float originalSpeed = 0f;
		private float speedBoostTarget = 0f;
		private Coroutine currentSpeedBoostCoroutine = null;
		private float speedBoostYPosition = 0f;     // Store Y position when speed boost starts

		[Header("Rocket")]
		public float fireRate = 0.35f;
		float timeToFire = 0;
		[Tooltip("minimum rockets")]
		public int rocketsDefault = 3;
		public GameObject Rocket;
		public AudioClip soundRocket;
		public Transform BulletSpawnPoint;

		[Header("Gun Fire")]
		public float fireBulletRate = 0.2f;
		float timeToFireBullet = 0;
		[Tooltip("minimum rockets")]
		public int bulletsDefault = 10;
		public GameObject Bullet;
		public AudioClip soundFireBullet;
		public bool allowFireGun { get; set; }
		public Animator GunSpawnPoint;

		[Header("Blink Effect")]
		public float timeBlink = 2;
		public float blinkSpeed = 0.2f;
		public Color blinkColor = Color.blue;
		public SpriteRenderer submarineSprite;

		//the player can be kill by anything, use this when player hit somthing and it's blinking
		bool godMode = false;
		private bool isSpeedBoostInvincible = false;

		Rigidbody2D rig;
		ShakeCamera SharkCamera;
		bool isDead = false;

		void Start()
		{
			//find the ShakeCamera object
			SharkCamera = FindObjectOfType<ShakeCamera>();
			//init the rigidbody
			rig = GetComponent<Rigidbody2D>();
			Shield.SetActive(false);
			Magnet.gameObject.SetActive(false);
			//init the Rocket and bullet
			if (GlobalValue.Rocket < rocketsDefault)
			{
				GlobalValue.Rocket = rocketsDefault;
			}
			if (GlobalValue.Bullet < bulletsDefault)
			{
				GlobalValue.Bullet = bulletsDefault;
			}
			//make the rig to zero gravity
			rig.gravityScale = 0;
			rig.velocity = Vector2.zero;

			soundEngineFX = gameObject.AddComponent<AudioSource>();
			soundEngineFX.clip = soundEngine;
			soundEngineFX.loop = true;
			soundEngineFX.volume = volumeOff;
			soundEngineFX.Play();
		}

		// Update is called once per frame
		void Update()
		{
			if (GameManager.Instance.State != GameManager.GameState.Playing)
				return;

			//calculating the firing time
			timeToFire += Time.deltaTime;
			timeToFireBullet += Time.deltaTime;
			//Check input
			HandleInput();
			//Rotating the player base on the velocity
			transform.rotation = Quaternion.Euler(0, 0, Mathf.Clamp(rig.velocity.y * rotationSpeed, -rotationMaxAngle, rotationMaxAngle));

			//Shield
			if (!isUsingShield)
				shieldEnegry = Mathf.Clamp((Time.time - timeBegin) * 100 / timeRecharge, 0, 100);
			else
			{
				shieldEnegry = 100 - Mathf.Clamp((Time.time - timeBegin) * 100 / timeUseShield, 0, 100);
				if (shieldEnegry == 0)
				{
					isUsingShield = false;
					Shield.GetComponent<Shield>().Close();
					timeBegin = Time.time;
				}
			}
			//play sound engine
			if (isUseEngine)
				soundEngineFX.volume = GlobalValue.isSound ? Mathf.Lerp(soundEngineFX.volume, volumeOn, 0.2f) : 0;
			//play sound engine
			if (rig.velocity.y < 0)
			{
				isUseEngine = false;
				soundEngineFX.volume = GlobalValue.isSound ? Mathf.Lerp(soundEngineFX.volume, volumeOff, 0.2f) : 0;
			}
		}

		//Check keyboard input
		private void HandleInput()
		{
			// Disable all input during speed boost
			if (isSpeedBoosted)
				return;

			if (Input.GetKey(KeyCode.F))
				Fire();

			if (Input.GetKey(KeyCode.Space))
				MoveUp();

			if (Input.GetKey(KeyCode.S))
				UseShield();

			if (Input.GetKey(KeyCode.A))
				FireBullet();
		}

		public void MoveUp()
		{
			//add force to the rigidbody
			rig.AddForce(new Vector2(0, force));
			isUseEngine = true;
		}

		public void Play()
		{
			//init the time and gravity
			timeBegin = Time.time;
			rig.gravityScale = 1.5f;
			rig.velocity = Vector2.zero;

			GlobalValue.PlayGame++;
		}

		void OnTriggerStay2D(Collider2D other)
		{
			if (isDead)
				return;

			if (other.gameObject.CompareTag("Coin"))
			{
				other.gameObject.SendMessage("Collect");

				GlobalValue.Coin++;
			}

			//if the player is blinking or speed boost invincible, don't hit any obstacles
			if (godMode || isSpeedBoostInvincible)
			{
				// Add small camera shake when colliding with obstacles during speed boost
				if (isSpeedBoostInvincible && (other.GetComponent<Enemy>() != null || other.CompareTag("Bomb")))
				{
					SharkCamera.DoShake();
				}
				return;
			}

			var Enemy = other.GetComponent<Enemy>();
			if (Enemy)
			{
				//Take damage
				Damage(Enemy.damage);      
				Enemy.Hit(0);
			}
		}

		public void Damage(int damage)
		{
			//calculating the health value
			health -= (int)(damage * (100f / defendStrength));
			if (health <= 0)
				GameManager.Instance.GameOver();
			else
			{
				StartCoroutine(DoBlinks(timeBlink, blinkSpeed));
				SoundManager.PlaySfx(soundDamage);
			}

			SharkCamera.DoShake();
		}

		public void Die()
		{
			//play sound and reset the rigidbody value
			SoundManager.PlaySfx(soundDestroy);
			soundEngineFX.Stop();
			isDead = true;
			rig.gravityScale = 0;
			rig.velocity = Vector2.zero;
			GetComponent<BoxCollider2D>().enabled = false;
			//disable the engine fx object
			if (transform.Find("EngineFX"))
				transform.Find("EngineFX").gameObject.SetActive(false);

			GunSpawnPoint.transform.parent.gameObject.SetActive(false);
			Instantiate(destroyFX, transform.position, Quaternion.identity);
			gameObject.SetActive(false);
		}

		public void Fire()
		{
			//If the rocket available or not
			if (Rocket == null)
			{
				Debug.LogWarning("There is no Rocket on this Submarine, please add one");
				return;
			}
			//check the remain rocket
			if (GlobalValue.Rocket <= 0)
				return;
			//check the rating time
			if (timeToFire < fireRate)
				return;

			timeToFire = 0;

			if (GameManager.Instance.RocketHolder.transform.childCount == 0)
				Instantiate(Rocket, BulletSpawnPoint.position, Quaternion.identity);
			else
			{
				GameManager.Instance.RocketHolder.transform.GetChild(0).transform.position = BulletSpawnPoint.position;
				GameManager.Instance.RocketHolder.transform.GetChild(0).gameObject.SetActive(true);
			}
			//uodate the rocket
			GlobalValue.Rocket--;
			SoundManager.PlaySfx(soundRocket);
			GlobalValue.UseRocket++;

		}

		public void FireBullet()
		{
			//Fire Gun 
			if (timeToFireBullet < fireBulletRate)
				return;
			//check the remain bullet
			if (GlobalValue.Bullet <= 0)
				return;

			timeToFireBullet = 0;
			if (Bullet == null)
			{
				Debug.LogWarning("There is no Bullet on this Submarine, please add one");
				return;
			}

			if (GameManager.Instance.BulletHolder.transform.childCount == 0)
				Instantiate(Bullet, GunSpawnPoint.gameObject.transform.position, Quaternion.identity);
			else
			{
				GameManager.Instance.BulletHolder.transform.GetChild(0).transform.position = GunSpawnPoint.gameObject.transform.position;
				GameManager.Instance.BulletHolder.transform.GetChild(0).gameObject.SetActive(true);
			}
			GunSpawnPoint.SetTrigger("Fire");
			//Update the bullet
			GlobalValue.Bullet--;
			SoundManager.PlaySfx(soundFireBullet);

		}

		public void UseShield()
		{
			//Check the shield percent
			if (shieldEnegry < 100 || isUsingShield)
				return;
			//active the shield
			Shield.SetActive(true);
			isUsingShield = true;
			timeBegin = Time.time;
			SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundPowerUpShield);
			GlobalValue.UseShield++;
		}

		public void UseMagnet()
		{
			//init the magnet
			Magnet.init(timeMagnet);
			Magnet.gameObject.SetActive(true);
		}

		public void UseSpeedBoost()
		{
			// If speed boost is already active, just reset the 10-second timer
			if (isSpeedBoosted)
			{
				// Stop the current coroutine
				if (currentSpeedBoostCoroutine != null)
				{
					StopCoroutine(currentSpeedBoostCoroutine);
				}
				
				// Restart the speed boost coroutine (this will reset only the 10-second timer)
				currentSpeedBoostCoroutine = StartCoroutine(SpeedBoostTimerResetCoroutine());
				
				// Play sound effect for the refresh
				SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundPowerUpShield);
			}
			else
			{
				// First time collecting speed boost
				// Store original speed and calculate target
				originalSpeed = GameManager.Instance.Speed;
				speedBoostTarget = originalSpeed * speedBoostMultiplier;
				isSpeedBoosted = true;
				isSpeedBoostInvincible = true; // Enable invincibility
				
				// Store current Y position
				speedBoostYPosition = transform.position.y;
				
				// Activate particle effect
				if (speedBoostParticle != null)
				{
					speedBoostParticle.SetActive(true);
				}
				
				// Play sound effect
				SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundPowerUpShield);
				
				// Start the speed boost coroutine
				currentSpeedBoostCoroutine = StartCoroutine(SpeedBoostCoroutine());
			}
		}

		private IEnumerator SpeedBoostCoroutine()
		{
			// Phase 1: Gradually increase speed to double over 1 second
			float elapsedTime = 0f;
			
			while (elapsedTime < speedBoostRampUpTime)
			{
				elapsedTime += Time.deltaTime;
				float t = elapsedTime / speedBoostRampUpTime;
				GameManager.Instance.Speed = Mathf.Lerp(originalSpeed, speedBoostTarget, t);
				
				// Keep player at the same Y position and disable control
				Vector3 currentPos = transform.position;
				transform.position = new Vector3(currentPos.x, speedBoostYPosition, currentPos.z);
				rig.velocity = new Vector2(rig.velocity.x, 0); // Disable vertical movement
				
				yield return null;
			}
			
			// Ensure we're exactly at target speed
			GameManager.Instance.Speed = speedBoostTarget;
			
			// Phase 2: Stay at double speed for 10 seconds
			// During this time, we need to maintain the boosted speed
			float boostStartTime = Time.time;
			while (Time.time - boostStartTime < speedBoostDuration)
			{
				// Keep the speed at the boosted level
				GameManager.Instance.Speed = speedBoostTarget;
				
				// Keep player at the same Y position and disable control
				Vector3 currentPos = transform.position;
				transform.position = new Vector3(currentPos.x, speedBoostYPosition, currentPos.z);
				rig.velocity = new Vector2(rig.velocity.x, 0); // Disable vertical movement
				
				yield return null;
			}
			
			// Phase 3: Gradually reduce speed back to original over 1 second
			elapsedTime = 0f;
			float currentSpeed = GameManager.Instance.Speed;
			
			while (elapsedTime < speedBoostRampDownTime)
			{
				elapsedTime += Time.deltaTime;
				float t = elapsedTime / speedBoostRampDownTime;
				GameManager.Instance.Speed = Mathf.Lerp(currentSpeed, originalSpeed, t);
				
				// Keep player at the same Y position and disable control
				Vector3 currentPos = transform.position;
				transform.position = new Vector3(currentPos.x, speedBoostYPosition, currentPos.z);
				rig.velocity = new Vector2(rig.velocity.x, 0); // Disable vertical movement
				
				yield return null;
			}
			
			// Ensure we're back to original speed
			GameManager.Instance.Speed = originalSpeed;
			isSpeedBoosted = false;
			isSpeedBoostInvincible = false; // Disable invincibility
			currentSpeedBoostCoroutine = null; // Clear the coroutine reference
			
			// Deactivate particle effect
			if (speedBoostParticle != null)
			{
				speedBoostParticle.SetActive(false);
			}
		}

		private IEnumerator SpeedBoostTimerResetCoroutine()
		{
			// Since speed is already at double, just reset the 10-second timer
			// Phase 1: Stay at double speed for 10 seconds
			float boostStartTime = Time.time;
			while (Time.time - boostStartTime < speedBoostDuration)
			{
				// Keep the speed at the boosted level
				GameManager.Instance.Speed = speedBoostTarget;
				
				// Keep player at the same Y position and disable control
				Vector3 currentPos = transform.position;
				transform.position = new Vector3(currentPos.x, speedBoostYPosition, currentPos.z);
				rig.velocity = new Vector2(rig.velocity.x, 0); // Disable vertical movement
				
				yield return null;
			}
			
			// Phase 2: Gradually reduce speed back to original over 1 second
			float elapsedTime = 0f;
			float currentSpeed = GameManager.Instance.Speed;
			
			while (elapsedTime < speedBoostRampDownTime)
			{
				elapsedTime += Time.deltaTime;
				float t = elapsedTime / speedBoostRampDownTime;
				GameManager.Instance.Speed = Mathf.Lerp(currentSpeed, originalSpeed, t);
				
				// Keep player at the same Y position and disable control
				Vector3 currentPos = transform.position;
				transform.position = new Vector3(currentPos.x, speedBoostYPosition, currentPos.z);
				rig.velocity = new Vector2(rig.velocity.x, 0); // Disable vertical movement
				
				yield return null;
			}
			
			// Ensure we're back to original speed
			GameManager.Instance.Speed = originalSpeed;
			isSpeedBoosted = false;
			isSpeedBoostInvincible = false; // Disable invincibility
			currentSpeedBoostCoroutine = null; // Clear the coroutine reference
			
			// Deactivate particle effect
			if (speedBoostParticle != null)
			{
				speedBoostParticle.SetActive(false);
			}
		}

		IEnumerator DoBlinks(float time, float seconds)
		{
			//make the player untouchable
			godMode = true;
			var timer = time;
			while (timer > 0)
			{
				submarineSprite.color = blinkColor;
				yield return new WaitForSeconds(seconds);
				timer -= seconds;
				submarineSprite.color = Color.white;
				yield return new WaitForSeconds(seconds);
				timer -= seconds;
			}

			//make sure renderer is enabled when we exit
			submarineSprite.color = Color.white;
			godMode = false;
		}
	}
}
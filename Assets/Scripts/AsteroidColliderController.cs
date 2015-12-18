using UnityEngine;
using System.Collections;

//destroy an object when it comes in contact
public class AsteroidColliderController : MonoBehaviour {


	public GameObject asteroidExplosion; //asteroid explosion effect
	public GameObject playerExplosion; //player explosion effect
	public GameObject playerSpaceship; //player spaceship object
	public int scoreValue; //player score
	private GameController gameController; //gamecontroller reference
	private AsteroidController asteroidController; //asteroidcontroller script reference
	public GameObject healthPack; //object reference to health pack
	public GameObject shieldPowerUp; //object reference to shield power up
	
	void Start () 
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		gameController = gameControllerObject.GetComponent <GameController>();	
		asteroidController = gameObject.GetComponent<AsteroidController>();
	}
	

	void Update () {
	

	}

	void DestroyAsteroid() {

		//create destruction effect
		Instantiate(asteroidExplosion, transform.position, transform.rotation);

		//if it is a large asteroid then drop a health pack with a 20%
		if (tag == "LargeAsteroid") {

			int chance = Random.Range(1,100); //probability of a powerup dropping
			if(chance <= 20) { //1-20
				Instantiate(healthPack, gameObject.transform.position,gameObject.transform.rotation);
			}
			else if(chance <=40) {//21-40
				Instantiate(shieldPowerUp, gameObject.transform.position, Quaternion.Euler(-90,-90, 0));
			}

		}

		//destroy the asteroid
		Destroy(gameObject);
		
		//update score 
		gameController.AddScore (scoreValue);



	}

	//when an enter trigger fires
	void OnTriggerEnter(Collider other) {


		switch (other.tag) {

		case "Boundary" : //if it is the boundary then do nothing
			break;

		case "Bolt": //if it hits the bolt subtract damage from asteroid's hp
			BoltController boltController = other.gameObject.GetComponent<BoltController>();
			asteroidController.asteroidHP -= boltController.damage;
			if(asteroidController.asteroidHP <= 0)
			{
				DestroyAsteroid();
			}
			Destroy (other.gameObject);
			break;

		case "Shield": //if it hits the shield destroy the asteroid
			DestroyAsteroid();
			break;

		case "Earth" : //if it hits earth then subtract one life
			DestroyAsteroid();
			gameController.UpdateLives(-2);
			break;

		case "Player": 
			DestroyAsteroid();
			PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
			if(!playerController.invincible) //if the player is destructible
			{
				//destroy player effect
				Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
				//destroy the player object
				Destroy(other.gameObject);
				
				if(gameController.lives > 3) //if enough lives to respawn
				{
					//create new player instance with invincibility for 2secs and deduct lives for death
					Vector3 spawnLocation = new Vector3(0,-1.75f,0);
					//create new instance
					gameController.player = (GameObject)Instantiate(playerSpaceship, spawnLocation, Quaternion.Euler(270,0,0));
					//get the player controller script and initialize required values
					PlayerController newPlayerController = gameController.player.GetComponent<PlayerController>();
					newPlayerController.earthPosition = gameController.earthPosition;
					newPlayerController.rotationPoint = gameController.earthPosition;
					//update lives
					gameController.UpdateLives(-3);
				}
				else //end game
				{
					gameController.GameOver ();
				}
			}
			break;

		}
	}
}

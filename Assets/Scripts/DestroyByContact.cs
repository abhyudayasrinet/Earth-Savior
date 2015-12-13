using UnityEngine;
using System.Collections;

//destroy an object when it comes in contact
public class DestroyByContact : MonoBehaviour {


	public GameObject asteroidExplosion; //asteroid explosion effect
	public GameObject playerExplosion; //player explosion effect
	public GameObject playerSpaceship; //player spaceship object
	public int scoreValue; //player score
	private GameController gameController; //gamecontroller reference

	
	void Start () 
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
	}
	

	void Update () {
	

	}

	//when an enter trigger fires
	void OnTriggerEnter(Collider other) {

		//if it is the boundary then do nothing
		if (other.tag == "Boundary")
			return;

		//create destruction effect
		Instantiate(asteroidExplosion, transform.position, transform.rotation);

		//destroy the asteroid
		Destroy(gameObject);

		//update score 
		gameController.AddScore (scoreValue);

		//if the other object is the player
		if (other.tag == "Player")
		{
			PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
			if(!playerController.invincible) 
			{
				//destroy player effect
				Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
				//destroy the player object
				Destroy(other.gameObject);
				
				if(gameController.lives > 3) //if enough lives to respawn
				{
					//create new player instance with invincibility for 2secs and deduct lives for death
					Vector3 spawnLocation = new Vector3(0,-1.75f,0);
					gameController.player = (GameObject)Instantiate(playerSpaceship, spawnLocation, Quaternion.Euler(270,0,0));
					PlayerController newPlayerController = gameController.player.GetComponent<PlayerController>();
					newPlayerController.earthPosition = gameController.earthPosition;
					newPlayerController.rotationPoint = gameController.earthPosition;
					gameController.UpdateLives(-3);
				}
				else //end game
				{
					gameController.GameOver ();
				}
			}
		}

		//if other object is earth
		if (other.tag == "Earth") 
		{
			//update lives
			gameController.UpdateLives(-1);
		}


	}
}

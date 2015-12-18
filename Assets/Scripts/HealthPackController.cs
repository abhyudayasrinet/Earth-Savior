using UnityEngine;
using System.Collections;

public class HealthPackController: MonoBehaviour {

	public Vector3 earthPosition; //location of earth
	public float speed; //speed of healthpack
	public int healthBonus; //hp gained from healthpack
	private GameController gameController; //reference to playerController

	// Use this for initialization
	void Start () {

		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		gameController = gameControllerObject.GetComponent <GameController>();	


	}
	
	// Update is called once per frame
	void Update () {
		
		//make healthpack move towards earth
		transform.position = Vector3.MoveTowards( transform.position, earthPosition, speed * Time.deltaTime);
		
	}

	void OnTriggerEnter(Collider other) {

		//if the health pack collides with the player then give the health bonus
		if (other.gameObject.tag == "Player") {
			Destroy(gameObject);
			gameController.UpdateLives(healthBonus);
		}
	}
}

using UnityEngine;
using System.Collections;

public class MegaBombPowerUpController : MonoBehaviour {



	public Vector3 earthPosition; //location of earth
	public float speed; //speed of movement
	private GameController gameController; //reference to game controller

	void Start () {
		
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		gameController = gameControllerObject.GetComponent <GameController> ();

	}
	
	// Update is called once per frame
	void Update () {

		transform.LookAt (earthPosition); //make the bomb point towards earth
		transform.position = Vector3.MoveTowards( transform.position, earthPosition, speed * Time.deltaTime); //make it move towards earth
	}

	void OnTriggerEnter(Collider other) {

		if (other.tag == "Player") {
			Destroy(gameObject);

			//increment shield count
			gameController.getMegaBomb();

			Destroy(gameObject); //destroy the mega bomb object
		}
	}
}

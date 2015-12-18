using UnityEngine;
using System.Collections;

public class ShieldPowerUpController : MonoBehaviour {

	public Vector3 earthPosition; //location of earth
	public float speed; //speed of healthpack
	public float duration; //duration of shield
	public float rotationSpeed; //speed of rotation of the object
	private PlayerController playerController; //reference to playerController
	

	void Start () {

		//get reference to current player controller
		GameObject playerControllerObject = GameObject.FindWithTag ("Player");
		playerController = playerControllerObject.GetComponent <PlayerController>();

	}

	void Update () {

		//rotate the object around its axis
		transform.RotateAround(transform.position, transform.forward, Time.deltaTime * rotationSpeed);
		//make healthpack move towards earth
		transform.position = Vector3.MoveTowards( transform.position, earthPosition, speed * Time.deltaTime);
		
	}
	
	void OnTriggerEnter(Collider other) {

		//if the power up collides with the player give him shield
		if (other.gameObject.tag == "Player") {

			//give shield to player for duration
			playerController.ActivateShield(duration);
			Destroy(gameObject); //destroy the shield object

		}
	}
}

using UnityEngine;
using System.Collections;

public class MegaBombPowerUpController : MonoBehaviour {



	public Vector3 earthPosition; //location of earth
	public float speed; //speed of movement
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.LookAt (earthPosition); //make the bomb point towards earth
		transform.position = Vector3.MoveTowards( transform.position, earthPosition, speed * Time.deltaTime); //make it move towards earth
	}

	void OnTriggerEnter(Collider other) {

		if (other.tag == "Player") {
			Destroy(gameObject);
			print ("increase mega bombs by 1");
			//increase number of mega bombs by 1
		}
	}
}

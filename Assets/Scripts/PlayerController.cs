using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour {


	public GameObject shot; //hold plasma shot object
	public Transform shotSpawn; //spawn location for plasma shot
	public float fireRate; //rate of firing shots
	public bool fireShots; //bool to hold firing state
	public bool invincible; //the player is indestructible

	private float nextFire; //time before firing next shot


	void Start () {

		fireShots = true; //set space shit to fire shots
		StartCoroutine(Blink (3, 0.5f, 0.5f)); //invulnerability for about 3 secs
		invincible = false; //the player can be destroyed
	}
	
	void Update() {

		if (fireShots && Time.time > nextFire) //if firing shots and time is greater than time at which next shot is to be fired then shoot
		{
			nextFire = Time.time + fireRate;
			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
		}

	}

	//make the player blink while he is invincible
	IEnumerator Blink(int nTimes, float timeOn, float timeOff)
	{
		invincible = true;
		while (nTimes > 0)
		{
			GetComponent<Renderer>().enabled = false;
			yield return new WaitForSeconds(timeOn);
			GetComponent<Renderer>().enabled = true;
			yield return new WaitForSeconds(timeOff);
			nTimes--;
		}
		GetComponent<Renderer>().enabled = true;
		invincible = false;
	}

}

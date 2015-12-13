using UnityEngine;
using System.Collections;


public class AsteroidController : MonoBehaviour {
	
	public Vector3 earthPosition; //location of earth
	public float speed; //speed of asteroid
	public float asteroidHP;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		//make asteroid move towards earth
		transform.position = Vector3.MoveTowards( transform.position, earthPosition, speed * Time.deltaTime);

	}
}

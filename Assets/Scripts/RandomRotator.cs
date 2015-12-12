using UnityEngine;
using System.Collections;

//makes the asteroid rotate/tumble at a random speed
public class RandomRotator : MonoBehaviour 
{
	public float tumble; //set tumble speed
	private Rigidbody rb;
	
	void Start()
	{
		rb = GetComponent<Rigidbody> ();
		rb.angularVelocity = Random.insideUnitSphere * tumble; //set random tumbling speed
	}
	
}

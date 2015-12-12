using UnityEngine;
using System.Collections;

//make earth rotate about its axis
public class EarthRotator : MonoBehaviour {

	private Rigidbody rb;
	public float tumble;
	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		rb.angularVelocity = transform.up * tumble;
	}

}

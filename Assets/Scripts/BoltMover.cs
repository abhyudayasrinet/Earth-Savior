using UnityEngine;
using System.Collections;

public class BoltMover : MonoBehaviour {


	private Rigidbody rb;
	public float speed; //speed of bolt

	void Start () {

		rb = GetComponent<Rigidbody> ();
		rb.velocity = transform.up * speed; //set bolt velocity
	}
	

}

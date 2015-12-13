using UnityEngine;
using System.Collections;

public class BoltController : MonoBehaviour {


	private Rigidbody rb;
	public float speed; //speed of bolt
	public int damage; //damage of bolt

	void Start () {

		rb = GetComponent<Rigidbody> ();
		rb.velocity = transform.up * speed; //set bolt velocity
	}
	

}

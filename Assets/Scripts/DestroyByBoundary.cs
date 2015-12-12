using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour {

	//when something goes out of boundary destroy it
	void OnTriggerExit(Collider other)
	{
		Destroy (other.gameObject);
	}
}

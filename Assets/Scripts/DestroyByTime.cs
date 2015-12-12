using UnityEngine;
using System.Collections;

//destroy the objects after a certain time
public class DestroyByTime : MonoBehaviour {

	public float lifetime;

	void Start () {
		Destroy (gameObject, lifetime);
	}

}

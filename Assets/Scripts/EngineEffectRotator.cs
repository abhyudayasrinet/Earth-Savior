using UnityEngine;
using System.Collections;

public class EngineEffectRotator : MonoBehaviour {

	public GameObject player;

	void Start () {
	
	}
	

	void Update () {
		transform.LookAt (player.transform.right);
		//transform.rotation = Quaternion.LookRotation();
	}
}

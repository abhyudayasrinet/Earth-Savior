using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour {

	private const float INITIAL_SHIELD_DURATION = 5.0f; //initial duration of shield when game starts
	public GameObject shot; //hold plasma shot object
	public Transform shotSpawn; //spawn location for plasma shot
	public float fireRate; //rate of firing shots
	public bool fireShots; //bool to hold firing state
	public bool invincible; //the player is indestructible
	public Transform earthPosition; //position of earth
	public Transform rotationPoint; //point of rotation(earth center)
	public float rotationSpeed; //speed of rotating the ship around earth

	private float nextFire; //time before firing next shot
	private Plane inputTouchPlane; //plane to convert screen coords to world coords on touch
	private GameObject shieldGameObject; //player's shield game object

	void Start () {

		//get reference to shield game object
		Transform[] childTransforms = GetComponentsInChildren<Transform>();
		foreach (Transform transform in childTransforms) {
			if (transform.gameObject.tag == "Shield") {
				shieldGameObject = transform.gameObject;
			}
		}

		fireShots = true; //set space shit to fire shots
		ActivateShield (INITIAL_SHIELD_DURATION); //invulnerability for 5 secs
		invincible = false; //the player can be destroyed
		inputTouchPlane = new Plane(new Vector3(0, 0, 1), new Vector3(0,0,0)); //input plane to convert viewport coords to world coords
	}



	void Update() {

		if (fireShots && Time.time > nextFire) //if firing shots and time is greater than time at which next shot is to be fired then shoot
		{
			nextFire = Time.time + fireRate;
			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
		}

		//detect touch
		if (Input.touchCount > 0 && (Input.GetTouch (0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary) ) {
			
			//print ("touch");
			Vector3 touchPosition = Input.GetTouch(0).position; //get touch screen coords
			
			Ray ray = Camera.main.ScreenPointToRay(touchPosition); //create ray at touch location
			float distance;
			if (inputTouchPlane.Raycast (ray, out distance)) //get intersection with input plane to get world coords
			{
				Vector3 rayEnd = ray.GetPoint(distance); //get touch location world coord
				Vector3 center = earthPosition.position; //center of earth
				Vector3 targetPosition = PointCircleIntersection(center, 1.75f, center, rayEnd); //get target location to rotate to
				
				Vector3 initialPosition = transform.position; //get current coords of player
				
				float angle = Vector3.Angle(initialPosition, targetPosition); //get angle
				if(angle > 0.01f) //move player if angle is greater
				{
					if(initialPosition.y > -3.5f) {
						
						if(initialPosition.x < targetPosition.x) //go right
							transform.RotateAround (rotationPoint.position, Vector3.forward, (-rotationSpeed) * Time.deltaTime);
						else
							transform.RotateAround (rotationPoint.position, Vector3.forward, (rotationSpeed) * Time.deltaTime);
					}
					else {
						
						if(initialPosition.x < 0) //go right
							transform.RotateAround (rotationPoint.position, Vector3.forward, (-rotationSpeed) * Time.deltaTime);
						else
							transform.RotateAround (rotationPoint.position, Vector3.forward, (rotationSpeed) * Time.deltaTime);
					}
				}
			}
		}
		//mouse movement
		if (Input.GetMouseButtonDown (0) || Input.GetMouseButton(0)) { 

			Vector3 mouse_position = Input.mousePosition;
			Ray ray = Camera.main.ScreenPointToRay(mouse_position);
			float distance;
			if (inputTouchPlane.Raycast(ray, out distance))
			{
			
				//get the point that the ray hits the plane
				Vector3 rayEnd = ray.GetPoint(distance);
				
				Vector3 center = earthPosition.position;
				Vector3 targetPosition = PointCircleIntersection(center, 1.75f, center, rayEnd);
				
				Vector3 initialPosition = transform.position;
				float angle = Vector3.Angle(initialPosition, targetPosition);
				if(angle > 0.01f)
				{
					if(initialPosition.y > -3.5f) {
						
						if(initialPosition.x < targetPosition.x) //go right
							transform.RotateAround (rotationPoint.position, Vector3.forward, (-rotationSpeed) * Time.deltaTime);
						else
							transform.RotateAround (rotationPoint.position, Vector3.forward, (rotationSpeed) * Time.deltaTime);
					}
					else {
						
						if(initialPosition.x < 0) //go right
							transform.RotateAround (rotationPoint.position, Vector3.forward, (-rotationSpeed) * Time.deltaTime);
						else
							transform.RotateAround (rotationPoint.position, Vector3.forward, (rotationSpeed) * Time.deltaTime);
					}
				}
				
			}
		}
	}

	//calculates the coordinates to which the player should rotate to when the user has touched the screen
	//center : center of earth
	//radius : radius of rotation along which the player moves
	//rayStart : start point of vector joining center of earth and touch location
	//rayEnd : touch coords
	Vector3 PointCircleIntersection(Vector3 center, float radius, Vector3 rayStart, Vector3 rayEnd)
	{
		//vector calculations to find target coords
		//explanation : http://answers.unity3d.com/questions/869869/method-of-finding-point-in-3d-space-that-is-exactl.html
		
		Vector3 directionRay = rayEnd - rayStart; //get vector pointing to touch coords
		Vector3 centerToRayStart = rayStart - center; //vector from center of earth to start of ray == 0 here
		
		float a = Vector3.Dot(directionRay, directionRay); 
		float b = 2 * Vector3.Dot(centerToRayStart, directionRay);
		float c = Vector3.Dot(centerToRayStart, centerToRayStart) - (radius * radius);
		
		float discriminant = (b * b) - (4 * a * c);
		if (discriminant >= 0) 
		{
			
			discriminant = Mathf.Sqrt (discriminant);
			
			//How far on ray the intersections happen
			float t1 = (-b - discriminant) / (2 * a);
			float t2 = (-b + discriminant) / (2 * a);
			
			Vector3 hitPoint = new Vector3(0,0,0);
			
			if (t1 >= 0) {
				hitPoint = rayStart + (directionRay * t1);
			} else if (t2 >= 0) {
				hitPoint = rayStart + (directionRay * t2);
			}
			
			return hitPoint;
		}		
		return new Vector3(0,0,0);
	}

	//give the player shield for duration
	public void ActivateShield(float duration) {
		StartCoroutine (Shield(duration));
	}

	//make the player blink while he is invincible
	private IEnumerator Shield(float duration)
	{
		shieldGameObject.SetActive(true); //activate shield
		invincible = true; // set invincible true
		yield return new WaitForSeconds (duration); //wait for duration
		invincible = false; // set invincible false
		shieldGameObject.SetActive (false); //remove shield
	}

}

using UnityEngine;
using System.Collections;


[System.Serializable]
public class SpawnValues
{
	public float xMin, xMax, yMin, yMax;
}

public class GameController : MonoBehaviour {

	public SpawnValues spawnValues; //values used to spawn asteroids
	public int asteroidCount; //number of initial asteroids
	public float spawnWait; //time before new asteroid created
	public float startWait; //time before game starts
	public float waveWait; //time before new wave

	public Transform earthPosition; //position of earth
	public GameObject asteroid; //asteroid object
	private bool gameOver; //holds gameover state

	public GameObject player; //player spaceship gameboject
	public Transform rotationPoint; //point of rotation(earth center)
	public float rotationSpeed; //speed of rotating the ship around earth
	public bool playerInvincible; //if player is invincible

	//GUI banners
	public GUIText livesText;
	public GUIText scoreText;
	public GUIText gameOverText;
	public GUIText waveNumberText;

	public int lives; //lives left
	public int score; //current score
	private int highScore; //current highscore
	private int wave; //wave number
	private bool pause; //pause game state
	private string pauseButtonText = "Pause"; //pause button text
	
	private Plane inputTouchPlane; //plane to convert screen coords to world coords on touch

	private PlayerController playerController; //reference to player controller script

	void OnGUI () 
	{
		//code for left and right control buttons to rotate the player
		//not used for now 
		/*
		if (GUI.RepeatButton (new Rect (Screen.width * 0.8f , Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.2f), "Right")) 
		{
			if(!gameOver)
			{
				if(player.transform.position.x < 0 || player.transform.position.y > -4f)
					player.transform.RotateAround (rotationPoint.position, Vector3.forward, (-rotationSpeed) * Time.deltaTime);
				else if(player.transform.position.x > 0)
					player.transform.RotateAround (rotationPoint.position, Vector3.forward, (rotationSpeed) * Time.deltaTime);

			}
		}
		if (GUI.RepeatButton (new Rect (0.0f , Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.2f), "Left")) 
		{
			if(!gameOver)
			{
				if(player.transform.position.x > 0 || player.transform.position.y > -4f)
					player.transform.RotateAround (rotationPoint.position, Vector3.forward, (rotationSpeed) * Time.deltaTime);
				else if(player.transform.position.x < 0)
					player.transform.RotateAround (rotationPoint.position, Vector3.forward, (-rotationSpeed) * Time.deltaTime);
			}
		}
		*/
		if (GUI.Button (new Rect (Screen.width * 0.2f, 0.0f, Screen.width * 0.2f, Screen.height * 0.05f), "Restart"))  //restart button
		{
			Application.LoadLevel (Application.loadedLevel);
		}
		if (GUI.Button (new Rect (Screen.width * 0.6f, 0.0f, Screen.width * 0.2f, Screen.height * 0.05f), pauseButtonText)) //pause button
		{
			if(!gameOver)
			{
				if(!pause)
				{
					//set time scale to 0 to pause game and change text to resume
					pause = true;
					Time.timeScale = 0.0f;
					pauseButtonText = "Resume";
				}
				else
				{
					//set time scale to 1 to resume game and change text to pause
					pause = false;
					Time.timeScale = 1.0f;
					pauseButtonText = "Pause";
				}
			}
		}

	}

	void Start () 
	{
		AudioListener.volume = 0.0f;
		inputTouchPlane = new Plane(new Vector3(0, 0, 1), new Vector3(0,0,0)); //input plane to convert viewport coords to world coords
		highScore = PlayerPrefs.GetInt ("HighScore"); //get the current highscore value
		GameObject playerControllerObject = GameObject.FindWithTag ("Player"); //reference the player game object
		if (playerControllerObject != null) //get the playercontroller script reference
		{
			playerController = playerControllerObject.GetComponent <PlayerController>();
		}
		if (playerController == null)
		{
			Debug.Log ("Cannot find 'Player' script");
		}

		livesText.text = "Lives : " + lives; //set lives banner text
		scoreText.text = "Score : " + score; //set score banner text
		gameOverText.text = ""; //initially gameover banner is empty
		Time.timeScale = 1.0f; //time scale set to 1 for continuous play 
							   //0 for pause
		pause = false;
		pauseButtonText = "Pause"; //pause button text
		wave = 1; //starting wave
		waveNumberText.text = "Wave " + wave; //set wave number information on the banner
		gameOver = false;
		StartCoroutine (SpawnWaves ()); //start spawning waves
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

	void Update () 
	{

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

				Vector3 initialPosition = player.transform.position; //get current coords of player

				float angle = Vector3.Angle(initialPosition, targetPosition); //get angle
				if(angle > 0.01f) //move player if angle is greater
				{
					if(initialPosition.y > -3.5f) {
						
						if(initialPosition.x < targetPosition.x) //go right
							player.transform.RotateAround (rotationPoint.position, Vector3.forward, (-rotationSpeed) * Time.deltaTime);
						else
							player.transform.RotateAround (rotationPoint.position, Vector3.forward, (rotationSpeed) * Time.deltaTime);
					}
					else {
						
						if(initialPosition.x < 0) //go right
							player.transform.RotateAround (rotationPoint.position, Vector3.forward, (-rotationSpeed) * Time.deltaTime);
						else
							player.transform.RotateAround (rotationPoint.position, Vector3.forward, (rotationSpeed) * Time.deltaTime);
					}
//					if(initialPosition.x < targetPosition.x){ //go right
//
//						//limit the rotation to specific point
//						if(player.transform.position.x < 0 || player.transform.position.y > -4f)
//							player.transform.RotateAround (rotationPoint.position, Vector3.forward, (-rotationSpeed) * Time.deltaTime);
//						else if(player.transform.position.x > 0)
//							player.transform.RotateAround (rotationPoint.position, Vector3.forward, (rotationSpeed) * Time.deltaTime);
//					}
//					else {//go left
//						
//						if(player.transform.position.x > 0 || player.transform.position.y > -4f)
//							player.transform.RotateAround (rotationPoint.position, Vector3.forward, (rotationSpeed) * Time.deltaTime);
//						else if(player.transform.position.x < 0)
//							player.transform.RotateAround (rotationPoint.position, Vector3.forward, (-rotationSpeed) * Time.deltaTime);
//						
//					}

				}
			}
		}
		if (Input.GetMouseButtonDown (0) || Input.GetMouseButton(0)) {
			//print ("mouse click");
			Vector3 mouse_position = Input.mousePosition;
			Ray ray = Camera.main.ScreenPointToRay(mouse_position);
			float distance;
			if (inputTouchPlane.Raycast(ray, out distance))
			{
				//print ("ray hit");
				//get the point that the ray hits the plane
				Vector3 rayEnd = ray.GetPoint(distance);

				Vector3 center = earthPosition.position;
				Vector3 targetPosition = PointCircleIntersection(center, 1.75f, center, rayEnd);

				Vector3 initialPosition = player.transform.position;
				float angle = Vector3.Angle(initialPosition, targetPosition);
				if(angle > 0.05f)
				{



					if(initialPosition.y > -3.5f) {

						if(initialPosition.x < targetPosition.x) //go right
							player.transform.RotateAround (rotationPoint.position, Vector3.forward, (-rotationSpeed) * Time.deltaTime);
						else
							player.transform.RotateAround (rotationPoint.position, Vector3.forward, (rotationSpeed) * Time.deltaTime);
					}
					else {

						if(initialPosition.x < 0) //go right
							player.transform.RotateAround (rotationPoint.position, Vector3.forward, (-rotationSpeed) * Time.deltaTime);
						else
							player.transform.RotateAround (rotationPoint.position, Vector3.forward, (rotationSpeed) * Time.deltaTime);
					}
				}

			}
		}
	}

	//called when game ends
	public void GameOver() 
	{
		gameOver = true;
		playerController.fireShots = false; //stop player from firing shots
		if (score > highScore) { //new high score is made
			gameOverText.text = "New High Score!!";
			PlayerPrefs.SetInt ("HighScore", score); //store the new score
		} else {
			gameOverText.text = "Game Over";
		}
	}

	//update the number of lives the player has
	public void UpdateLives(int life)
	{
		if (gameOver)
			return;
		lives += life; //update lives
		livesText.text = "Lives : " + lives; //update lives banners
		if (lives == 0) //if no more lives left end game
			GameOver ();
	}

	//update the score value
	public void AddScore(int value)
	{
		if (gameOver) //if game over then return
			return;
		score += value; //update score
		scoreText.text = "Score : " + score; //update score banner
	}


	//routine to spawn waves
	IEnumerator SpawnWaves()
	{
		yield return new WaitForSeconds (startWait); //intial wait when game begins
		while (true)
		{
			for (int i = 0; i < asteroidCount; i++)
			{
				int choice = Random.Range(0, 2); //choose the edge to appear from 0 - left 1 - top 2 - right
				float x, y;
				//set x and y coords for asteroid
				if(choice == 0)
				{
					x = spawnValues.xMin;
					y = Random.Range (spawnValues.yMin, spawnValues.yMax);
				}
				else if(choice == 1)
				{
					x = Random.Range (spawnValues.xMin, spawnValues.xMax);
					y = spawnValues.yMax;
				}
				else
				{
					x = spawnValues.xMax;
					y = Random.Range (spawnValues.yMin, spawnValues.yMax);
				}
				//create the asteroid
				Vector3 spawnPosition = new Vector3 (x, y, 0.0f);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (asteroid, spawnPosition, spawnRotation);
				yield return new WaitForSeconds (spawnWait); //wait before spawning new asteroid
			}

			wave++; //next wave
			asteroidCount++; //increase number of asteroids
			waveNumberText.text = "Wave " + wave; //update wave banner
			spawnWait -= 0.1f; //decrease intermediate spawn time 
			UpdateLives(1); //increment life

			yield return new WaitForSeconds (waveWait); //wait before next wave


			if (gameOver) //if game over then stop
			{
				break;
			}


		}
	}



	public IEnumerator Blink(int nTimes, float timeOn, float timeOff)
	{
		//float endTime = Time.time + time;
		playerInvincible = true;
		while (nTimes > 0)
		{
			player.GetComponent<Renderer>().enabled = false;
			print ("renedered off top");
			yield return new WaitForSeconds(timeOn);
			player.GetComponent<Renderer>().enabled = true;
			print ("renedered on");
			yield return new WaitForSeconds(timeOff);
			nTimes--;
		}
		player.GetComponent<Renderer>().enabled = true;
		print (".. renedered on");
		playerInvincible = false;
	}
}

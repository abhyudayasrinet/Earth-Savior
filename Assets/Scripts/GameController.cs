using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

[System.Serializable]
public class SpawnValues
{
	public float xMin, xMax, yMin, yMax;
}

public class GameController : MonoBehaviour {

	public SpawnValues spawnValues; //values used to spawn asteroids
	public int asteroidCount; //number of initial asteroids
	public int largeAsteroidCount; //number of large asteroids
	public int asteroidLimit; //number of asteroids in the current wave
	public float spawnWait; //time before new asteroid created
	public float startWait; //time before game starts
	public float waveWait; //time before new wave

	public Transform earthPosition; //position of earth
	public GameObject asteroid; //asteroid object
	public GameObject largeAsteroid; //large asteroid object
	private bool gameOver; //holds gameover state


	public GameObject player; //player spaceship gameboject
	public Transform rotationPoint; //point of rotation(earth center)
	public float rotationSpeed; //speed of rotating the ship around earth

	public GameObject megaBombEffect; //mega bomb object reference
	private EllipsoidParticleEmitter megaBombParticleEmitter; //mega bomb particle emitter

	private int megaBombCount; //number of mega bombs the player has
	private int shieldCount; //number of shield packs the player has

	public int lives; //lives left
	public int score; //current score
	public int shieldDuration; //duration of shield
	private int highScore; //current highscore
	private int wave; //wave number
	private bool pause; //pause game state

	public PlayerController playerController; //reference to player controller script

	//GUI variables
	public Text pauseText; //button stating pause
	public Text scoreText; //text showing score
	public Text livesText; //text showing number of lives
	public Text waveNumberText; //text showing the current wave number
	public Text gameOverText; //text to show game over
	public Text megaBombCountText; //text to show mega bomb count
	public Text shieldCountText; //text to show shield count
	public Text highScoreText; //show highscore
	public Canvas BackMenu; //canvas holding back menu confirmation buttons

	InterstitialAd interstitial;//interstitial ad object

	public void getShield() {
		shieldCount = Mathf.Min (shieldCount + 1, 5);
		shieldCountText.text = shieldCount+"";
	}

	public void ActivateShield() {
		if (shieldCount > 0) {

			GameObject playerControllerObject = GameObject.FindWithTag ("Player"); //reference the player game object
			playerController = playerControllerObject.GetComponent <PlayerController>();
			playerController.ActivateShield (shieldDuration);
			shieldCount -= 1;
			shieldCountText.text = shieldCount+"";
			//Debug.Log (shieldCount);
		}
		//add error blinker message
	}


	public void getMegaBomb() {

		megaBombCount = Mathf.Min (megaBombCount + 1, 5);
		megaBombCountText.text = megaBombCount + "";
	}

	public void ActivateMegaBomb() {

		if (megaBombCount > 0) {
			Vector3 spawnPosition = player.transform.position;
			Quaternion spawnRotation = Quaternion.identity;
			Instantiate (megaBombEffect, spawnPosition, spawnRotation);
			megaBombCount -= 1;
			megaBombCountText.text = megaBombCount + "";
		}
		//add error blinker message
	}

	//when restart is clicked
	public void RestartClick() {
		
		if (!BackMenu.enabled) {
			//Application.LoadLevel (Application.loadedLevel);
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
			if (interstitial.IsLoaded()) {
				interstitial.Show();
			}
		}
	}

	//when pause is clicked
	public void PauseClick() {
		
		if(!gameOver && !BackMenu.enabled)
		{
			if(!pause)
			{
				//set time scale to 0 to pause game and change text to resume
				pause = true;
				Time.timeScale = 0.0f;
				pauseText.text = "Resume";
			}
			else
			{
				//set time scale to 1 to resume game and change text to pause
				pause = false;
				Time.timeScale = 1.0f;
				pauseText.text = "Pause";
			}
		}
	}


	public void BackClick() 
	{
		PauseClick (); //pause the game
		BackMenu.enabled = true;
		gameOverText.text = ""; //remove game over text
	}

	public void BackConfirmClick()
	{
		//Application.LoadLevel(0); //go back to mainmenu
		SceneManager.LoadScene("mainmenu");
		if (interstitial.IsLoaded()) {
			interstitial.Show();
		}
	}

	public void BackCancelClick()
	{
		BackMenu.enabled = false;
		PauseClick (); //resume the game
	}
		
	void Start () 
	{
		GetComponent<AudioSource>().Play (); //play audio
		//AudioListener.volume = 0.0f; //sound muted for debugging
		highScore = PlayerPrefs.GetInt ("HighScore",0); //get the current highscore value


		megaBombCount = 0; //initially set to 0
		shieldCount = 0;

		livesText.text = "Lives " + lives; //set lives banner text
		scoreText.text = "Score : " + score; //set score banner text
		gameOverText.text = ""; //initially gameover banner is empty
		Time.timeScale = 1.0f; //time scale set to 1 for continuous play 
							   //0 for pause
		highScoreText.text = "HIGHSCORE : " +highScore; //display highscore
		pause = false;
		wave = 1; //starting wave
		waveNumberText.text = "Wave " + wave; //set wave number information on the banner
		gameOver = false;
		StartCoroutine (SpawnWaves ()); //start spawning waves

		BackMenu.enabled = false; //keep menu disabled

		RequestInterstitial (); //Request Interstitial ad
	}

	private void RequestInterstitial()
	{
		string adUnitId = "ca-app-pub-4192002242677873/1267225744";


		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd(adUnitId);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		interstitial.LoadAd(request);
	}

	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Escape)) { //if back button is pressed
			//Application.LoadLevel(0); //go back to mainmenu
			SceneManager.LoadScene("mainmenu");
			if (interstitial.IsLoaded()) {
				interstitial.Show();
			}
		}
	}

	//called when game ends
	public void GameOver() 
	{
		gameOver = true; //set game over to true
		lives = 0; //set lives to 0 in case they are greater than 0
		livesText.text = "Lives : " + lives; //update lives banners
		GameObject playerControllerObject = GameObject.FindWithTag ("Player"); //reference the player game object
		playerController = playerControllerObject.GetComponent <PlayerController>();
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

	//generate spawn position for asteroid
	Vector3 GetSpawnPosition()
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
		return new Vector3 (x, y, 0);
	}
	//routine to spawn waves
	IEnumerator SpawnWaves()
	{
		yield return new WaitForSeconds (startWait); //intial wait when game begins
		while (true)
		{
			int largeAsteroidsLeft = largeAsteroidCount;
			asteroidLimit = asteroidCount;
			while(asteroidLimit > 0)
			{
				//create large asteroid
				if(largeAsteroidsLeft > 0 && Random.Range (0,1) == 0)
				{
					//create large asteroid
					Vector3 spawnPosition = GetSpawnPosition(); //get asteroid spawn location
					Quaternion spawnRotation = Quaternion.identity;
					Instantiate (largeAsteroid, spawnPosition, spawnRotation);
					yield return new WaitForSeconds (spawnWait); //wait before spawning new asteroid
					asteroidLimit--;
					largeAsteroidsLeft--;
				}
				else
				{
					//create regular asteroid
					Vector3 spawnPosition = GetSpawnPosition(); //get asteroid spawn location
					Quaternion spawnRotation = Quaternion.identity;
					Instantiate (asteroid, spawnPosition, spawnRotation);
					yield return new WaitForSeconds (spawnWait); //wait before spawning new asteroid
					asteroidLimit--;
				}

			}

			wave++; //next wave
			asteroidCount++; //increase number of asteroids
			waveNumberText.text = "Wave " + wave; //update wave banner
			spawnWait -= 0.1f; //decrease intermediate spawn time 
			UpdateLives(1); //increment life
			if(wave%3 == 0) //add a large asteroid every third wave
				largeAsteroidCount++;
			yield return new WaitForSeconds (waveWait); //wait before next wave


			if (gameOver) //if game over then stop
			{
				break;
			}


		}
	}




}

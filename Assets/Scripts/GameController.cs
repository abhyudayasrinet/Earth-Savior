﻿using UnityEngine;
using UnityEngine.UI;
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

	public int lives; //lives left
	public int score; //current score
	private int highScore; //current highscore
	private int wave; //wave number
	private bool pause; //pause game state

	private PlayerController playerController; //reference to player controller script

	//GUI variables
	public Text pauseText; //button stating pause
	public Text scoreText; //text showing score
	public Text livesText; //text showing number of lives
	public Text waveNumberText; //text showing the current wave number
	public Text gameOverText; //text to show game over

	//when restart is clicked
	public void RestartClick() {

		Application.LoadLevel (Application.loadedLevel);
	}

	//when pause is clicked
	public void PauseClick() {
		if(!gameOver)
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
		Application.LoadLevel(0); //go back to mainmenu
	}

	void Start () 
	{
		AudioListener.volume = 0.0f; //sound muted for debugging
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

		livesText.text = "Lives " + lives; //set lives banner text
		scoreText.text = "Score : " + score; //set score banner text
		gameOverText.text = ""; //initially gameover banner is empty
		Time.timeScale = 1.0f; //time scale set to 1 for continuous play 
							   //0 for pause
		pause = false;
		wave = 1; //starting wave
		waveNumberText.text = "Wave " + wave; //set wave number information on the banner
		gameOver = false;
		StartCoroutine (SpawnWaves ()); //start spawning waves
	}

	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Escape)) { //if back button is pressed
			Application.LoadLevel(0); //go back to mainmenu
		}
	}

	//called when game ends
	public void GameOver() 
	{
		gameOver = true; //set game over to true
		lives = 0; //set lives to 0 in case they are greater than 0
		livesText.text = "Lives : " + lives; //update lives banners
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




}

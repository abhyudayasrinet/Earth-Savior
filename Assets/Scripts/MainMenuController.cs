using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using NativeAlert;

public class MainMenuController : MonoBehaviour {

	public Canvas quitMenu; //are you sure quit menu reference
	public Canvas settingsMenu; //settings menu reference
	public Canvas mainMenu; //main menu reference
	public Canvas instructionsMenu; //instructions menu reference
	public Text highScoreText; //text showing the high score
	public Slider soundSlider; //sound volume slider in settings menu

	public GameObject instruction1_text; //initial text instruction
	public GameObject instruction2_text; //megabomb text
	public GameObject instruction2_image; //megabomb image
	public GameObject instruction3_text; //shield text
	public GameObject instruction3_image; //shield image
	public GameObject instruction4_text; //health pack text
	public GameObject instruction4_image; //health pack image
	public GameObject nextInstruction; //next instruction button

	//public Text logText;
	BannerView bannerView; //banner view ad

	void Start () {

		//intially have exit and settings menu disabled
		quitMenu.enabled = false; 
		instructionsMenu.enabled = false;
		settingsMenu.enabled = false;
		soundSlider.value = PlayerPrefs.GetFloat ("gameVolume", 1.0f); //set slider to set volume
		AudioListener.volume = soundSlider.value;
		int highScore = PlayerPrefs.GetInt ("HighScore",0); //get the current highscore value
		highScoreText.text = "HighScore : " + highScore; 
		GetComponent<AudioSource> ().Play(); //play music

		RequestBanner (); //request ad
	}

	public void RateButtonClick() {
		
		Application.OpenURL ("market://details?id=com.ggwp.earthsavior"); //open play store 
	}

	private void RequestBanner()
	{
		
		string adUnitId = "ca-app-pub-4192002242677873/6684317346";

		//logText.text = adUnitId;

		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
		// Create an empty ad request.
		AdRequest request2 = new AdRequest.Builder().Build();
		// Load the banner with the request.
		bannerView.LoadAd(request2);
	}

	//destroy the ad once scene changes
	void OnDestroy() {
		bannerView.Destroy (); //destroy ad
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape)) { //if back button is pressed
			if (quitMenu.enabled) //if quit menu is displayed remove it
				NoClick ();
			else if (settingsMenu.enabled) //is settings menu is showing
				BackClick ();
			else if (instructionsMenu.enabled) //if instructions menu is showing
				InstructionsBackClick ();
			else //display quit menu
				ExitClick ();

		}
	}

	//instructions button clicked
	//show instructions
	public void InstructionsClick() {

		mainMenu.enabled = false;
		instructionsMenu.enabled = true;
		bannerView.Hide (); //hide ad for space

		//show only first instruction set
		instruction1_text.SetActive(true);
		instruction2_text.SetActive(false);
		instruction2_image.SetActive(false);
		instruction3_text.SetActive(false);
		instruction3_image.SetActive(false);
		instruction4_text.SetActive(false);
		instruction4_image.SetActive(false);
		nextInstruction.SetActive(true);
	}

	//back to menu from instructions
	public void InstructionsBackClick() {
		
		mainMenu.enabled = true;
		instructionsMenu.enabled = false;
		bannerView.Show (); //show ad

	}

	//next button clicked on instructions menu
	public void NextInstructionClick() {

		//remove first instruction and show next set of instructions
		instruction1_text.SetActive(false);
		instruction2_text.SetActive(true);
		instruction2_image.SetActive(true);
		instruction3_text.SetActive(true);
		instruction3_image.SetActive(true);
		instruction4_text.SetActive(true);
		instruction4_image.SetActive(true);
		nextInstruction.SetActive(false);

	}


	//settings button clicked
	public void SettingsClick() {
		settingsMenu.enabled = true;
		mainMenu.enabled = false;
	}

	//sound slider is moved
	public void SoundSliderMoved() {
		AudioListener.volume = soundSlider.value;
		PlayerPrefs.SetFloat ("gameVolume", soundSlider.value);
	}

	//settings menu back clicked
	public void BackClick() {
		settingsMenu.enabled = false;
		mainMenu.enabled = true;
	}

	//exit button pressed
	public void ExitClick() {
		quitMenu.enabled = true;
		mainMenu.enabled = false;
	}

	//no pressed in the quit menu
	public void NoClick() {
		quitMenu.enabled = false;
		mainMenu.enabled = true;
	}

	//yes pressed in the quit menu
	public void YesClick() {

		Application.Quit ();

	}


	//play clicked
	public void PlayClick() {
		SceneManager.LoadScene("main"); //start game
	}
	
}

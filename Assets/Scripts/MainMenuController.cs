using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;


public class MainMenuController : MonoBehaviour {

	public Canvas quitMenu; //are you sure quit menu reference
	public Canvas settingsMenu; //settings menu reference
	public Canvas mainMenu; //main menu reference
	public Canvas instructionsMenu; //instructions menu reference
	public Button play; //play button 
	public Button exit; //exit button
	public Button back; //back button in settings menu
	public Slider soundSlider; //sound volume slider in settings menu

	//public Text logText;
	BannerView bannerView; //banner view ad

	void Start () {

		//intially have exit and settings menu disabled
		quitMenu.enabled = false; 
		instructionsMenu.enabled = false;
		settingsMenu.enabled = false;
		soundSlider.value = PlayerPrefs.GetFloat ("gameVolume", 1.0f); //set slider to set volume
		GetComponent<AudioSource> ().Play(); //play music

		RequestBanner (); //request ad
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

		//logText.text = "over";
	}

	//destroy the ad once scene changes
	void OnDestroy() {
		bannerView.Destroy (); //destroy ad
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape)) { //if back button is pressed
			if(quitMenu.enabled) //if quit menu is displayed remove it
				NoClick();
			else if(settingsMenu.enabled)
				BackClick();
			else //display quit menu
				ExitClick ();

		}
	}

	//instructions button clicked
	//show instructions
	public void InstructionsClick() {

		mainMenu.enabled = false;
		instructionsMenu.enabled = true;

	}

	//back to menu from instructions
	public void InstructionsBackClick() {
		
		mainMenu.enabled = true;
		instructionsMenu.enabled = false;
	}

	//settings button clicked
	public void SettingsClick() {
		settingsMenu.enabled = true;
		mainMenu.enabled = false;
		//play.enabled = false;
		//exit.enabled = false;
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
		//play.enabled = true;
		//exit.enabled = true;
	}

	//exit button pressed
	public void ExitClick() {
		quitMenu.enabled = true;
		mainMenu.enabled = false;
		//play.enabled = false;
		//exit.enabled = false;
	}

	//no pressed in the quit menu
	public void NoClick() {
		quitMenu.enabled = false;
		mainMenu.enabled = true;
		//play.enabled = true;
		//exit.enabled = true;
	}

	//yes pressed in the quit menu
	public void YesClick() {

		Application.Quit ();

	}


	//play clicked
	public void PlayClick() {
		//Application.LoadLevel("main");
		SceneManager.LoadScene("main");
		//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

	}
	
}

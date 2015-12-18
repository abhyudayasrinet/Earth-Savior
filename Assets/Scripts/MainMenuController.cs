using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour {

	public Canvas quitMenu; //are you sure quit menu reference
	public Canvas settingsMenu; //settings menu reference
	public Button play; //play button 
	public Button exit; //exit button
	public Button back; //back button in settings menu
	public Slider soundSlider; //sound volume slider in settings menu


	void Start () {

		//intially have exit and settings menu disabled
		quitMenu.enabled = false; 
		settingsMenu.enabled = false;
		soundSlider.value = PlayerPrefs.GetFloat ("gameVolume", 1.0f); //set slider to set volume
		GetComponent<AudioSource> ().Play(); //play music
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

	//settings button clicked
	public void SettingsClick() {
		settingsMenu.enabled = true;
		play.enabled = false;
		exit.enabled = false;
	}

	//sound slider is moved
	public void SoundSliderMoved() {
		AudioListener.volume = soundSlider.value;
		PlayerPrefs.SetFloat ("gameVolume", soundSlider.value);
	}

	//settings menu back clicked
	public void BackClick() {
		settingsMenu.enabled = false;
		play.enabled = true;
		exit.enabled = true;
	}

	//exit button pressed
	public void ExitClick() {
		quitMenu.enabled = true;
		play.enabled = false;
		exit.enabled = false;
	}

	//no pressed in the quit menu
	public void NoClick() {
		quitMenu.enabled = false;
		play.enabled = true;
		exit.enabled = true;
	}

	//yes pressed in the quit menu
	public void YesClick() {

		Application.Quit ();

	}


	//play clicked
	public void PlayClick() {
		Application.LoadLevel("main");

	}
	
}

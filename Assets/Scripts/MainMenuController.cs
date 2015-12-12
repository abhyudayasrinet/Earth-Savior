using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour {

	public Canvas quitMenu;
	public Button play;
	public Button exit;

	// Use this for initialization
	void Start () {

		quitMenu = quitMenu.GetComponent<Canvas> ();
		play = play.GetComponent<Button> ();
		exit = play.GetComponent<Button> ();
		quitMenu.enabled = false;

	}
	
	public void ExitClick() {
		quitMenu.enabled = true;
		play.enabled = false;
		exit.enabled = false;
	}

	public void NoClick() {
		quitMenu.enabled = false;
		play.enabled = true;
		exit.enabled = true;
	}

	public void YesClick() {

		Application.Quit ();

	}

	public void PlayClick() {
		Application.LoadLevel("main");

	}
	
}

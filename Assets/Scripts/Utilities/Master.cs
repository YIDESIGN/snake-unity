using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Master : StateMachineBehaviourEx {
	public static Master Instance;
	private Dictionary<string,object> config; 

	
	public enum MasterStates {
		Starting,
		Menu,
		Playing,
		Paused,
		Exiting
	}
	
	void Start() {
		this._mainmenu = this.gameObject.GetComponent<MainMenu>();
		this._menupaused = this.gameObject.GetComponent<MenuPaused>();
		currentState = MasterStates.Starting;
	}

	static void DoNothing(){}
	
	
	public Action OnGUIHandler = DoNothing;
	
	
#region Starting
	public int countdown = 3;
	
	
	private IEnumerator Countdown() {
		while (countdown > 0){
			Debug.Log (Time.time);
			countdown--;
			yield return new WaitForSeconds(1);
		}
		
	}
		void Starting_EnterState () {
			if (Application.loadedLevelName != GameScenes.StartingScreen.ToString())
				Application.LoadLevel(GameScenes.StartingScreen.ToString());
//			this.OnGUIHandler = this.Starting_OnGUI;
			StartCoroutine(this.Countdown());
		}
		void Starting_OnGUI() {
			float width = 400f;
			float height = 30f;
			GUI.Box (new Rect(Screen.width/2-width/2, Screen.height/2-height/2, width,height), "Hey there this is the loading screen");
			width = 60f;
			height = 60;
			GUI.Box (new Rect(Screen.width/2-width/2, Screen.height/2+200-height/2, width,height), this.countdown.ToString());
			if (this.countdown == 0)
				this.currentState = MasterStates.Menu;
		}
			
		void Starting_ExitState () {
		}
#endregion
	
#region Menu
		private MainMenu _mainmenu = null;
		void Menu_EnterState () {
		
			this._mainmenu.Show();
		}
		void Menu_Update() {
			if (snakeloaded)
				snakeloaded = false;
		}
		void Menu_ExitState () {
		}
	public void theGameJustStarted(){
		this.currentState = MasterStates.Playing;
	}
#endregion
	
#region Playing
	private SnakeMover2 mover;
	private bool snakeloaded = false;
	public void snakeLoaded() {
		snakeloaded = true;
	}
	void Playing_EnterState () {
		this._mainmenu.Hide();
		this._menupaused.Hide();
		

	}
	void Playing_Update() {
		if (snakeloaded) 
		{
			snakeloaded = false;
			GameObject.FindGameObjectWithTag("Snake").GetComponent<SnakeMover2>().StartMoving();
		}
		if (Input.GetKeyUp(KeyCode.Escape))
			this.currentState = MasterStates.Paused;

	}
	void Playing_ExitState () {
		
		Debug.Log ("Saving game State haha");
	}
#endregion
	
#region Paused
	private MenuPaused _menupaused = null;
	
	public bool isPaused() {
		return (MasterStates)this.currentState == MasterStates.Paused;
	}
	void Paused_EnterState () {

		Time.timeScale = 0; // Pause the game
		this._menupaused.Show();
		
	}
	void Paused_Update() {
		if (Input.GetKeyUp(KeyCode.Escape))
			this.currentState = MasterStates.Playing;

	}
	void Paused_ExitState () {
		this._menupaused.enabled = false;
		Time.timeScale = 1;
	}
#endregion
	
	
#region Exiting
	
	public void Quit() {
		this.currentState = MasterStates.Exiting;
	}
	
	void Exiting_EnterState () {
		Debug.Log ("Showing Credits");
		countdown = 10;
		Application.LoadLevel(GameScenes.Credits.ToString());
		StartCoroutine(Countdown());
		
	}
	void Exiting_Update() {
		if (countdown == 0 || Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Return))
		{
			Debug.Log ("Quitting the Application");
			Application.Quit();
			// Does not quit in editor
			countdown = -1;
		}
		

	}
	void Exiting_ExitState () {
	}
#endregion
	
}

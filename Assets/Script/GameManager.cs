using UnityEngine;
using System.Collections;
using GooglePlayGames;

public class GameManager : MonoBehaviour {
	
	//transfrom
	public Camera cameraGUI;
	public Camera cameraMain;
	public Transform guiHome;
	public Transform guiRunning;
	public Transform guiEnd;
	public Transform guiPause;
	public Transform bumper;
	public GameObject imgNew;
	public TextMesh txtDistance;
	public TextMesh txtDistanceShadow;
	public TextMesh txtKill;
	public TextMesh txtKillShadow;
	public EnemySpawner spawner;
	public TaskManager taskManager;
	public ItemManager itemManager;
	public ButtonHelper buttonStart;
	public ButtonHelper buttonShield;
	public ButtonHelper buttonHome;
	public ButtonHelper buttonRestart;
	public ButtonHelper buttonHome2;
	public ButtonHelper buttonResume;
	public ButtonHelper buttonGooglePlus;
	public Rank imgRank;
	//sprite
	public Sprite spriteEnemyHead;
	public Sprite spriteEnemyBody;
	public Sprite spriteEnemyFeetStand;
	public Sprite spriteEnemyFeetRun1;
	public Sprite spriteEnemyFeetRun2;
	public Sprite spriteEnemyFeetRun3;
	public Sprite spritePlayerFeetStand;
	public Sprite spritePlayerFeetRun1;
	public Sprite spritePlayerFeetRun2;
	public Sprite spritePlayerFeetRun3;

	//weapon1
	public Weapon[] weapons;
	//weapon2
	public Weapon Thrower;
	//player settings
	public Soldier player;
	public WeaponButton playerDefaultWeapon;
	public Weapon playerWeapon1;
	public Weapon playerWeapon2;
	public Weapon playerWeapon3;
	public Weapon playerWeapon4;
	public Weapon playerWeapon5;
	private Vector3 _playerDefaultPos;
	public Map map;
	public Transform joystick;
	
	//end screen
	public TextMesh counterTotalDistance;
	public TextMesh counterTotalKill;
	public TextMesh counterTotalHeadshot;
	//public SmoothMoves.Sprite txtRank;
	
	//sound
	public AudioClip soundStart;
	
	//game info
	public int distance = 0;
	public int kill = 0;
	public int headshot = 0;
	public int highestScore = 0;
	public int highestDistance = 0;
	public int score = 0;
	public int rank = 0;
	public static string LEADERBOARD_HIGH_SCORE_ID = "CgkI7-6Z0IAIEAIQBw";
	
	public float scoreAnimationTime;
	private float _scoreAnimationTimeRemaining;
	private bool _scoreNewAnimationPlayed;
	private bool _newDistanceRecord;
	
	//game settings
	public float gravityRatio = 100;
	public float gameSpeed;
	public float playerAndCameraOffset = 1000;
	public float soldierMaxHp = 100;
	public float headshotComboTime;
	public float headshotCombo = 0;
	private float _headshotComboTimeRemaining = 0;
	private Vector3 _pauseScreenPausePos;
	private Vector3 _pauseScreenResumePos;
	private float _playerMinX;
	private Color _colorNewScore;
	
	//touch
	private int _maxTouchCount = 2;
	private Hashtable _touchs;
	private float _joystickScreenPointX;
	
	public enum GameState {
		HOME,
		RUNNING,
		PAUSE,
		END
	}
	
	public GameState gameState = GameState.HOME;
	
	void Awake() {
		//Physics.gravity = new Vector3(0, Physics.gravity.y * gravityRatio, 0);
		Vector3 screenLeft = cameraMain.ScreenToWorldPoint (Vector3.zero);
		_playerMinX = screenLeft.x;
		_touchs = new Hashtable();
		_colorNewScore = new Color (255f/255f, 228f/255f , 11f/255f);
		// recommended for debugging:
		//PlayGamesPlatform.DebugLogEnabled = true;
		
		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate();
		buttonGooglePlus.setEnabled(false);
	}
	
	// Use this for initialization
	void Start () {
		restoreData();
		_playerDefaultPos = player.transform.position;
		_pauseScreenPausePos = guiPause.position;
		_pauseScreenResumePos = new Vector3(guiPause.position.x, guiPause.position.y, 2000);
		guiPause.position = _pauseScreenResumePos;
		//player.gameObject.SetActive(false);

		setGameState(gameState);
		//setPlayerSelectedWeapon(playerDefaultWeapon);
	}

	void OnApplicationFocus(bool focusStatus) {
		Debug.Log ("OnApplicationFocus=" + focusStatus);
		if (focusStatus) {
			Social.localUser.Authenticate(success => {
				if (success) {
					Debug.Log ("Authentication successful");
					/*string userInfo = "Username: " + Social.localUser.userName + 
						"\nUser ID: " + Social.localUser.id + 
							"\nIsUnderage: " + Social.localUser.underage;
					Debug.Log (userInfo);*/
					buttonGooglePlus.setEnabled(true);
				}
				else {
					Debug.Log ("Authentication failed");
					buttonGooglePlus.setEnabled(false);
				}
			});
		}
	}
	
	void OnApplicationPause(bool paused) {
		Debug.Log ("OnApplicationPause=" + paused);
		if(paused) {
			//((PlayGamesPlatform) Social.Active).SignOut();
			if(gameState == GameState.RUNNING)
				setGameState(GameState.PAUSE);
		}
	}
	
	// Update is called once per frame
	void Update () {
		checkInput();
		updateUI();
	}
	
	private void updateUI() {
		
		if(gameState == GameState.RUNNING) {
			txtDistance.text = txtDistanceShadow.text = string.Format("{0}m", distance.ToString());
			//txtDistance.text = txtDistanceShadow.text = GPGHelper.getString("I'm Joeyp");
			txtKill.text = txtKillShadow.text = kill.ToString();
			
			if(player.isAvailable()) {
				//player alive
				Vector3 pos = player.transform.position;
				float diff = pos.x - cameraMain.transform.position.x;

				if(diff < _playerMinX) {
					pos.x = cameraMain.transform.position.x + _playerMinX;
					player.transform.position = pos;
				}
			} else {
				//player died
				setGameState(GameState.END);
			}
			
			//headshot combo
			if(_headshotComboTimeRemaining > 0) {
				_headshotComboTimeRemaining -= Time.deltaTime;
			} else {
				headshotCombo = 0;
			}
		} else if(gameState == GameState.END) {
			if(_scoreAnimationTimeRemaining > 0) {
				_scoreAnimationTimeRemaining -= Time.deltaTime;
				_scoreAnimationTimeRemaining = Mathf.Max(_scoreAnimationTimeRemaining, 0);
				float t = (scoreAnimationTime - _scoreAnimationTimeRemaining) / scoreAnimationTime;
				int tempScore = (int)Mathf.Lerp(0, distance, t);
				if(_newDistanceRecord) {
					if(t > 0.7 && !_scoreNewAnimationPlayed) {
						Animator anim = counterTotalDistance.GetComponent<Animator>();
						_scoreNewAnimationPlayed = true;
						anim.SetTrigger("new");
					}

					if(t > 0.7) {
						float total = 1.0f - 0.7f;
						float now = t - 0.7f;

						Color color = Color.Lerp(Color.white, _colorNewScore, now/total);
						counterTotalDistance.color = color;
					}
				}
				counterTotalDistance.text = string.Format("{0}m", tempScore);
			}
		}
	}
	
	private void checkInput() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if(gameState == GameState.RUNNING) {
				setGameState(GameState.PAUSE);
			} else if(gameState == GameState.PAUSE) {
				setGameState(GameState.RUNNING);
			} else if(gameState == GameState.END) {
				setGameState(GameState.HOME);
			} else {
				Application.Quit();
			}
		}

		//touch base device
		if(Input.touchCount > 0) {
			int N = Mathf.Min(_maxTouchCount, Input.touchCount);
			for (int i = 0; i < N; i++) {
				Touch t = Input.GetTouch(i);
				if(t.phase == TouchPhase.Began &&  _touchs.Count < _maxTouchCount) {
					_touchs.Add(t.fingerId, null);
				}
				
				if(_touchs.ContainsKey(t.fingerId))
					handleTouch(t.phase, t.position, t.fingerId);
				
				if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) {
					_touchs.Remove(t.fingerId);
				}
			}
		
		} else {//non-touch base device
			int fingerId = 0;
			if(Input.GetMouseButtonDown(0)) {
				if(!_touchs.ContainsKey(fingerId))
					_touchs.Add(fingerId, null);
				else
					_touchs[fingerId] = null;
				handleTouch(TouchPhase.Began, Input.mousePosition, fingerId);
			} else if(Input.GetMouseButtonUp(0)) {
				handleTouch(TouchPhase.Ended, Input.mousePosition, fingerId);
				if(_touchs.ContainsKey(fingerId))
					_touchs.Remove(fingerId);
			} else if(Input.GetMouseButton(0)) {
				handleTouch(TouchPhase.Moved, Input.mousePosition, fingerId);
			}
			
			if(Input.GetKey ("d"))
				player.moveRight();
			else if(Input.GetKey ("a"))
				player.moveLeft();
			else if(Input.GetKeyDown("s"))
				player.shield();
			else if(Input.GetKeyUp("s"))
				player.unshield();
			else if(Input.GetKeyUp("a") || Input.GetKeyUp("d") || Input.GetKeyUp("s")){
				player.stand();
			}
		}
	}
	
	private void handleTouch(TouchPhase action, Vector3 position, int fingerId) {
		
		object obj = _touchs[fingerId];
		string touchArea = "";
		if(obj == null) {
			Ray ray = cameraGUI.ScreenPointToRay(position);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast (ray, out hit, 100)) {
				touchArea = hit.transform.gameObject.name;
				_touchs[fingerId] = touchArea;
			}
		} else
			touchArea = obj.ToString();
		
		//Debug.Log("ta=" + touchArea + " ac=" + action + " pos=" + position + " id=" + fingerId);
		
		switch(touchArea) {
		case "button_shoot":
			if(action == TouchPhase.Began || action == TouchPhase.Moved || action == TouchPhase.Stationary)
				player.shoot(cameraMain.ScreenToWorldPoint(position));
			else if(action == TouchPhase.Ended || action == TouchPhase.Canceled)
				player.stopAttack();
			break;
		case "ta_flare":
			if(action == TouchPhase.Ended)
				player.shootFlare();
			break;
		case "button_shield":
			buttonShield.handleTouchEvent(action);
			if(action == TouchPhase.Began || action == TouchPhase.Moved || action == TouchPhase.Stationary) {
				player.shield();
			} else if(action == TouchPhase.Ended || action == TouchPhase.Canceled) {
				player.unshield();
			}
			break;
		case "button_joystick":
			if(action == TouchPhase.Began || action == TouchPhase.Moved || action == TouchPhase.Stationary) {
				if(position.x < _joystickScreenPointX)
					player.moveLeft();
				else
					player.moveRight();
			} else {
				Debug.Log("move end");
				player.stand();
			}
			break;
		case "button_weapon_1":
		case "button_weapon_2":
		case "button_weapon_3":
		case "button_weapon_4":
		case "button_weapon_5":
			if(action == TouchPhase.Began) {
				WeaponButton wb = GameObject.Find(touchArea).GetComponent<WeaponButton>();
				setPlayerSelectedWeapon(wb);
			}
			break;
		case "button_start":
			if(buttonStart.handleTouchEvent(action))
				setGameState(GameState.RUNNING);
			break;
		case "button_home":
			if(buttonHome.handleTouchEvent(action)) 
				setGameState(GameState.HOME);
			break;
		case "ranking":
		case "button_g+":
			if(buttonGooglePlus.handleTouchEvent(action)) {
				//Social.ShowAchievementsUI();
				((PlayGamesPlatform) Social.Active).ShowLeaderboardUI(LEADERBOARD_HIGH_SCORE_ID);
			}
			break;
		case "button_restart":
			if(buttonRestart.handleTouchEvent(action)) 
				setGameState(GameState.RUNNING);
			break;
		case "button_resume":
			if(buttonResume.handleTouchEvent(action)) {
				setGameState(GameState.RUNNING);
			}
			break;
		case "button_home2":
			if(buttonHome2.handleTouchEvent(action)) {
				setGameState(GameState.HOME);
			}
			break;
		case "rank":
			if(action == TouchPhase.Ended) {
				Social.ShowAchievementsUI();
			}
			break;
		}
	}
	
	public void setPlayerSelectedWeapon(WeaponButton selected) {
		player.equipWeapon(selected.weapon);
				
		WeaponButton[] weaponButtons = guiRunning.gameObject.GetComponentsInChildren<WeaponButton> ();
		foreach(WeaponButton w in weaponButtons) {
			w.setEquipped(false);
		}
		
		selected.setEquipped(true);
	}
	
	private void pauseGame() {
		Debug.Log("pause");
		Time.timeScale = 0.0f;
		guiPause.position = _pauseScreenPausePos;
	}
	
	private void resumeGame() {
		Debug.Log("resume");
		Time.timeScale = 1.0f;
		guiPause.position = _pauseScreenResumePos;
	}
	
	private void setGameState(GameState state) {
		switch(state) {
		case GameState.HOME:
			goToHome();
			break;
		case GameState.RUNNING:
			if(gameState == GameState.PAUSE)
				resumeGame();
			else
				startGame();
			break;
		case GameState.PAUSE:
			pauseGame();
			break;
		case GameState.END:
			gameOver();
			break;
		}
		gameState = state;
	}
	
	public void incKill() {
		++kill;
	}
	
	public void incHeadShot() {
		++headshot;
		//combo
		++headshotCombo;
		_headshotComboTimeRemaining = headshotComboTime;
	}
	
	private void reset() {
		score = 0;
		distance = 0;
		kill = 0;
		headshot = 0;

		setPlayerSelectedWeapon(playerDefaultWeapon);
		player.transform.position = _playerDefaultPos;
		player.reset();
		playerWeapon2.fill();
		playerWeapon3.fill();
		playerWeapon4.fill();
		playerWeapon5.fill();

		cameraMain.transform.position = new Vector3(0, cameraMain.transform.position.y, cameraMain.transform.position.z);

		bumper.position = new Vector3 (0, bumper.position.y, bumper.position.z);

		map.reset();

		spawner.reset();
		itemManager.reset();
		_joystickScreenPointX = cameraGUI.WorldToScreenPoint(joystick.position).x;

		imgRank.setFlash (false);

	}
	
	private void goToHome() {
		cameraGUI.transform.position = new Vector3(guiHome.position.x, cameraGUI.transform.position.y, cameraGUI.transform.position.z);
		reset();
	}
	
	private void startGame() {
		cameraGUI.transform.position = new Vector3(guiRunning.position.x, cameraGUI.transform.position.y, cameraGUI.transform.position.z);
		reset();
		resumeGame();
		audio.PlayOneShot(soundStart);
	}
	
	private void computeScore() {
		score += distance * 100;
		score += kill * 50;
		score += headshot * 70;
		if (score > highestScore) {
			//imgNew.SetActive(true);
			highestScore = score;
		} else 
			//imgNew.SetActive(false);
		if (distance > highestDistance) {
			highestDistance = distance;
			Social.ReportScore(highestDistance, LEADERBOARD_HIGH_SCORE_ID, (bool success) => {
				if(success) {

				} else {
				}
			});
		}
	}
	
	private void prepareScoreAnimation() {
		_scoreAnimationTimeRemaining = scoreAnimationTime;
		_scoreNewAnimationPlayed = false;
		_newDistanceRecord = distance > highestDistance;
		counterTotalDistance.color = Color.white;
	}
	
	private void goToTask() {
		guiEnd.animation.Stop();
		//guiEnd.position = _endScreenDefaultPos;
		//guiEnd.animation["Go to Task"].wrapMode = WrapMode.ClampForever;
		guiEnd.animation.Play("Go to Task");
	}
	
	public void playTaskAnimation() {
		if(taskManager.isCurrentTaskComplete)
			guiEnd.animation.Play("Next Task");
	}
	
	private void gameOver() {
		prepareScoreAnimation();
		computeScore();
		Task task = taskManager.findLevel (rank + 1);
		if (task != null && task.achieveId != null && taskManager.isComplete (task)) {
			Social.ReportProgress(task.achieveId, 100.0f, (bool success) => {
				if(success) {
					++rank;
					taskManager.setCurrentLevel(rank);
					imgRank.setSprite(task.rank);
					imgRank.setFlash(true);
				}
			});
		}

		//txtRank.SetTextureName(string.Format("rank_{0}_s1", rank));
		//counterTotalDistance.text = string.Format("{0}m", distance);
		counterTotalKill.text = string.Format("{0} kill", kill);
		counterTotalHeadshot.text = string.Format("{0} head shot", headshot);
		cameraGUI.transform.position = new Vector3(guiEnd.position.x, cameraGUI.transform.position.y, cameraGUI.transform.position.z);
		saveData();
		spawner.reset();
	}
	
	private void saveData() {
		PlayerPrefs.SetInt("rank", rank);
		//PlayerPrefs.SetInt("level", taskManager.getCurrentLevel());
		PlayerPrefs.SetInt("highest_score", highestScore);
		PlayerPrefs.SetInt("highest_distance", highestDistance);

		PlayerPrefs.Save();
	}
	
	private void restoreData() {
		highestScore = PlayerPrefs.GetInt("highest_score", 0);
		highestDistance = PlayerPrefs.GetInt("highest_distance", 0);

		rank = PlayerPrefs.GetInt("rank", 0);
		//int level = PlayerPrefs.GetInt("level", 1);
		taskManager.setCurrentLevel(rank);

		Task task = taskManager.findLevel (rank);
		taskManager.setCurrentLevel(rank);
		imgRank.setSprite(task.rank);
		saveData ();
	}

	/*public static void moveObjectToBackground(Transform t) {
		Vector3 pos = t.position;
		pos.z = Random.Range (60, 100);
		t.position = pos; 
	}

	public static void moveObjectToItemLayer(Transform t) {
		Vector3 pos = t.position;
		pos.z = 50;
		t.position = pos; 
	}*/

}

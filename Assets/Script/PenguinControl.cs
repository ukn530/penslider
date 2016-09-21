using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;

public class PenguinControl : MonoBehaviour {

	// ペンギンの向き
	enum Position {Left, Center, Right};
	Position verPos = Position.Center;
	private bool enableCrashAnimation = false;
	private bool enableShowUI = false;
	private Vector3 firstPosition;
	public GameObject button_retry;
	public GameObject result_ui;
	public GameObject start_ui;
	public GameObject score_text;
	private bool shareable = false;
	private bool isJump = false;
	private float yPrev = 0;

	public AudioClip theme;

	private BannerView bannerView;
	bool capture = false;

	private float timeCounter = 0f;

	// 正常状態か混乱状態か
	private bool isRightCondition = true;

	// 無敵状態か
	private bool isInvincibleCondition = false;

	void OnEnable () {
		// このクラスのメソッドをDelegateメソッドに追加
		EventManager.onSpace += onSpace;
		EventManager.onLeft += onLeft;
		EventManager.onRight += onRight;
		EventManager.onUp += onUp;
		EventManager.onRestartButtonDown += onRestartButtonDown;
		EventManager.onHomeButtonDown += onHomeButtonDown;
		EventManager.onRankButtonDown += onRankButtonDown;
		EventManager.onShareButtonDown += onShareButtonDown;
	}

	void OnDisable () {
		// このクラスのメソッドをDelegateメソッドから削除
		EventManager.onSpace -= onSpace;
		EventManager.onLeft -= onLeft;
		EventManager.onRight -= onRight;
		EventManager.onUp -= onUp;
		EventManager.onRestartButtonDown -= onRestartButtonDown;
		EventManager.onHomeButtonDown -= onHomeButtonDown;
		EventManager.onRankButtonDown -= onRankButtonDown;
		EventManager.onShareButtonDown -= onShareButtonDown;
	}

	// 最初のフレームが始まる直前
	void Start () {
		GameManager.state = State.Title;
		firstPosition = transform.position;

		// Game Center認証
		RankingUtility.Auth();

		// バナー読み込み
		RequestBanner ();
	}
	
	// Update is called once per frame
	void Update () {
		movePenguin ();

		if (enableCrashAnimation) {
			crashPenguin ();
		}

		if (enableShowUI) {
			showResult ();
		}
		if (!isRightCondition) {
			ConfusedTimer ();
		}

		if (isInvincibleCondition) {
			InvincibleTimer ();
		}
	}

	void ConfusedTimer () {
		timeCounter += Time.deltaTime;
		if (timeCounter > 5) {
			GetClear ();
			timeCounter = 0;
		}
	}

	void InvincibleTimer (){
		timeCounter += Time.deltaTime;

		if (timeCounter % 1.0f < 0.25f) {
			GetComponent<SpriteRenderer> ().enabled = false;
		} else if (timeCounter % 1.0f < 0.5f) {
			GetComponent<SpriteRenderer> ().enabled = true;
		} else if (timeCounter % 1.0f < 0.75f) {
			GetComponent<SpriteRenderer> ().enabled = false;
		} else {
			GetComponent<SpriteRenderer> ().enabled = true;
		}

		if (timeCounter > 3) {
			GetUninvincible ();
			timeCounter = 0;
			GetComponent<SpriteRenderer> ().enabled = true;
		}
	}

	// ペンギンを左右に移動させる
	void movePenguin(){

		if (GameManager.state == State.Play) {
			// 現在の位置
			Vector3 pos = transform.position;

			// 右向きの場合右に動く
			if (verPos == Position.Right) {
				pos.x += Time.deltaTime * 100 * (6 - transform.position.x) / 6;
			} else if (verPos == Position.Center) {
				pos.x += Time.deltaTime * 100 * (0 - transform.position.x) / 6;
			} else {
				pos.x += Time.deltaTime * 100 * (-6 - transform.position.x) / 6;
			}

			if (isJump) {
				float yTemp = pos.y;
				pos.y += (pos.y - yPrev) - 0.3f;
				yPrev = yTemp;
				Debug.Log ("posy = " + pos.y);
				if (pos.y < 2.5f) {
					pos.y = 2.5f;
					isJump = false;
				}
			}

			// 位置の値を設定
			transform.position = pos;
		}
	}

	void crashPenguin() {
		if (transform.position.z > 23f) {
			// 現在の位置
			Vector3 pos = transform.position;
			Vector3 roa = transform.eulerAngles;
			pos.y += Time.deltaTime * 0.15f * 150;
			pos.z -= Time.deltaTime * 0.3f * 150;
			roa.z += Time.deltaTime * 5f * 150;

			// 位置の値を設定
			transform.position = pos;
			transform.eulerAngles = roa;
		} else {
			enableCrashAnimation = false;
			enableShowUI = true;
		}
	}


	void showResult() {
		if (result_ui.GetComponent<RectTransform> ().anchoredPosition3D.y < -1) {
			Vector3 pos = result_ui.GetComponent<RectTransform> ().anchoredPosition3D;
			pos.y += Time.deltaTime * (-result_ui.GetComponent<RectTransform> ().anchoredPosition3D.y) / 100 * 1000;
			result_ui.GetComponent<RectTransform> ().anchoredPosition3D = pos;
			capture = true;
		} else {
			if (capture) {
				Debug.Log ("lateShare");
				Application.CaptureScreenshot("image.png");
				StartCoroutine(lateShare(1.0F));
				capture = false;
			}
		}
	}

	void resetGame () {
		GameManager.state = State.Play;
		// ペンギンを元の位置に
		transform.position = firstPosition;
		verPos = Position.Center;
		transform.eulerAngles = new Vector3 (0, 0, 0);

		// 状態異常を直す
		speedUp ();
		isRightCondition = true;

		// ボタンを隠す
		enableShowUI = false;
		result_ui.transform.Translate (0, -300 * 2, 0);
		result_ui.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (0, -1300, 0);
		start_ui.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (0, -2300, 0);

		// スコアを隠す
		score_text.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (10, -10, 0);

		// バナーを隠す
		bannerView.Hide();
	}

	// ペンギンが何かに当たったら
	void OnTriggerEnter(Collider collision) {

		if (collision.gameObject.CompareTag ("Mashroom")) {
			// マッシュルームに当たったら

			GetConfused (collision.gameObject);

		} else {
			//障害物に当たったら
			GameManager.state = State.Over;
			enableCrashAnimation = true;

			SoundManager.instance.StopBGM ();

			// スコアを出す
			score_text.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (10, 110, 0);

			// GameCenterにスコア送信
			RankingUtility.ReportScore(ObstacleController.score);

			// バナー表示
			bannerView.Show ();

		}
	}

	// リスタートボタンを押したら
	void onRestartButtonDown() {
		SoundManager.instance.PlayBGM (theme);
		resetGame ();
	}


	void onHomeButtonDown() {
		result_ui.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (0, -1300, 0);
		start_ui.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (0, 0, 0);


		bannerView.Hide ();
	}

	void onRankButtonDown() {
		RankingUtility.ShowLeaderboardUI();
	}


	IEnumerator lateShare(float time) {
		yield return new WaitForSeconds (time);
		shareable = true;

	}

	string imagePath
	{
		get
		{
			return Application.persistentDataPath + "/image.png";
		}
	}

	// シェアボタンを押したら
	void onShareButtonDown() {
		if (shareable){
			Debug.Log ("share");
			string message = string.Format("Pen's Adventureで{0}点とりました。", ObstacleController.score);
			SocialConnector.Share(message, "#PensAdventure @PenPenDream", imagePath);
			shareable = false;
		}
	}

	// スペースを押したら or 二本指でタッチしたら
	void onSpace() {
		resetGame ();
	}
		
	// 左が押されたら or 左スワイプ
	void onLeft () {
		//混乱状態は逆向きに動かす
		if (isRightCondition) {
			goLeft ();
		} else {
			goRight ();
		}
	}

	// 右が押されたら or 右スワイプ
	void onRight () {
		//混乱状態は逆向きに動かす
		if (isRightCondition) {
			goRight ();
		} else {
			goLeft ();
		}
	}


	void onUp () {
		if (transform.position.y == 2.5f) {
			isJump = true;
			yPrev = transform.position.y;
			Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
			transform.position = pos;
		}
	}

	void goLeft () {
		Debug.Log (verPos);
		if (verPos == Position.Right) {
			verPos = Position.Center;
		} else if (verPos == Position.Center) {
			verPos = Position.Left;
		} else {
			verPos = Position.Left;
		}
	}

	void goRight () {
		Debug.Log (verPos);
		if (verPos == Position.Left) {
			verPos = Position.Center;
		} else if (verPos == Position.Center) {
			verPos = Position.Right;
		} else {
			verPos = Position.Right;
		}
	}

	void speedDown () {
		ObstacleController obstacleController = GetComponent<ObstacleController> ();
		obstacleController.maxSpeed = 0.5f;
		obstacleController.speed = 0;
	}

	void speedUp () {
		ObstacleController obstacleController = GetComponent<ObstacleController> ();
		obstacleController.maxSpeed = 1.5f;
		obstacleController.speed = 0;
	}

	void GetConfused (GameObject collision) {
		timeCounter = 0;
		isRightCondition = false;
		speedDown ();

		collision.transform.parent = gameObject.transform;
		Vector3 vec3 = new Vector3(transform.position.x, transform.position.y+20, transform.position.z);
		collision.transform.position = vec3;
	}

	void GetClear () {
		isRightCondition = true;
		speedUp ();
		GetInvincible ();
	}

	void GetInvincible () {
		timeCounter = 0;
		isInvincibleCondition = true;
		GetComponent<BoxCollider> ().enabled = false;
	}


	void GetUninvincible () {
		isInvincibleCondition = false;
		GetComponent<BoxCollider> ().enabled = true;
	}

	private void RequestBanner()
	{
		#if UNITY_ANDROID
		string adUnitId = "INSERT_ANDROID_BANNER_AD_UNIT_ID_HERE";
		#elif UNITY_IPHONE
		string adUnitId = Strings.iosUnitID;
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
		bannerView.Hide ();
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the banner with the request.
		bannerView.LoadAd(request);
	}
}

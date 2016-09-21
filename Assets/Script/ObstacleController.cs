using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleController : MonoBehaviour {

	public float speed = 0;
	public float maxSpeed = 1.5f;
	static public int score = -2;

	private Vector3 bounds;

	// プレハブ化した障害物
	public GameObject preGroundBoth;
	public GameObject preGroundCenter;
	public GameObject preGroundLeft;
	public GameObject preGroundRight;
	public GameObject preGroundNothing;

	// プレハブ化した障害物を入れた配列
	private List<GameObject> preGrounds = new List<GameObject> ();

	// 実際に表示されている障害物の配列
	private List<GameObject> instantiatedGrounds = new List<GameObject> ();

	// きのこ
	public GameObject preMashroom;

	// さる
	public GameObject preMonkey;
	private GameObject monkey;
	private int monkeyDirection;

	// ぞう
	public GameObject preElephant;
	private GameObject elephant;

	// もぐら
	public GameObject preMole;
	private GameObject mole;
	private bool isDoneAnimation = false;


	void OnEnable () {
		EventManager.onSpace += onSpace;
		EventManager.onRestartButtonDown += onRestartButtonDown;
	}

	void OnDisable () {
		EventManager.onSpace -= onSpace;
		EventManager.onRestartButtonDown -= onRestartButtonDown;
	}


	void Start ()
	{
		// 障害物の大きさ
		bounds = preGroundNothing.GetComponent<MeshRenderer>().bounds.size;

		// 最初の障害物を消す
		GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");
		foreach (GameObject ground in grounds) {
			Destroy (ground);
		}

		// 配列にプレハブを入れる
		preGrounds.Add (preGroundNothing);
		preGrounds.Add (preGroundBoth);
		preGrounds.Add (preGroundCenter);
		preGrounds.Add (preGroundLeft);
		preGrounds.Add (preGroundRight);

		// 障害物を配置
		createNewGround ();
	}

	void Update ()
	{
		if (GameManager.state == State.Play) {
			if (speed > -maxSpeed) {
				speed -= 5 * Time.deltaTime;
			}

			for (int i = 0; i < instantiatedGrounds.Count; i++) {
				GameObject obj = instantiatedGrounds [i];

				// 障害物を動かす
				obj.transform.Translate (0, 0, speed * Time.deltaTime * 50);

				// もし画面外に出たら
				if (obj.transform.position.z <= 0) {

					// 出た障害物を削除
					instantiatedGrounds.RemoveAt (i);
					Destroy (obj);

					// 最後の障害物＋障害物一つ分の位置に新しい障害物を追加
					int randomNum = (int)Random.Range (1, preGrounds.Count);
					Vector3 pos = new Vector3 (0, 0, instantiatedGrounds[instantiatedGrounds.Count-1].transform.position.z + bounds.z);
					GameObject newObj = Instantiate ((GameObject)preGrounds[randomNum], pos, Quaternion.identity) as GameObject;
					//配列に追加
					instantiatedGrounds.Add (newObj);

					int randomNum10 = Random.Range (1, 11);
					Debug.Log ("randomNum10: " + randomNum10);
					// きのこを1/10の確率で出す
					if (randomNum10 == 1) {
						int randomNum3 = Random.Range (-1, 2);
						Debug.Log ("mashroom: " + randomNum3);
						Vector3 mashPos = new Vector3 (randomNum3 * 5.85f, 1.2f, pos.z + 0.5f);
						GameObject mashroom = Instantiate ((GameObject)preMashroom, mashPos, Quaternion.identity) as GameObject;
						mashroom.transform.parent = newObj.transform;
					}
					// 猿を1/10の確率で出す
					if (monkey == null) {
						if (randomNum10 == 2 || randomNum10 == 5) {
							int randomNum2 = Random.Range (0, 2);
							Debug.Log ("monkey: " + randomNum2);
							Vector3 monkeyPos = new Vector3 (((float)randomNum2 - 0.5f) * 20, 8.3f, pos.z - 2);
							Vector3 monkeyAngle = new Vector3 (0, (randomNum2 - 1) * 180, 10);

							monkey = Instantiate ((GameObject)preMonkey, monkeyPos, Quaternion.identity) as GameObject;
							monkey.transform.eulerAngles = monkeyAngle;
							monkey.transform.parent = newObj.transform;
						}
					}
					if (elephant == null) {
						// ぞうを1/10の確率で出す
						if (randomNum10 == 3 || randomNum10 == 6) {
							int randomNum2 = Random.Range (0, 2);
							Debug.Log ("elephant: " + randomNum2);
							Vector3 elephantPos = new Vector3 (((float)randomNum2 - 0.5f) * 40, 4.6f, pos.z - 2);
							Vector3 elephantAngle = new Vector3 (0, (randomNum2 - 1) * 180, 0);

							elephant = Instantiate ((GameObject)preElephant, elephantPos, Quaternion.identity) as GameObject;
							elephant.transform.eulerAngles = elephantAngle;
							elephant.transform.parent = newObj.transform;
						}
					}

					if (mole == null) {
						// もぐらを1/10の確率で出す
						if (randomNum10 == 4 || randomNum10 == 7) {
							int randomNum3 = Random.Range (-1, 2);
							Debug.Log ("mole: " + randomNum3);
							Vector3 molePos = new Vector3 (randomNum3 * 5.85f, 1.1f, pos.z - 2);
							mole = Instantiate ((GameObject)preMole, molePos, Quaternion.identity) as GameObject;
							mole.GetComponent<Animator> ().speed = 0;
							mole.transform.parent = newObj.transform;
							isDoneAnimation = false;
						}
					}



					score++;
				}

				if (monkey != null && monkey.transform.parent.position.z <= 80) {
					monkey.transform.Translate (-Time.deltaTime * 2, 0, 0);
					Vector3 roatation = new Vector3 (0, 0, -Time.deltaTime * 2);
					monkey.transform.eulerAngles += roatation;
				}

				if (elephant != null && elephant.transform.parent.position.z <= 80) {
					elephant.transform.Translate (-Time.deltaTime * 6, 0, 0);
				}

				if (mole != null && mole.transform.parent.position.z <= 100) {
					Animator anim = mole.GetComponent<Animator>();
					if (!isDoneAnimation) {
						anim.speed = 1;
						isDoneAnimation = true;
						Debug.Log ("mole");
					}
				}



			}

		} else {
			speed = 0;
		}
	}

	void createNewGround() {
		// 3つ目からランダムでプレハブ化した障害物を配置
		for (int i = 0; i < 7; i++) {
			if (i < 2) {
				GameObject obj = Instantiate ((GameObject)preGrounds [0], new Vector3 (0, 0, i * bounds.z), Quaternion.identity) as GameObject;
				instantiatedGrounds.Add (obj);
			} else {
				GameObject obj = Instantiate ((GameObject)preGrounds[(int)Random.Range(1, preGrounds.Count)], new Vector3(0, 0, i * bounds.z), Quaternion.identity) as GameObject;
				instantiatedGrounds.Add (obj);
			}
		}
	}

	// スペースが押されたら or 二本指でタッチしたら
	void onSpace () {
		resetGame ();
	}

	// リスタートボタンを押したら
	void onRestartButtonDown() {
		resetGame ();
	}

	void resetGame () {
		score = -2;

		for (int i = 0; i < instantiatedGrounds.Count; i++) {
			Destroy (instantiatedGrounds [i]);
		}
		instantiatedGrounds.Clear();

		createNewGround ();
	}
}



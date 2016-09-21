using UnityEngine;
using System.Collections;

public class EventManager: MonoBehaviour
{
	// Delegateの定義
	public delegate void UserAction();
	public static event UserAction onDown;
	public static event UserAction onSpace;
	public static event UserAction onLeft;
	public static event UserAction onRight;
	public static event UserAction onUp;
	public static event UserAction onRestartButtonDown;
	public static event UserAction onShareButtonDown;
	public static event UserAction onHomeButtonDown;
	public static event UserAction onRankButtonDown;

	private Vector2 touchBeganPosition;
	private bool enableAction = true;

	void LateUpdate ()
	{
		// スマホのスワイプ検知のためタッチした瞬間の座標を取得
		if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Began) {
			touchBeganPosition = Input.GetTouch (0).position;
			enableAction = true;
		}

		// スマホで左スワイプorPCで左キー押下
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if (onLeft != null)
				onLeft ();
		} else {
			if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
				if (Input.GetTouch (0).position.x < touchBeganPosition.x - 50 && enableAction) {
					Debug.Log ("touchPos:" + Input.GetTouch(0).position.x + "touchBegan:" + touchBeganPosition.x);
					if (onLeft != null) {
						onLeft ();
						enableAction = false;
					}
				}
			}
		}

		// スマホで右スワイプorPCで右キー押下
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if (onRight != null)
				onRight ();
		} else {
			if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
				if (Input.GetTouch (0).position.x > touchBeganPosition.x + 50 && enableAction) {
					Debug.Log ("touchPos:" + Input.GetTouch(0).position.x + "touchBegan:" + touchBeganPosition.x);
					if (onRight != null) {
						onRight ();
						enableAction = false;
					}
				}
			}
		}

		// スマホで上スワイプorPCで上キー押下
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if (onUp != null)
				onUp ();
		} else {
			if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
				if (Input.GetTouch (0).position.y > touchBeganPosition.y + 50 && enableAction) {
					Debug.Log ("touchPos:" + Input.GetTouch(0).position.y + "touchBegan:" + touchBeganPosition.y);
					if (onUp != null) {
						onUp ();
						enableAction = false;
					}
				}
			}
		}

		// PCでスペースキーを押下orスマホで２本指でタッチ
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (onSpace != null)
				onSpace ();
		}
	}

	public void RestartButtonDown () {
		if (onRestartButtonDown != null)
			onRestartButtonDown ();
	}

	public void ShareButtonDown () {
		if (onShareButtonDown != null)
			onShareButtonDown ();
	}

	public void HomeButtonDown () {
		if (onHomeButtonDown != null)
			onHomeButtonDown ();
	}

	public void RankButtonDown () {
		if (onRankButtonDown != null)
			onRankButtonDown ();
	}
}

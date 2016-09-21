using UnityEngine;
using System.Collections;

public class OpeningPenguin : MonoBehaviour {

	bool stop = false;
	Animator animator;

	bool isGo = false;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		StartCoroutine(LateStart(2F));
	}

	IEnumerator LateStart(float time) {
		yield return new WaitForSeconds (time);
		isGo = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (isGo) {
			if (GetComponent<RectTransform> ().anchoredPosition3D.x > 50) {
				Vector3 pos = GetComponent<RectTransform> ().anchoredPosition3D;
				pos.x -= Time.deltaTime * 500;
				GetComponent<RectTransform> ().anchoredPosition3D = pos;
			} else if (!stop){
				stop = true;
				animator.SetTrigger("stop");
			}
		}
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class fps : MonoBehaviour {

	private Text fpsText;
	private int counter;

	void Awake () {
		fpsText = GetComponent<Text> ();
	}

	void LateUpdate() {
		float fps = 1f / Time.deltaTime;
		if (counter > 5) {
			fpsText.text = "fps: " + (int)fps;
			counter = 0;
		} else {
			counter++;
		}
	}

}

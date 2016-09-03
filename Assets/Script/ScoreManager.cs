using UnityEngine;
using UnityEngine.UI;

public class ScoreManager: MonoBehaviour
{
	private Text scoreText;
	public Text resultText;
	public Text bestText;
	public int highScore;
	void Start () {
		scoreText = GetComponent<Text> ();
		highScore = PlayerPrefs.GetInt ("HighScoreKey", 0);
	}

	void LateUpdate() {
		setScore ();
	}

	public void setScore () {
		int score = ObstacleController.score;
		if (score < 0) {
			score = 0;
		}

		if (highScore < score) {
			highScore = score;
			PlayerPrefs.SetInt ("HighScoreKey", highScore);
			bestText.text = "" + highScore;
		}

		scoreText.text = "Score: " + score;
		resultText.text = "" + score;
		bestText.text = "" + highScore;
	}
}


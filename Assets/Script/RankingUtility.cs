using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

public class RankingUtility {

	//ユーザー認証
	public static void Auth()
	{
		// 認証のため ProcessAuthenticationをコールバックとして登録
		// This call needs to be made before we can proceed to other calls in the Social API
		Social.localUser.Authenticate (ProcessAuthentication);        
	}

	// 認証が完了した時に呼び出される
	// 認証が成功した場合、サーバーからのデータがSocial.localUserにセットされている
	private static void ProcessAuthentication (bool success)
	{
		if (success) {
			Debug.Log ("Authenticated, checking achievements");


		} else {
			Debug.Log ("Failed to authenticate");
		}
	}

	// リーダーボードを表示する
	public static void ShowLeaderboardUI()
	{
		Social.ShowLeaderboardUI();
	}

	// リーダーボードにスコアを送信する
	public static void ReportScore (int score)
	{
		//このIDは任意の登録したものを使う
		string leaderboardID = "rank";

		Debug.Log ("スコア " + score + " を次の Leaderboard に報告します。" + leaderboardID);

		Social.ReportScore (score, leaderboardID, success => {

			Debug.Log(success ? "スコア報告は成功しました" : "スコア報告は失敗しました");
		});
	}
}
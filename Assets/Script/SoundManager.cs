﻿using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioSource efxSource; //効果音用AudioSource
	public AudioSource musicSource; //BGM用AudioSource
	public static SoundManager instance = null;
	//音の高さにバリエーションを付ける用の変数
	public float lowPitchRange = .95f;
	public float highPitchRange = 1.05f;

	//シングルトンの処理
	void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
	}

	public void PlayBGM (AudioClip clip) {
		musicSource.clip = clip;
		musicSource.Play ();
	}

	public void StopBGM () {
		musicSource.Stop ();
	}

	//BGMの再生
	public void PlaySingle (AudioClip clip)
	{
		efxSource.clip = clip;
		efxSource.Play();
	}

	//数種類の効果音を受け取り、ランダムで再生
	public void RandomizeSfx (params AudioClip[] clips)
	{
		//受け取った効果音番号をランダムで指定
		int randomIndex = Random.Range(0, clips.Length);
		//音の高さをランダムで指定
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);
		efxSource.pitch = randomPitch;
		//受け取った効果音を選択
		efxSource.clip = clips[randomIndex];
		//効果音を再生
		efxSource.Play();
	}
}
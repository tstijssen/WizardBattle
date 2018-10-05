using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	public AudioSource m_ButtonSound;



	public void SoundButton()
	{
		m_ButtonSound.Play ();
	}

	public void StartGame()
	{
		PlayerPrefs.SetInt ("AIActive", 0);
		SceneManager.LoadScene ("Game");
	}

	public void StartAIGame()
	{
		PlayerPrefs.SetInt ("AIActive", 1);
		SceneManager.LoadScene ("Game");
	}
}

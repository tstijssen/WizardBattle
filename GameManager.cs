using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// denotes whose current turn it is
public enum ePlayerPriority{PlayerOne, PlayerTwo};

// main manager for game behaviour
public class GameManager : MonoBehaviour {

	int m_AIGame; // int value used to determine whether this game contains an ai (uses player prefs)

	// references to players and UI animations
	public PlayerControl m_PlayerOne, m_PlayerTwo;
	public GameObject m_TurnTorchOne, m_TurnTorchTwo;
	public GameObject m_Player1Idle, m_Player1Victory, m_Player1Death;
	public GameObject m_Player2Idle, m_Player2Victory, m_Player2Death;
	public ePlayerPriority m_Turn;
	public Text m_TurnText;	// top screen text

	public WizardAI m_AI;	// reference to ai

	public AudioClip m_FireSound, m_IceSound, m_VictorySound, m_ButtonSound;
	private AudioSource m_AudioSource;

	// Use this for initialization
	void Start () {
		m_AudioSource = GetComponent<AudioSource> ();
		m_AIGame = PlayerPrefs.GetInt("AIActive");

		// perform starting value setup for players
		m_PlayerOne.Setup ();
		m_PlayerTwo.Setup ();

		// roll for player initiative, 0-3 = player 1, 4-6 = player 2
		int roll = Random.Range (0, 7);

		// activate movement state for current player
		if (roll > 3) 
		{
			m_Turn = ePlayerPriority.PlayerTwo;
			if (m_AIGame == 0)
				m_PlayerTwo.DisplayMovement ();
			else
				m_AI.CalculateMove ();
			m_TurnTorchTwo.SetActive (true);
			m_TurnText.text = "Player 2's Turn";
		}
		else 
		{
			m_Turn = ePlayerPriority.PlayerOne;
			m_PlayerOne.DisplayMovement ();
			m_TurnTorchOne.SetActive (true);
			m_TurnText.text = "Player 1's Turn";
		}

	}

	public void DisplaySpellTiles()
	{
		if (m_Turn == ePlayerPriority.PlayerOne)
		{
			m_PlayerOne.DisplaySpellTiles ();
		}
		else if (m_Turn == ePlayerPriority.PlayerTwo && m_AIGame == 0)
		{
			m_PlayerTwo.DisplaySpellTiles ();
		}
	}

	public void HideSpellTiles()
	{
		if (m_Turn == ePlayerPriority.PlayerOne)
		{
			m_PlayerOne.HideSpellTiles ();
		}
		else if (m_Turn == ePlayerPriority.PlayerTwo)
		{
			m_PlayerTwo.HideSpellTiles ();
		}
	}

	public void PlayIceSound()
	{
		m_AudioSource.PlayOneShot (m_IceSound);
	}

	public void PlayFireSound()
	{
		m_AudioSource.PlayOneShot (m_FireSound);
	}

	public void PlayButtonSound()
	{
		m_AudioSource.PlayOneShot (m_ButtonSound);
	}

	public void TurnSwitch()
	{
		if (m_Turn == ePlayerPriority.PlayerOne) 
		{
			m_Turn = ePlayerPriority.PlayerTwo;
			m_PlayerOne.m_State = WizardState.Casting;
			m_TurnTorchOne.SetActive (false);
			m_TurnTorchTwo.SetActive (true);
			m_TurnText.text = "Player 2's Turn";
			if (m_AIGame == 0)
				m_PlayerTwo.DisplayMovement ();
			else
				m_AI.CalculateMove ();
		} 
		else
		{
			m_Turn = ePlayerPriority.PlayerOne;
			m_PlayerTwo.m_State = WizardState.Casting;
			m_TurnTorchOne.SetActive (true);
			m_TurnTorchTwo.SetActive (false);
			m_TurnText.text = "Player 1's Turn";
			m_PlayerOne.DisplayMovement ();
		}


	}

	public void GameOver()
	{
		m_Player1Idle.SetActive (false);
		m_Player2Idle.SetActive (false);
		m_AudioSource.PlayOneShot (m_VictorySound);
		HideSpellTiles ();
		// turn over is at start of move, so previous player is the winner
		if (m_Turn == ePlayerPriority.PlayerTwo) 
		{
			// player 1 win
			m_TurnText.text = "Player 1 Wins!";
			m_Player1Victory.SetActive (true);
			m_Player2Death.SetActive (true);
		}
		else if (m_Turn == ePlayerPriority.PlayerOne) 
		{
			// player 2 win
			m_TurnText.text = "Player 2 Wins!";
			m_Player2Victory.SetActive (true);
			m_Player1Death.SetActive (true);
		}
	}

	public void GoToMainMenu()
	{
		SceneManager.LoadScene ("Menu");
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WizardState{Moving, Casting, Idle, Dead};


[System.Serializable]
public struct sTileRow
{
	public GameObject[] m_Tiles;
}

public class PlayerControl : MonoBehaviour {

	public GameManager m_Manager;
	public sTileRow[] m_TileMap;
	public GameObject[] m_MovementIndicators;
	public TileManager m_CurrentTile;
	public bool m_Dead;
	public WizardState m_State;
	public int m_CurrentX, m_CurrentY;

	public SpriteAnimator m_IdleAnimation;
	public SpriteAnimator m_CastingAnimation;
	public SpriteAnimator m_MovingAnimation;
	public SpriteAnimator m_DeathAnimation;

	public AudioClip m_DeathSound, m_MoveSound;
	private AudioSource m_AudioPlayer;

	public void Move(int x, int y)
	{
		TileManager nextTile = m_TileMap [y].m_Tiles[x].GetComponent<TileManager> ();

		m_CurrentTile.m_Player = false;

		this.transform.position = nextTile.transform.position;
		nextTile.m_Player = true;
		m_CurrentTile = nextTile;
		m_CurrentX = x;
		m_CurrentY = y;
		m_State = WizardState.Moving;

		m_AudioPlayer.PlayOneShot (m_MoveSound);
		DisplaySpellTiles ();
	}

	public void DisplayMovement()
	{
		HideSpellTiles ();
		int movementValue = m_CurrentTile.m_Value;
		if (m_CurrentX + movementValue < 5 && 
			!m_TileMap [m_CurrentY].m_Tiles [m_CurrentX + movementValue].GetComponent<TileManager>().m_Player &&
			m_TileMap [m_CurrentY].m_Tiles [m_CurrentX + movementValue].GetComponent<TileManager>().m_Value < 5 &&
			m_TileMap [m_CurrentY].m_Tiles [m_CurrentX + movementValue].GetComponent<TileManager>().m_Value > 0) 
		{
			m_MovementIndicators [0].transform.position = m_TileMap [m_CurrentY].m_Tiles [m_CurrentX + movementValue].transform.position;
			m_MovementIndicators [0].gameObject.SetActive (true);
		}
		if(m_CurrentX - movementValue >= 0 &&
			!m_TileMap [m_CurrentY].m_Tiles [m_CurrentX - movementValue].GetComponent<TileManager>().m_Player &&
			m_TileMap [m_CurrentY].m_Tiles [m_CurrentX - movementValue].GetComponent<TileManager>().m_Value < 5 &&
			m_TileMap [m_CurrentY].m_Tiles [m_CurrentX - movementValue].GetComponent<TileManager>().m_Value > 0) 
		{
			m_MovementIndicators [1].transform.position = m_TileMap [m_CurrentY].m_Tiles [m_CurrentX - movementValue].transform.position;
			m_MovementIndicators [1].gameObject.SetActive (true);
		}
		if (m_CurrentY + movementValue < 5 &&
			!m_TileMap [m_CurrentY + movementValue].m_Tiles [m_CurrentX].GetComponent<TileManager>().m_Player &&
			m_TileMap [m_CurrentY + movementValue].m_Tiles [m_CurrentX].GetComponent<TileManager>().m_Value < 5 &&
			m_TileMap [m_CurrentY + movementValue].m_Tiles [m_CurrentX].GetComponent<TileManager>().m_Value > 0) 
		{
			m_MovementIndicators [2].transform.position = m_TileMap [m_CurrentY + movementValue].m_Tiles [m_CurrentX].transform.position;
			m_MovementIndicators [2].gameObject.SetActive (true);
		}
		if(m_CurrentY - movementValue >= 0 &&
			!m_TileMap [m_CurrentY - movementValue].m_Tiles [m_CurrentX].GetComponent<TileManager>().m_Player &&
			m_TileMap [m_CurrentY - movementValue].m_Tiles [m_CurrentX].GetComponent<TileManager>().m_Value < 5 &&
			m_TileMap [m_CurrentY - movementValue].m_Tiles [m_CurrentX].GetComponent<TileManager>().m_Value > 0) 
		{
			m_MovementIndicators [3].transform.position = m_TileMap [m_CurrentY - movementValue].m_Tiles [m_CurrentX].transform.position;
			m_MovementIndicators [3].gameObject.SetActive (true);
		}

		foreach (GameObject movement in m_MovementIndicators) 
		{
			if (movement.activeInHierarchy)
				return;
		}

		m_AudioPlayer.PlayOneShot (m_DeathSound);
		m_State = WizardState.Dead;
		m_Manager.GameOver ();
	}

	public void HideSpellTiles()
	{
		for (int row = 0; row < m_TileMap.Length; ++row) 
		{
			for (int col = 0; col < m_TileMap[row].m_Tiles.Length; ++col) 
			{
				if (!m_TileMap[row].m_Tiles[col].GetComponent<TileManager>().m_Player)
				{
					m_TileMap[row].m_Tiles[col].GetComponent<TileManager> ().ActivateCollider (false);
				}
			}
		}
	}

	public void DisplaySpellTiles()
	{
		foreach (GameObject mov in m_MovementIndicators) {
			mov.gameObject.SetActive (false);
		}

		for (int row = 0; row < m_TileMap.Length; ++row) 
		{
			for (int col = 0; col < m_TileMap[row].m_Tiles.Length; ++col) 
			{
				if (!m_TileMap[row].m_Tiles[col].GetComponent<TileManager>().m_Player)
				{
					m_TileMap[row].m_Tiles[col].GetComponent<TileManager> ().ActivateCollider (true);
				}
			}
		}
	}

	// Use this for initialization
	public void Setup () {
		m_CurrentTile = m_TileMap [m_CurrentY].m_Tiles [m_CurrentX].GetComponent<TileManager>();
		m_CurrentTile.m_Player = true;
		m_AudioPlayer = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {

		// select animation set
		switch (m_State) 
		{
		case WizardState.Idle:
			m_IdleAnimation.enabled = true;
			m_DeathAnimation.enabled = false;
			m_MovingAnimation.enabled = false;
			m_CastingAnimation.enabled = false;
			break;

		case WizardState.Casting:
			m_CastingAnimation.enabled = true;
			m_DeathAnimation.enabled = false;
			m_MovingAnimation.enabled = false;
			m_IdleAnimation.enabled = false;

			if (m_CastingAnimation.m_AnimationFinished) 
			{
				m_State = WizardState.Idle;
			}

			break;

		case WizardState.Moving:
			m_MovingAnimation.enabled = true;
			m_DeathAnimation.enabled = false;
			m_IdleAnimation.enabled = false;
			m_CastingAnimation.enabled = false;

			if (m_MovingAnimation.m_AnimationFinished)
				m_State = WizardState.Idle;
			break;

		case WizardState.Dead:
			m_DeathAnimation.enabled = true;
			m_MovingAnimation.enabled = false;
			m_IdleAnimation.enabled = false;
			m_CastingAnimation.enabled = false;
			break;
		}
	}
}

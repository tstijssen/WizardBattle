using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WizardState{Moving, Casting, Idle, Dead};

// holds references to all tiles on the map in rows and columns
[System.Serializable]	
public struct sTileRow
{
	public GameObject[] m_Tiles;
}

public class PlayerControl : MonoBehaviour {

	public GameManager m_Manager;
	public sTileRow[] m_TileMap;	// game map 5x5 grid
	public GameObject[] m_MovementIndicators;	// sprites to indicate where player can move
	public TileManager m_CurrentTile;		// reference to current tile player is on
	public WizardState m_State;
	public int m_CurrentX, m_CurrentY;		// current grid position

	// animations for idling, casting, moving and dying states
	public SpriteAnimator m_IdleAnimation;		
	public SpriteAnimator m_CastingAnimation;
	public SpriteAnimator m_MovingAnimation;
	public SpriteAnimator m_DeathAnimation;

	public AudioClip m_DeathSound, m_MoveSound;
	private AudioSource m_AudioPlayer;

	public void Move(int x, int y)
	{
		// grab reference to next tile
		TileManager nextTile = m_TileMap [y].m_Tiles[x].GetComponent<TileManager> ();

		// previous tile no longer contains a player
		m_CurrentTile.m_Player = false;

		// move to new square
		this.transform.position = nextTile.transform.position;
		nextTile.m_Player = true;
		m_CurrentTile = nextTile;
		m_CurrentX = x;
		m_CurrentY = y;
		m_State = WizardState.Moving; // trigger animation

		m_AudioPlayer.PlayOneShot (m_MoveSound);
		DisplaySpellTiles (); // go to next stage
	}

	// display valid movements
	public void DisplayMovement()
	{
		HideSpellTiles ();
		int movementValue = m_CurrentTile.m_Value; // get movement value for turn
		
		// check wether movement is within boundaries, and whether next tile in all 4 directions is valid
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

		// if any movements are valid, return
		foreach (GameObject movement in m_MovementIndicators) 
		{
			if (movement.activeInHierarchy)
				return;
		}

		// if no movements are valid, game over for player
		m_AudioPlayer.PlayOneShot (m_DeathSound);
		m_State = WizardState.Dead;
		m_Manager.GameOver ();
	}

	// hide indicators for casting spells on tiles
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

	// show indicators for casting spells on tiles
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
				// idle animation loops
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

			// casting animation reverts to idle when done
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

			// moving animation reverts to idle when done
			if (m_MovingAnimation.m_AnimationFinished)
				m_State = WizardState.Idle;
			break;

		case WizardState.Dead:
				// death animation stops after one loop
			m_DeathAnimation.enabled = true;
			m_MovingAnimation.enabled = false;
			m_IdleAnimation.enabled = false;
			m_CastingAnimation.enabled = false;
			break;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct sMovePath
{
	public List<TileManager> m_AvailableMoves;
	public int x, y;
}

public class WizardAI : MonoBehaviour {

	public PlayerControl m_PlayerControl;
	public PlayerControl m_Opponent;
	public GameObject m_IceSpell, m_FireSpell;
	public float m_Delay;

	IEnumerator DelayAI()
	{
		yield return new WaitForSeconds (m_Delay);
		CalculateSpell ();
	}

	sMovePath GetPossibleMoves(int x, int y)
	{
		sMovePath newPath;
		newPath.x = x;
		newPath.y = y;
		newPath.m_AvailableMoves = new List<TileManager>();

		int movementValue = m_PlayerControl.m_TileMap [y].m_Tiles [x].GetComponent<TileManager>().m_Value;
		if (x + movementValue < 5 && 
			!m_PlayerControl.m_TileMap [y].m_Tiles [x + movementValue].GetComponent<TileManager>().m_Player &&
			m_PlayerControl.m_TileMap [y].m_Tiles [x + movementValue].GetComponent<TileManager>().m_Value < 5 &&
			m_PlayerControl.m_TileMap [y].m_Tiles [x + movementValue].GetComponent<TileManager>().m_Value > 0) 
		{
			newPath.m_AvailableMoves.Add (m_PlayerControl.m_TileMap [y].m_Tiles [x + movementValue].GetComponent<TileManager> ());
		}
		if(x - movementValue >= 0 &&
			!m_PlayerControl.m_TileMap [y].m_Tiles [x - movementValue].GetComponent<TileManager>().m_Player &&
			m_PlayerControl.m_TileMap [y].m_Tiles [x - movementValue].GetComponent<TileManager>().m_Value < 5 &&
			m_PlayerControl.m_TileMap [y].m_Tiles [x - movementValue].GetComponent<TileManager>().m_Value > 0) 
		{
			newPath.m_AvailableMoves.Add (m_PlayerControl.m_TileMap [y].m_Tiles [x - movementValue].GetComponent<TileManager> ());
		}
		if (y + movementValue < 5 &&
			!m_PlayerControl.m_TileMap [y + movementValue].m_Tiles [x].GetComponent<TileManager>().m_Player &&
			m_PlayerControl.m_TileMap [y + movementValue].m_Tiles [x].GetComponent<TileManager>().m_Value < 5 &&
			m_PlayerControl.m_TileMap [y + movementValue].m_Tiles [x].GetComponent<TileManager>().m_Value > 0) 
		{
			newPath.m_AvailableMoves.Add (m_PlayerControl.m_TileMap [y + movementValue].m_Tiles [x].GetComponent<TileManager> ());
		}
		if(y - movementValue >= 0 &&
			!m_PlayerControl.m_TileMap [y - movementValue].m_Tiles [x].GetComponent<TileManager>().m_Player &&
			m_PlayerControl.m_TileMap [y - movementValue].m_Tiles [x].GetComponent<TileManager>().m_Value < 5 &&
			m_PlayerControl.m_TileMap [y - movementValue].m_Tiles [x].GetComponent<TileManager>().m_Value > 0) 
		{
			newPath.m_AvailableMoves.Add (m_PlayerControl.m_TileMap [y - movementValue].m_Tiles [x].GetComponent<TileManager> ());
		}

		return newPath;
	}

	public void CalculateMove()
	{
		//calculate possible moves
		sMovePath m_PossibleMoves = GetPossibleMoves(m_PlayerControl.m_CurrentX, m_PlayerControl.m_CurrentY);

		// if only one move left, simply go there
		if (m_PossibleMoves.m_AvailableMoves.Count == 1)
			m_PlayerControl.Move ((int)m_PossibleMoves.m_AvailableMoves [0].transform.position.x, (int)m_PossibleMoves.m_AvailableMoves [0].transform.position.y);
		else if (m_PossibleMoves.m_AvailableMoves.Count == 0) // if no moves left, AI loses game
		{
			m_PlayerControl.m_State = WizardState.Dead;
			m_PlayerControl.m_Manager.GameOver ();
			return;
		}

		List<sMovePath> m_MoveList = new List<sMovePath>();

		// check moves after next
		foreach (TileManager tile in m_PossibleMoves.m_AvailableMoves) 
		{
			m_MoveList.Add(GetPossibleMoves((int)tile.transform.position.x, (int)tile.transform.position.y));
		}
		
		m_MoveList.Sort(delegate(sMovePath x, sMovePath y) {
			return x.m_AvailableMoves.Count - y.m_AvailableMoves.Count;
		}
		);

		// move to tile with most available future options
		m_PlayerControl.Move ((int)m_MoveList [0].x, (int)m_MoveList [0].y);

		StartCoroutine (DelayAI());
	}

	void CalculateSpell()
	{
		// calculate opponents possible moves
		sMovePath m_PossibleMoves = GetPossibleMoves(m_Opponent.m_CurrentX, m_Opponent.m_CurrentY);
		TileManager targetTile;
		// if only one move left and tile is 1 or 4, simply cast spell to kill
		if (m_PossibleMoves.m_AvailableMoves.Count == 1) 
		{
			targetTile = m_PlayerControl.m_TileMap [(int)m_PossibleMoves.m_AvailableMoves [0].transform.position.y].m_Tiles [(int)m_PossibleMoves.m_AvailableMoves [0].transform.position.x].GetComponent<TileManager> ();
		
			if (targetTile.m_Value == 1) 
			{
				targetTile.m_Manager.PlayIceSound ();
				m_IceSpell.transform.position = targetTile.transform.position;
				m_IceSpell.SetActive (true);
				targetTile.EnchantTile (-1);
			} 
			else if (targetTile.m_Value == 4) 
			{
				targetTile.m_Manager.PlayFireSound ();
				m_FireSpell.transform.position = targetTile.transform.position;
				m_FireSpell.SetActive (true);
				targetTile.EnchantTile (1);
			}
		}
		else if (m_PossibleMoves.m_AvailableMoves.Count == 0)
		{
			m_PlayerControl.m_Manager.TurnSwitch ();
			return;	// should technically cast a spell before passing turn but whatevs
		}
			
		// otherwise cast spell randomly on one of the opponents next tiles?
		int randomTile = Random.Range(0, m_PossibleMoves.m_AvailableMoves.Count);
		targetTile = m_PlayerControl.m_TileMap [(int)m_PossibleMoves.m_AvailableMoves [randomTile].transform.position.y].m_Tiles [(int)m_PossibleMoves.m_AvailableMoves [randomTile].transform.position.x].GetComponent<TileManager> ();

		if (targetTile.m_Value >= 3) 
		{
			targetTile.m_Manager.PlayFireSound ();
			m_FireSpell.transform.position = targetTile.transform.position;
			m_FireSpell.SetActive (true);
			targetTile.EnchantTile (1);
		} 
		else 
		{
			targetTile.m_Manager.PlayIceSound ();
			m_IceSpell.transform.position = targetTile.transform.position;
			m_IceSpell.SetActive (true);
			targetTile.EnchantTile (-1);
		}

	}
}

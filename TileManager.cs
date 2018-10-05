using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// behaviour for a single tile object
public class TileManager : MonoBehaviour {

	public GameManager m_Manager;
	public GameObject m_SpellSelector;	// menu for selecting spells to cast on this tile
	public SpriteRenderer m_Ice, m_Fire;	// sprites for displaying ice (for low value tiles) and fire (for high value)
	public int m_Value;	
	public bool m_Player;	// shows whether there is a player on this tile
	public SpriteRenderer m_DisplayNumber;	// sprite to display the current value of the tile
	public Sprite[] m_NumberSprites;	// list of values that can be displayed
	public GameObject m_ActivatedIndicator;	// sprite that indicates whether this tile can be selected for a spell
	public BoxCollider2D m_Collider;	

	// Use this for initialization
	void Start () {
		// randomly generate tile value (1,2,3)
		m_Value = Random.Range(1, 4);

		ChangeTileAppearance ();
	}

	// activate collider for casting spells, allows players to click on this tile
	public void ActivateCollider(bool value)
	{
		m_ActivatedIndicator.SetActive (value);
		m_Collider.enabled = value;
	}
		
	// cast spell on tile, changing its value
	public bool EnchantTile(int value)
	{
		// if tile is already and min or max, cancel and go back to casting state
		if (m_Value + value < 0 || m_Value + value > 5) 
		{
			m_Manager.DisplaySpellTiles ();
			return false;
		}

		m_Value += value;

		// change tile appearance
		ChangeTileAppearance ();

		// after spell cast, turn switches to other player
		m_Manager.TurnSwitch ();

		return true;
	}

	// display value number and ice and fire sprites on top of tile
	// ice and fire sprites toggled using transparency
	public void ChangeTileAppearance()
	{
		m_DisplayNumber.sprite = m_NumberSprites [m_Value];

		switch (m_Value) 
		{
		case 0:
			m_Ice.color = new Color (1, 1, 1, 1);
			m_Fire.color = new Color (1, 1, 1, 0);
			break;
		case 1:
			m_Ice.color = new Color (1, 1, 1, 0.5f);
			m_Fire.color = new Color (1, 1, 1, 0);
			break;
		case 2:
			m_Ice.color = new Color (1, 1, 1, 0);
			m_Fire.color = new Color (1, 1, 1, 0);
			break;
		case 3:
			m_Ice.color = new Color (1, 1, 1, 0);
			m_Fire.color = new Color (1, 1, 1, 0);
			break;
		case 4:
			m_Ice.color = new Color (1, 1, 1, 0);
			m_Fire.color = new Color (1, 1, 1, 0.5f);
			break;
		case 5:
			m_Ice.color = new Color (1, 1, 1, 0);
			m_Fire.color = new Color (1, 1, 1, 1);
			break;
		}
	}

	void OnMouseOver()
	{
		// activate spell menu and move it over this tile
		if (Input.GetMouseButtonDown (0)) 
		{
			m_SpellSelector.SetActive (true);
			m_SpellSelector.transform.position = this.transform.position;
			m_SpellSelector.GetComponentsInChildren<SpellSelector> ()[0].m_Tile = this;
			m_SpellSelector.GetComponentsInChildren<SpellSelector> ()[1].m_Tile = this;
			m_Manager.HideSpellTiles ();
		}
	}
}

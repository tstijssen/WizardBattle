using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

	public GameManager m_Manager;
	public GameObject m_SpellSelector;
	public SpriteRenderer m_Ice, m_Fire;
	public int m_Value;
	public bool m_Player;
	public SpriteRenderer m_DisplayNumber;
	public Sprite[] m_NumberSprites;
	public GameObject m_ActivatedIndicator;
	public BoxCollider2D m_Collider;

	// Use this for initialization
	void Start () {
		// random generator
		m_Value = Random.Range(1, 4);

		ChangeTileAppearance ();
	}

	public void ActivateCollider(bool value)
	{
		m_ActivatedIndicator.SetActive (value);
		m_Collider.enabled = value;
	}
		

	public bool EnchantTile(int value)
	{
		if (m_Value + value < 0 || m_Value + value > 5) 
		{
			m_Manager.DisplaySpellTiles ();
			return false;
		}

		m_Value += value;

		// change tile appearance
		ChangeTileAppearance ();

		m_Manager.TurnSwitch ();

		return true;
	}

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

		if (Input.GetMouseButtonDown (0)) 
		{
			m_SpellSelector.SetActive (true);
			m_SpellSelector.transform.position = this.transform.position;
			m_SpellSelector.GetComponentsInChildren<SpellSelector> ()[0].m_Tile = this;
			m_SpellSelector.GetComponentsInChildren<SpellSelector> ()[1].m_Tile = this;
			m_Manager.HideSpellTiles ();
		}
//		if (Input.GetMouseButtonDown (0)) 
//		{
//			m_FireSpell.SetActive (true);
//			m_FireSpell.transform.position = this.transform.position;
//			EnchantTile (1);
//		}
//		else if (Input.GetMouseButtonDown (1)) 
//		{
//			m_IceSpell.SetActive (true);
//			m_IceSpell.transform.position = this.transform.position;
//			EnchantTile (-1);
//		}
	}
}

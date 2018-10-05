using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSpellType {Ice, Fire};

public class SpellSelector : MonoBehaviour {

	public TileManager m_Tile;
	public eSpellType m_Type;
	public GameObject m_IceSpell, m_FireSpell;

	public void SelectSpell()
	{
		if (m_Type == eSpellType.Ice)
		{
			if (m_Tile.EnchantTile (-1)) 
			{
				m_Tile.m_Manager.PlayIceSound ();
				m_IceSpell.transform.position = this.transform.parent.position;
				m_IceSpell.SetActive (true);
			}
		}
		else if (m_Type == eSpellType.Fire)
		{
			if (m_Tile.EnchantTile (1)) 
			{
				m_Tile.m_Manager.PlayFireSound ();
				m_FireSpell.transform.position = this.transform.parent.position;
				m_FireSpell.SetActive (true);
			}
		}
		this.transform.parent.gameObject.SetActive (false);
	}
}

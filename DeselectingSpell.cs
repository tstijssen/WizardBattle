using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeselectingSpell : MonoBehaviour {

	public GameManager m_Manager;
	public GameObject m_SpellSelector;

	// used to click off of the spell selection menu
	void OnMouseOver()
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			m_SpellSelector.gameObject.SetActive (false);
			m_Manager.DisplaySpellTiles ();
		}
	}
}

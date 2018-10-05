using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDetector : MonoBehaviour {

	PlayerControl m_Player;

	// Use this for initialization
	void Start () {
		m_Player = this.GetComponentInParent<PlayerControl> ();
	}

	void OnMouseOver()
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			m_Player.Move ((int)this.transform.position.x, (int)this.transform.position.y);
		}
	}
}

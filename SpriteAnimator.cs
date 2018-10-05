using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour {

	public float m_AnimationDelay;
	public bool m_Loop;
	public bool m_DeactivateOnFinish;
	public bool m_AnimationFinished;
	public Sprite[] m_AnimationSprites;
	public int m_Frame = 0;
	private SpriteRenderer m_Sprite;
	private float m_Timer;

	// Use this for initialization
	void Start () {
		m_Sprite = GetComponent<SpriteRenderer> ();
		m_Timer = m_AnimationDelay;
	}
	
	// Update is called once per frame
	void Update () {
		m_Timer -= Time.deltaTime;

		if(!m_AnimationFinished)
			m_Sprite.sprite = m_AnimationSprites [m_Frame];

		if (m_Timer < 0.0f && !m_AnimationFinished) 
		{
			m_Frame++;
			m_Timer = m_AnimationDelay;

			if (m_Loop)
				m_Frame %= m_AnimationSprites.Length;
			else if (m_Frame >= m_AnimationSprites.Length) 
			{


				if (m_DeactivateOnFinish) 
				{
					m_Frame = 0;
					this.gameObject.SetActive (false);
					m_AnimationFinished = false;
				}
				else
					m_AnimationFinished = true;
			}
		}
	}



	void OnDisable()
	{
		m_AnimationFinished = false;
		m_Frame = 0;
	}
}

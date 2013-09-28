//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Plays the specified sound.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Sound")]
public class UIButtonSound : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
	}

	public AudioClip audioClip;
	public Trigger trigger = Trigger.OnClick;
	public float volume = 1f;
	public float pitch = 1f;

	void OnHover (bool isOver)
	{
		if ( audioClip && enabled && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
		{
			string strName = audioClip.name;
			AudioManager.Instance.PlayClip( strName );			
			//NGUITools.PlaySound(audioClip, volume, pitch);
		}
	}

	void OnPress (bool isPressed)
	{
		if (audioClip && enabled && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
		{
			string strName = audioClip.name;
			AudioManager.Instance.PlayClip( strName );			
			//NGUITools.PlaySound(audioClip, volume, pitch);
		}
	}

	void OnClick ()
	{
		if ( audioClip && enabled && trigger == Trigger.OnClick)
		{
			string strName = audioClip.name;
			AudioManager.Instance.PlayClip( strName );
			//NGUITools.PlaySound(audioClip, volume, pitch);
		}
	}
}
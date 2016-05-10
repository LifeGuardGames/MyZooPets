//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright Â© 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Sample script showing how easy it is to implement a standard button that swaps sprites.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Image Button")]
public class UIImageButton : MonoBehaviour
{
	public UISprite target;
	public string normalSprite;
	public string hoverSprite;
	public string pressedSprite;
	public string disabledSprite;
	
	public bool isEnabled
	{
		get
		{
			Collider col = GetComponent<Collider>();
			return col && col.enabled;
		}
		set
		{
			Collider col = GetComponent<Collider>();
			if (!col) return;

			if (col.enabled != value)
			{
				col.enabled = value;
				UpdateImage();
			}
		}
	}

	void Awake () { if (target == null) target = GetComponentInChildren<UISprite>(); }
	void OnEnable () { UpdateImage(); }
	
	void UpdateImage()
	{
		if (target != null)
		{
			if (isEnabled)
			{
				string strSpriteName = UICamera.IsHighlighted(gameObject) ? hoverSprite : normalSprite;
				
				if ( strSpriteName != null )
					target.spriteName = strSpriteName;
			}
			else
			{
				target.spriteName = disabledSprite;
			}	
			target.MakePixelPerfect();
		}
	}

	void OnHover (bool isOver)
	{
		if (isEnabled && target != null && hoverSprite != null)
		{
			target.spriteName = isOver ? hoverSprite : normalSprite;
			target.MakePixelPerfect();
		}
	}

	void OnPress (bool pressed)
	{
		if (pressed && pressedSprite != null)
		{
			target.spriteName = pressedSprite;
			target.MakePixelPerfect();
		}
		else UpdateImage();
	}
}

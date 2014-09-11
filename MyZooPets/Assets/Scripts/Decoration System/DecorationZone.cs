using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Decoration zone.
/// This is the replacement for decoration nodes in V1.3.4 update
/// This is the zone that the inventory will be dragged and dropped on.
/// </summary>

public enum DecorationZoneState{
	Neutral, Inactive, Active, Hover
}

public class DecorationZone : MonoBehaviour {

	// what type of decorations can go on this node?
	public DecorationTypes nodeType;
	public DecorationTypes GetDecoType(){
		return nodeType;
	}

	public UISprite spriteOutline;
	public UISprite spriteFill;
	public Color neutralColorOutline;
	public Color neutralColorFill;
	public Color activeColorOutline;
	public Color activeColorFill;
	public Color inactiveColorOutline;
	public Color inactiveColorFill;

	public Animation activeHoverAnimation;

	private bool isHovered = false;
	private bool isHoverPlaying = false;

	private DecorationZoneState state;

	void Start(){
		DecorationUIManager.Instance.OnDecoPickedUp += OnDecorationPickedUp;
		DecorationUIManager.Instance.OnDecoDropped += OnDecorationDropped;
	}

	void OnDestroy(){
		DecorationUIManager.Instance.OnDecoPickedUp -= OnDecorationPickedUp;
		DecorationUIManager.Instance.OnDecoDropped -= OnDecorationDropped;
	}

	/// <summary>
	/// Raises the decoration picked up event.
	/// </summary>
	private void OnDecorationPickedUp(object sender, EventArgs args){
		SetState(DecorationUIManager.Instance.currentDeco.DecorationType.ToString());
	}

	/// <summary>
	/// Raises the decoration dropped event.
	/// </summary>
	private void OnDecorationDropped(object sender, EventArgs args){
		SetState(null);
	}

	/// <summary>
	/// Raycasting call from drag object
	/// </summary>
	public void CheckHover(GameObject sender){
		SetState(DecorationUIManager.Instance.currentDeco.DecorationType.ToString(), isHovered:true);
	}

	/// <summary>
	/// Sets the state of the node.
	/// NOTE: Only for object picked up! hovering will be done with raycasting
	/// </summary>
	/// <param name="typeName">Type name.</param>
	private void SetState(string nodeTypeName, bool isHovered = false){
		// Neutral state, play idle state
		if(nodeTypeName == null){
			state = DecorationZoneState.Neutral;

			isHoverPlaying = false;
			if(!isHoverPlaying){
				animation.Stop();
			}

			spriteOutline.color = neutralColorOutline;
			spriteFill.color = neutralColorFill;
		}
		// Wrong state, play the inactive state
		else if(nodeTypeName != nodeType.ToString()){
			state = DecorationZoneState.Inactive;

			isHoverPlaying = false;
			if(!isHoverPlaying){
				animation.Stop();
			}

			spriteOutline.color = inactiveColorOutline;
			spriteFill.color = inactiveColorFill;
		}
		// Correct state, is hovered with right object
		else if(state == DecorationZoneState.Active && isHovered){
			state = DecorationZoneState.Hover;

			if(!isHoverPlaying){
				isHoverPlaying = true;
				activeHoverAnimation.Play();
			}
		}
		// Correct state, play the active state
		else{
			state = DecorationZoneState.Active;

			isHoverPlaying = false;
			if(!isHoverPlaying){
				animation.Stop();
			}

			spriteOutline.color = activeColorOutline;
			spriteFill.color = activeColorFill;
		}
	}
}

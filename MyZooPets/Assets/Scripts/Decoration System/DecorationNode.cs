using UnityEngine;
using System.Collections;

public class DecorationNode : MonoBehaviour {
	
	// what type of decorations can go on this node?
	public DecorationTypes eType;
	public DecorationTypes GetDecoType() {
		return eType;
	}

	void Start () {
		
		// use event handler to listen for when the player goes into edit deco mode
		EditDecosUIManager.OnManagerOpen += OnDecoMode;
		
		// listen for when this node is tapped/clicked -- if we change nodes to 2D, use OnPress instead
        TapItem tapItem = GetComponent<TapItem>();
        if (tapItem != null){
            tapItem.OnTap += NodeClicked;
        }		
		
		// by default, decoration nodes are not visible/interactable
		ToggleNode( false );
	}
	
	void OnDestroy() {
		// remove event handler when this node is destroyed
		EditDecosUIManager.OnManagerOpen -= OnDecoMode;	
	}
	
    //Event listener. listening to when decoration mode is enabled/disabled
    private void OnDecoMode(object sender, SingletonUI<EditDecosUIManager>.UIManagerEventArgs e){
       if(e.Opening){
            ToggleNode( true );		// edit mode is opening, so turn this node on
        }else{
            ToggleNode( false );	// edit mode is closing so turn this node off
        }
    }
	
	private void ToggleNode( bool bOn ) {
		GetComponent<MeshRenderer>().enabled = bOn;
		GetComponent<BoxCollider>().enabled = bOn;
	}
	
	private void NodeClicked() {
		// inform the ui manager
		DecorationTypes eType = GetDecoType();
		
		Debug.Log("deco node of type " + eType + " clicked");
		
		EditDecosUIManager.Instance.UpdateChooseMenu( eType );
	}
}

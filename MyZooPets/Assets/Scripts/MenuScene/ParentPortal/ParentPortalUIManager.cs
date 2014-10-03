using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Parse;

public class ParentPortalUIManager : SingletonUI<ParentPortalUIManager> {

	public UIGrid grid;
	public GameObject parentPortalEntryPetPrefab;
	public GameObject parentPortalEntryCreatePrefab;
	

	void Awake(){
		eModeType = UIModeTypes.ParentPortal;
	}

	void Start(){
		ParentPortalManager.OnDataRefreshed += UpdateGrid;
	}

	void Destroy(){
		ParentPortalManager.OnDataRefreshed -= UpdateGrid;
	}
	
	private void UpdateGrid(object sender, ServerEventArgs args){
		Debug.Log("updategrid");
		foreach(Transform child in grid.transform){
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}

		//data refreshed successfully
		if(args.IsSuccessful){
			ParseObjectKidAccount account = ParentPortalManager.Instance.KidAccount;

			GameObject portalEntryPet = NGUITools.AddChild(grid.gameObject, parentPortalEntryPetPrefab);

			//set the code
			UILabel codeLabel = portalEntryPet.FindInChildren("LabelCode").GetComponent<UILabel>();
			codeLabel.text = account.AccountCode;

			MutableDataPetMenuInfo menuInfo = DataManager.Instance.MenuSceneData;
			UILabel nameLabel = portalEntryPet.FindInChildren("LabelName").GetComponent<UILabel>();
			if(menuInfo != null)
				nameLabel.text = menuInfo.PetName;

			grid.Reposition();
		}
		else{
			//display error message to user
		}
	}

	protected override void _OpenUI(){
		GetComponent<TweenToggleDemux>().Show();

		//Note: maybe do some cache check. don't need to do a server everytime
		//the list opens
		ParentPortalManager.Instance.RefreshData();

	}

	protected override void _CloseUI(){
		GetComponent<TweenToggleDemux>().Hide();
	}

		void OnGUI(){
			if(GUI.Button(new Rect(100, 100, 100, 100), "Open")){
				OpenUI();
			}
			if(GUI.Button(new Rect(200, 100, 100, 100), "Close")){
				CloseUI();
			}
		}
}

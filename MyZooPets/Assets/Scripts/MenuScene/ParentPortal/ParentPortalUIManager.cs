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
		ParentPortalManager.OnAccountCreated += NewAccountCreated;
	}

	void Destroy(){
		ParentPortalManager.OnDataRefreshed -= UpdateGrid;
		ParentPortalManager.OnAccountCreated -= NewAccountCreated;
	}

	public void AddNewAccount(){
		ParentPortalManager.Instance.AddNewAccount();
	}

	private void NewAccountCreated(object sender, ServerEventArgs args){
		if(args.IsSuccessful){
			CloseUI();
		}
		else{
			//display error message to user
		}
	}

	private void UpdateGrid(object sender, ServerEventArgs args){
		// TODO Jason populate grid here based on data
		foreach(Transform child in grid.transform){
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}

		//data refreshed successfully
		if(args.IsSuccessful){
			List<ParseObjectKidAccount> kidAccountList = ParentPortalManager.Instance.KidAccountList;
			if(kidAccountList.Count > 0){
				//go through the account list and create UI for every account
				foreach(ParseObjectKidAccount account in kidAccountList){
					GameObject portalEntryPet = NGUITools.AddChild(grid.gameObject, parentPortalEntryPetPrefab);

					//set the code
					UILabel codeLabel = portalEntryPet.FindInChildren("LabelCode").GetComponent<UILabel>();
					codeLabel.text = account.AccountCode;

					//set the name for the account
					ParseObjectPetInfo petInfo = account.PetInfo;
					if(petInfo != null && petInfo.IsDataAvailable){
						UILabel nameLabel = portalEntryPet.FindInChildren("LabelName").GetComponent<UILabel>();
						nameLabel.text = petInfo.Name;
					}
				}

				grid.Reposition();
			}
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
		ParentPortalManager.OnDataRefreshed -= UpdateGrid;
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

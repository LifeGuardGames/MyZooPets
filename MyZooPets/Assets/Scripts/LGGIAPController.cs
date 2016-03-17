using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LGGIAPController : MonoBehaviour {
	public enum IAPUIState{
		Confirm,
		WaitingForTransaction,
		Completed
	}

	public TweenToggle purchaseTween;	// Used for confirm panel
	public List<GameObject> activeObjects;	// Objects to show when UI is active
	public List<GameObject> loadingObjects;	// Objects to show when UI is waiting for transaction
	public GameObject doneButton;	// For use when the confirmation panel is showing
	public UILabel statusLabel;

	public delegate void OnSuccessDelegate();	// Fires when remove ad is successful, for doing handling outside changes (hide ads UI from caller)
	public OnSuccessDelegate successCallback;

	private IAPUIState UIState;

	void Start(){
		LGGIAPManager.OnTransactionFinishedUI += OnTransactionComplete;

		foreach(GameObject go in loadingObjects){
			go.SetActive(false);
		}
	}

	void OnDestroy(){
		LGGIAPManager.OnTransactionFinishedUI -= OnTransactionComplete;
	}

	// Main driver for the UI to see which UI needs to be shown
	public void SwitchState(IAPUIState state){
		UIState = state;
		switch(state){
		case IAPUIState.Confirm:
			// Show all active objects
			foreach(GameObject go in activeObjects){
				go.SetActive(true);
			}
			foreach(GameObject go in loadingObjects){
				go.SetActive(false);
			}
			doneButton.SetActive(false);
			statusLabel.text = String.Format(Localization.Localize("IAP_CONFIRM"), LGGIAPManager.Instance.GetLocalizedPrice(LGGIAPManager.IAP_REMOVE_ADS));
			break;
		case IAPUIState.WaitingForTransaction:
			// Show all waiting objects
			foreach(GameObject go in activeObjects){
				go.SetActive(false);
			}
			foreach(GameObject go in loadingObjects){
				go.SetActive(true);
			}
			doneButton.SetActive(false);
			break;
		case IAPUIState.Completed:
			// Show all active objects
			foreach(GameObject go in activeObjects){
				go.SetActive(false);
			}
			foreach(GameObject go in loadingObjects){
				go.SetActive(false);
			}
			doneButton.SetActive(true);
			break;
		}
	}

	public void OnTransactionComplete(object sender, IAPEventArgs args){
		switch(args.purchaseState){
		case InAppPurchaseState.Purchased:
		case InAppPurchaseState.Restored:
			statusLabel.text = Localization.Localize("IAP_SUCCESS");
			successCallback();
			SwitchState(IAPUIState.Completed);
			break;
		case InAppPurchaseState.Deferred:
			statusLabel.text = Localization.Localize("IAP_DEFERRED");
			SwitchState(IAPUIState.Completed);	// TODO test this case
			break;
		case InAppPurchaseState.Failed:
			statusLabel.text = Localization.Localize("IAP_FAIL");
			SwitchState(IAPUIState.Completed);
			break;
		}
	}

	public void OnYesButtonClicked(){
		switch(UIState){
		case IAPUIState.Confirm:
			LGGIAPManager.Instance.PurchaseRemoveAds();
			SwitchState(IAPUIState.WaitingForTransaction);
			break;
		case IAPUIState.Completed:
			HideConfirmPanel();
			break;
		default:
			Debug.LogError("Bad UI state yes button clicked: " + UIState.ToString());
			break;
		}
	}

	public void OnNoButtonClicked(){
		switch(UIState){
		case IAPUIState.Confirm:
			HideConfirmPanel();
			break;
		case IAPUIState.Completed:
			HideConfirmPanel();
			break;
		default:
			Debug.LogError("Bad UI state no button clicked: " + UIState.ToString());
			break;
		}
	}

	public void OnDoneButtonClicked(){
		switch(UIState){
		case IAPUIState.Confirm:
			HideConfirmPanel();
			break;
		case IAPUIState.Completed:
			HideConfirmPanel();
			break;
		default:
			Debug.LogError("Bad UI state done button clicked: " + UIState.ToString());
			break;
		}
	}

	public void OnRestorePurchaseClicked(){
		switch(UIState){
		case IAPUIState.Confirm:
			LGGIAPManager.Instance.RestorePurchases();
			SwitchState(IAPUIState.WaitingForTransaction);
			break;
		case IAPUIState.Completed:
			HideConfirmPanel();
			break;
		default:
			Debug.LogError("Bad UI state restore button clicked: " + UIState.ToString());
			break;
		}
	}

	public void ShowConfirmPanel(OnSuccessDelegate successDelegate){
		successCallback = successDelegate;
		SwitchState(IAPUIState.Confirm);
		purchaseTween.Show();
	}

	public void HideConfirmPanel(){
		purchaseTween.Hide();
	}
}

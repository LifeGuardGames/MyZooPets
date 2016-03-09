using UnityEngine;
using System.Collections;
using System;

public class LGGIAPManager : Singleton<LGGIAPManager>{

	public const string IAP_REMOVE_ADS = "com.LifeGuardGames.WellapetsAsthma.IAP.RemoveAds";

	void Awake(){
		IOSInAppPurchaseManager.OnStoreKitInitComplete += OnStoreKitInitComplete;
		IOSInAppPurchaseManager.OnTransactionComplete += OnTransactionComplete;
		IOSInAppPurchaseManager.OnRestoreComplete += OnRestoreComplete;
		IOSInAppPurchaseManager.instance.LoadStore();
	}

	// Callback for intialize complete
	private void OnStoreKitInitComplete(ISN_Result result){
		IOSInAppPurchaseManager.OnStoreKitInitComplete -= OnStoreKitInitComplete;
		if(result.IsSucceeded){
			Debug.Log("Inited successfully, Available products count: " + IOSInAppPurchaseManager.Instance.Products.Count.ToString());
		}
		else{
			Debug.Log("StoreKit Init Failed.  Error code: " + result.Error.Code + "\n" + "Error description:" + result.Error.Description);
		}
	}

	// Callback for transaction complete, restoring purchases will call these in succession
	private void OnTransactionComplete(IOSStoreKitResult response){
		Debug.Log("OnTransactionComplete: " + response.ProductIdentifier);
		Debug.Log("OnTransactionComplete: state: " + response.State);

		switch(response.State){
		case InAppPurchaseState.Purchased:
		case InAppPurchaseState.Restored:
			//Our product been successfully purchased or restored
			UnlockProduct(response.ProductIdentifier);
			break;
		case InAppPurchaseState.Deferred:
			//iOS 8 introduces Ask to Buy, which lets parents approve any purchases initiated by children
			//You should update your UI to reflect this deferred state, and expect another Transaction 
			//Complete to be called again with a new transaction state reflecting the parent's decision or after the 
			//transaction times out. Avoid blocking your UI or gameplay while waiting for the transaction to be updated.
			break;
		case InAppPurchaseState.Failed:
			//We can unlock interface and report user that the purchase is failed. 
			Debug.Log("Transaction failed with error, code: " + response.Error.Code);
			Debug.Log("Transaction failed with error, description: " + response.Error.Description);
			break;
		}
		// TODO need this?
		IOSNativePopUpManager.showMessage("Store Kit Response", "product " + response.ProductIdentifier + " state: " + response.State.ToString());
	}

	private static void OnRestoreComplete(IOSStoreKitRestoreResult res){
		if(res.IsSucceeded){
			IOSNativePopUpManager.showMessage("Success", "Restore Completed");
		}
		else{
			IOSNativePopUpManager.showMessage("Error: " + res.Error.Code, res.Error.Description);
		}
	}

	private void UnlockProduct(string productID){
		switch(productID){
		// Add all product actions here
		case IAP_REMOVE_ADS:
			Debug.Log("Remove ads unlocked");
			DataManager.Instance.IsAdsEnabled = false;
			break;
		default:
			Debug.LogError("Invalid unlock product: " + productID);
			break;
		}
	}

	public void PurchaseRemoveAds(){
		PurchaseProduct(IAP_REMOVE_ADS);
	}

	private void PurchaseProduct(string productID){
		if(IOSInAppPurchaseManager.Instance.IsStoreLoaded){
			Debug.Log("BUYING PRODUCT : " + productID);
			IOSInAppPurchaseManager.Instance.BuyProduct(productID);
		}
		else{
			Debug.LogError("Store not loaded yet for purchase.");
		}
	}

	private void RestorePurchases(){
		if(IOSInAppPurchaseManager.Instance.IsStoreLoaded){
			Debug.Log("RESTORING PURCHASES");
			IOSInAppPurchaseManager.Instance.RestorePurchases();
		}
		else{
			Debug.LogError("Store not loaded yet for purchase.");
		}
	}
}

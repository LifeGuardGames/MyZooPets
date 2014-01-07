// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class IAPLogic : MonoBehaviour {
//     public string[] productIdentifiers = new string[]{};

// 	// Use this for initialization
// 	void Start () {
//         // Listens to all the StoreKit events.  All event listeners MUST be removed before this object is disposed!
//         StoreKitManager.productPurchaseAwaitingConfirmationEvent += productPurchaseAwaitingConfirmationEvent;
//         StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessful;
//         StoreKitManager.purchaseCancelledEvent += purchaseCancelled;
//         StoreKitManager.purchaseFailedEvent += purchaseFailed;
//         StoreKitManager.productListReceivedEvent += productListReceivedEvent;
//         StoreKitManager.productListRequestFailedEvent += productListRequestFailed;
//         StoreKitManager.restoreTransactionsFailedEvent += restoreTransactionsFailed;
//         StoreKitManager.restoreTransactionsFinishedEvent += restoreTransactionsFinished;
//         StoreKitManager.paymentQueueUpdatedDownloadsEvent += paymentQueueUpdatedDownloadsEvent;	
// 	}
    
//     void OnDestroy(){
//        // Listens to all the StoreKit events.  All event listeners MUST be removed before this object is disposed!
//         StoreKitManager.productPurchaseAwaitingConfirmationEvent += productPurchaseAwaitingConfirmationEvent;
//         StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessful;
//         StoreKitManager.purchaseCancelledEvent += purchaseCancelled;
//         StoreKitManager.purchaseFailedEvent += purchaseFailed;
//         StoreKitManager.productListReceivedEvent += productListReceivedEvent;
//         StoreKitManager.productListRequestFailedEvent += productListRequestFailed;
//         StoreKitManager.restoreTransactionsFailedEvent += restoreTransactionsFailed;
//         StoreKitManager.restoreTransactionsFinishedEvent += restoreTransactionsFinished;
//         StoreKitManager.paymentQueueUpdatedDownloadsEvent += paymentQueueUpdatedDownloadsEvent; 
//     }	

//     private void productListReceivedEvent( List<StoreKitProduct> productList ){
//         Debug.Log( "productListReceivedEvent. total products received: " + productList.Count );
        
//         // print the products to the console
//         foreach( StoreKitProduct product in productList )
//             Debug.Log( product.ToString() + "\n" );
//     }
    
//     private void productListRequestFailed( string error ){
//         Debug.Log( "productListRequestFailed: " + error );
//     }
    
//     private void purchaseFailed( string error ){
//         Debug.Log( "purchase failed with error: " + error );
//     }

//     private void purchaseCancelled( string error ){
//         Debug.Log( "purchase cancelled with error: " + error );
//     }
    
//     private void productPurchaseAwaitingConfirmationEvent( StoreKitTransaction transaction ){
//         Debug.Log( "productPurchaseAwaitingConfirmationEvent: " + transaction );
//     }
    
//     private void purchaseSuccessful( StoreKitTransaction transaction ){
//         Debug.Log( "purchased product: " + transaction );
//     }
    
//     private void restoreTransactionsFailed( string error ){
//         Debug.Log( "restoreTransactionsFailed: " + error );
//     }
    
//     private void restoreTransactionsFinished(){
//         Debug.Log( "restoreTransactionsFinished" );
//     }
    
//     private void paymentQueueUpdatedDownloadsEvent( List<StoreKitDownload> downloads ){
//         Debug.Log( "paymentQueueUpdatedDownloadsEvent: " );
//         foreach( var dl in downloads )
//             Debug.Log( dl );
//     }
// }

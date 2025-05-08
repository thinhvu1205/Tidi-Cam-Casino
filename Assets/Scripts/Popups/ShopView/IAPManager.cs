using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : IStoreListener
{
    private IStoreController controller;
    private IExtensionProvider extensions;

    
    public IAPManager(JObject jData)
    {
        var lsItem = (JArray)jData["items"];
        //var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        for (var i =0; i < lsItem.Count; i++)
        {
            var itData = (JObject)lsItem[i];
            builder.AddProduct((string)itData["url"], ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("-=-=-=-=-=-= OnInitialized ");
        this.controller = controller;
        this.extensions = extensions;
    }

    void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
    {
        //throw new System.NotImplementedException();
        Debug.Log("-=-=-=-=-=-= OnInitializeFailed");
    }

    void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        //throw new System.NotImplementedException();

        Debug.Log("-=-=-=-=-=-=  OnPurchaseFailed");
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log("receipt:  " + args.purchasedProduct.receipt);
        Debug.Log("transactionID:  " + args.purchasedProduct.transactionID);
        JObject receiptObj = JObject.Parse(args.purchasedProduct.receipt);
        if (((string)receiptObj["Store"]).Equals("fake")) return PurchaseProcessingResult.Complete;

#if UNITY_ANDROID
        SocketSend.sendIAPResult(args.purchasedProduct.receipt);
#else
        SocketSend.validateIAPReceipt(args.purchasedProduct.receipt);
#endif

        return PurchaseProcessingResult.Complete;
    }

    private bool IsInitialized()
    {
        return controller != null && extensions != null;
    }

    public void buyIAP(string productId)
    {
        Debug.Log("buyIAP  " + productId);
        if (IsInitialized())
        {
            Product product = controller.products.WithID(productId);
            controller.InitiatePurchase(product);
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }
}

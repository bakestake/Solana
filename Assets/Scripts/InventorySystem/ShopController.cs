using System;
using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Engines;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public Shop thisShop;

    public void SetShopToGlobal()
    {
        GlobalShopController.Instance.SetShop(thisShop);
    }

    public void CloseShopToGlobal()
    {
        GlobalShopController.Instance.CloseShop(thisShop);
    }
}

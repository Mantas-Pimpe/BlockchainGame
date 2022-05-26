using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;

public class BuyScript : MonoBehaviour
{
    public int listingId;
    public GameObject button;
    
    public void BuyToken(){
        //Debug.Log(listingId);
        BuyEntity buyEntity = RestApi.BuyToken(listingId);
        ListingEntity listingEntity = RestApi.GetListing(buyEntity.listingId);
        TokenEntity token = new TokenEntity();
        token.tokenId = buyEntity.tokenId;
        token.owner = buyEntity.buyer;
        token.name = listingEntity.name;
        token.uri = listingEntity.uri;

        PlayerController playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerController.items.Add(token);

        button.transform.parent.transform.parent.GetComponent<MarketScript>().ReloadTableData();
    }

    public void CancelListing(){
        //Debug.Log(listingId);
        ListingEntity tmp = RestApi.CancelListing(listingId);
        ListingEntity listingEntity = RestApi.GetListing(tmp.listingId);
        TokenEntity token = new TokenEntity();
        token.tokenId = listingEntity.tokenId;
        token.owner = listingEntity.seller;
        token.name = listingEntity.name;
        token.uri = listingEntity.uri;

        PlayerController playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerController.items.Add(token);

        button.transform.parent.transform.parent.GetComponent<MarketScript>().ReloadTableData();
    }
}

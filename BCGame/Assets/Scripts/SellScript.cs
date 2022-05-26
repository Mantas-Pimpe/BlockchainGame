using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using TMPro;
using System.Linq;

public class SellScript : MonoBehaviour
{
    public int tokenId;
    public GameObject button;
    
    public void CreateListing(){
        InventoryScript inventoryScript = button.transform.parent.transform.parent.GetComponent<InventoryScript>();
        string priceText = button.transform.parent.transform.GetChild(3).GetComponent<TMP_InputField>().text;
        GameObject inventoryItem = GameObject.Find("InventoryItems_" + tokenId);
        if(inventoryItem != null && !String.IsNullOrEmpty(priceText)){
            int price = Int32.Parse(priceText);
            RestApi.CreateListing(tokenId, price);
            
            DestroyImmediate(inventoryItem);
            PlayerController playerController = GameObject.Find("Player").GetComponent<PlayerController>();
            playerController.items = playerController.items.Where(x => x.tokenId != tokenId).ToList();
        }
    }
}

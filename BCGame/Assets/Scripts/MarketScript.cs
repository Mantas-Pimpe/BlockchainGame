using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketScript : MonoBehaviour
{
    public GameObject listTemplate;
    public GameObject container;

    private List<ListingEntity> listings;
    private List<GameObject> listingsGameObjects;

    private bool inited = false;

    public bool isInited(){
        return inited;
    }

    // Start is called before the first frame update
    void Start()
    {
        listingsGameObjects = new List<GameObject>() {};
        ReloadTableData();
        inited = true;
    }
    
    async public void ReloadTableData(){
        if(listingsGameObjects.Count > 0){
            foreach (GameObject gameObject in listingsGameObjects){
                DestroyImmediate(gameObject);
            }
        }

        listings = RestApi.GetListings().list;
        int i=1;
        foreach (ListingEntity listing in listings){
            //Debug.Log(listing.listingId);
            GameObject listItemClone = Instantiate(listTemplate);

            listItemClone.transform.SetParent(container.transform, false);
            listItemClone.name = "ListItems_" + i;
            i++;

            listItemClone.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = GetBoxText(listing);
            listItemClone.transform.GetChild(0).GetComponent<BuyScript>().listingId = listing.listingId;
            listItemClone.transform.GetChild(3).GetComponent<BuyScript>().listingId = listing.listingId;
            Texture2D texture = await HelperScript.GetRemoteTexture(listing.uri);
            if(texture != null){
                listItemClone.transform.GetChild(1).GetChild(0).GetComponent<RawImage>().texture = texture;
            }
            if(listing.status != "Active"){
                listItemClone.transform.GetChild(0).GetComponent<Button>().interactable = false;
                listItemClone.transform.GetChild(3).GetComponent<Button>().interactable = false;
            }

            listingsGameObjects.Add(listItemClone);
        }
    }

    private string GetBoxText(ListingEntity listing){
        return 
            "Name: " + listing.name + "\r\n" +
            "Status: " + listing.status + "\r\n" +
            "Price " + listing.price.ToString() + "\r\n";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

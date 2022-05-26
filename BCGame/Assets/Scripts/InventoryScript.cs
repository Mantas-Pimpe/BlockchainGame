using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryScript : MonoBehaviour
{
    public GameObject listTemplate;
    public GameObject container;

    private List<TokenEntity> tokens;
    private List<GameObject> tokensGameObjects;

    private bool inited = false;

    public bool isInited(){
        return inited;
    }

    // Start is called before the first frame update
    void Start()
    {
        tokensGameObjects = new List<GameObject>() {};
        ReloadTableData();
        inited = true;
    }
    
    async public void ReloadTableData(){
        tokens = GameObject.Find("Player").GetComponent<PlayerController>().items;
        if(tokensGameObjects.Count > 0){
            foreach (GameObject gameObject in tokensGameObjects){
                DestroyImmediate(gameObject);
            }
        }
        
        int i=1;
        foreach (TokenEntity token in tokens){
            GameObject inventoryItemClone = Instantiate(listTemplate);

            inventoryItemClone.transform.SetParent(container.transform, false);
            inventoryItemClone.name = "InventoryItems_" + token.tokenId;
            i++;

            inventoryItemClone.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = token.name;
            inventoryItemClone.transform.GetChild(0).GetComponent<SellScript>().tokenId = token.tokenId;
            inventoryItemClone.transform.GetChild(1).GetChild(0).GetComponent<RawImage>().texture = await HelperScript.GetRemoteTexture(token.uri);
            Texture2D texture = await HelperScript.GetRemoteTexture(token.uri);
            if(texture != null){
                inventoryItemClone.transform.GetChild(1).GetChild(0).GetComponent<RawImage>().texture = texture;
            }

            tokensGameObjects.Add(inventoryItemClone);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

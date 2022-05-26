using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RestApi {
    public static string apiUrl = "http://localhost:3000/";

    public static ListingEntityList GetListings(){
        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(apiUrl + "listings");
        request.Method = "GET";
        HttpWebResponse response = (HttpWebResponse) request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();
        //Debug.Log(json);
        return JsonUtility.FromJson<ListingEntityList>("{\"list\":" + json + "}"); 
        //C# does not support parsing arrays of objects - [], we turn it into a class with an array variable
    }

    public static ListingEntity GetListing(int listingId){
        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(apiUrl + "listings/" + listingId);
        Debug.Log("GetListing " + apiUrl + "listings/" + listingId);
        request.Method = "GET";
        HttpWebResponse response = (HttpWebResponse) request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();
        Debug.Log(json);
        return JsonUtility.FromJson<ListingEntity>(json);
    }

    public static void CreateAndListToken(string name, string uri, int price){
        TokenEntity token = CreateToken(name, uri);
        CreateListing(token.tokenId, price);
    }

    public static TokenEntity CreateToken(string name, string uri){
        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(apiUrl + "tokens");
        request.Method = "POST";
        request.ContentType = "application/json";

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            TokenAttributeEntity tokenAttributeEntity = new TokenAttributeEntity();
            tokenAttributeEntity.name = name;
            tokenAttributeEntity.uri = uri;

            string jsonWriter = JsonUtility.ToJson(tokenAttributeEntity);
            streamWriter.Write(jsonWriter);
        }

        HttpWebResponse response = (HttpWebResponse) request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();
        //Debug.Log(json);
        return JsonUtility.FromJson<TokenEntity>(json); 
    }

    public static void CreateListing(int tokenId, int price){

        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(apiUrl + "listings");
        request.Method = "POST";
        request.ContentType = "application/json";

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            CreateListingEntity listingParams = new CreateListingEntity();
            listingParams.tokenId = tokenId;
            listingParams.price = price;

            string jsonWriter = JsonUtility.ToJson(listingParams);
            streamWriter.Write(jsonWriter);
        }

        HttpWebResponse response = (HttpWebResponse) request.GetResponse();
        //TODO handle if needed
    }

    public static BuyEntity BuyToken(int listingId){
        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(apiUrl + "listings/buy/" + listingId);
        request.Method = "POST";
        HttpWebResponse response = (HttpWebResponse) request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();
        return JsonUtility.FromJson<BuyEntity>(json);
    }

    public static ListingEntity CancelListing(int listingId){
        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(apiUrl + "listings/cancel/" + listingId);
        request.Method = "POST";
        HttpWebResponse response = (HttpWebResponse) request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();
        return JsonUtility.FromJson<ListingEntity>(json);
    }

}

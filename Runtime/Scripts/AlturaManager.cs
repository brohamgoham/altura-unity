using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;    
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AlturaWeb3.SDK {
    /// <summary>
    /// Altura SDK for Unity
    /// </summary>

    public class AlturaClient 
    {
        public class Response<T> { public T response; }

        public readonly static string BASE_URL = "https://api.alturanft.com/api/v2/";

        /// <summary>
        /// Initializes a new instance of the <see cref="AlturaWeb3.SDK.AlturaClient"/> class.
        /// </summary>

        // token 
        private static string token;
        private string userId;
        private string userName;
        private string userEmail;
        private string userAvatar;


        /// <summary>
        /// Create query params object from dictionary. add ? to the beginning of the query string and add & to the end of the query string.
        /// perPage=20&page=1&sortBy="name"&sortOrder="desc"&slim=true
        /// </summary>

        public static string CreateQueryParams(Dictionary<string, string> queryParams)
        {
            string query = "?";
            foreach (KeyValuePair<string, string> entry in queryParams)
            {
                query += entry.Key + "=" + entry.Value + "&";
            }
            return query;
        }


        /// <summary>
        /// add apiKey to the header of the request
        /// </summary>
        private static void AddApiKey(UnityWebRequest request)
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);
        }

        /// <summary>
        /// builder for the request
        /// </summary>
        private static UnityWebRequest BuildRequest(string url, string method, Dictionary<string, string> queryParams, Dictionary<string, string> headers, byte[] body)
        {
            UnityWebRequest request = new UnityWebRequest(url, method);
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> entry in headers)
                {
                    request.SetRequestHeader(entry.Key, entry.Value);
                }
            }
            if (queryParams != null)
            {
                request.url += CreateQueryParams(queryParams);
            }
            if (body != null)
            {
                request.uploadHandler = new UploadHandlerRaw(body);
            }
            return request;
        }

        /// <summary>
        /// send the request and return the response
        /// </summary>
        private static async Task<Response<T>> SendRequest<T>(UnityWebRequest request)
        {
            Response<T> response = new Response<T>();
            await request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
                response.response = default(T);
            }
            else
            {
                response.response = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
            }
            return response;
        }


        /// <summary>
        /// Builder for Unity Get request
        /// </summary>
        private static async Task<Response<T>> Get<T>(string url, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
        {
            UnityWebRequest request = BuildRequest(url, UnityWebRequest.kHttpVerbGET, queryParams, headers, null);
            return await SendRequest<T>(request);
        }

        /// <summary>
        /// Calls the user/verify_auth_code endpoint. with address and code
        /// returns true or false, no query params
        /// </summary>
        public async Task<bool> VerifyAuthCode(string address, string code)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("address", address);
            queryParams.Add("code", code);
            Response<bool> response = await Get<bool>("user/verify_auth_code", queryParams, null);
            return response.response;
        }

        /// <summary>
        /// Calls the "user" endpoint. queryParams
        /// returns the  many user objects
        /// </summary>
   //     public async Task<List<User>> GetUsers(Dictionary<string, string> queryParams)
     //   {
         //   Response<List<User>> response = await Get<List<User>>("user", queryParams, null);
       //     return response.response;
       // }

        /// <summary>
        /// Calls the "user/:address" endpoint. queryParams
        /// returns the  user object
        /// </summary>
        public async Task<User> GetUser(string address, Dictionary<string, string> queryParams)
        {
            Response<User> response = await Get<User>("user/" + address, queryParams, null);
            return response.response;
        }
        public static async Task<string> GetUsers(string queryParams)
        {
            UnityWebRequest request = UnityWebRequest.Get(BASE_URL + "user" + queryParams);
            await request.SendWebRequest();
                return request.downloadHandler.text;
            
        }

        


        /// <summary>
        /// Calls the "item" endpoint. queryParams determing how many perPage, page, sortBy, sortOrder, and slim
        /// returns the  many item objects
        /// </summary>
        public async Task<List<Item>> GetItems(Dictionary<string, string> queryParams)
        {
            UnityWebRequest request = UnityWebRequest.Get(BASE_URL + "item" + queryParams);
            await request.SendWebRequest();
            return request.downloadHandler.text;
        }
        

        /// <summary>
        /// Calls the "item/:address/:tokenId" endpoint. queryParams
        /// returns the  a single item object takes in address and tokenId
        /// </summary>
        public async Task<Item> GetItem(string address, string tokenId)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("address", address);
            queryParams.Add("token_id", tokenId);
            Response<Item> response = await Get<Item>("item/" + address + "/" + tokenId, queryParams, null);
            return response.response;
        }
   
        /// <summary>
        ///Calls the "item/holders/:address/:tokenId" endpoint. queryParams
        /// returns the  a single item object takes in address and tokenId
        /// </summary>
    
    /*
        public async Task<List<Holder>> GetItemHolders(string address, string tokenId)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("address", address);
            queryParams.Add("token_id", tokenId);
            Response<List<Holder>> response = await Get<List<Holder>>("item/holders/" + address + "/" + tokenId, queryParams, null);
            return response.response;
        }
    */

        /// <summary>
        /// Calls the "item/events/:address/:tokenId" endpoint. queryParams
        /// returns the  a single item object takes in address and tokenId
        /// </summary>
        public async Task<List<Event>> GetItemEvents(string address, string tokenId)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("address", address);
            queryParams.Add("token_id", tokenId);
            Response<List<Event>> response = await Get<List<Event>>("item/events/" + address + "/" + tokenId, queryParams, null);
            return response.response;
        }

        
        /// <summary>
        /// Calls the "item/activity" endpoint. queryParams
        /// returns the  a single item object takes in address and tokenId
        /// </summary>
    

    /*
        public async Task<List<Activity>> GetItemActivity(Dictionary<string, string> queryParams)
        {
            Response<List<Activity>> response = await Get<List<Activity>>("item/activity", queryParams, null);
            return response.response;
        }

    */

        /// <summary>
        /// Calls the "collection/:address" endpoint. queryParams
        /// returns the  a single collection object takes in address
        /// </summary>
        public async Task<Collections> GetCollection(string address, Dictionary<string, string> queryParams)
        {
            Response<Collections> response = await Get<Collections>("collection/" + address, queryParams, null);
            return response.response;
        }

        /// <summary>
        /// Calls the "collection/" endpoint. queryParams
        /// returns the  all collections based on queryParams perPage, page, sortBy, sortOrder, and slim
        /// </summary>
        public async Task<List<Collections>> GetCollections(Dictionary<string, string> queryParams)
        {
            Response<List<Collections>> response = await Get<List<Collections>>("collection", queryParams, null);
            return response.response;
        }




        /// <summary>
        /// Calls the "/api/v2/item/transfer" endpoint. queryParams
        /// user must be authenticated to use this endpoint
        /// takes in collectionAddress, tokenId, toAddress, and returns txnHash string
        /// POST request
        /// </summary>
        
        /*
        public async Task<string> TransferItem(string collectionAddress, string tokenId, string toAddress)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("collection_address", collectionAddress);
            queryParams.Add("token_id", tokenId);
            queryParams.Add("to_address", toAddress);
       //     Response<string> response = await Post<string>("item/transfer", queryParams, null);
       
            return response.response;
        }
        */
 
        /// <summary>
        /// Calls the "/api/v2/item/transfer" endpoint. queryParams
        /// user must be authenticated to use this endpoint
        /// takes in collectionAddress, tokenId, toAddress, and returns txnHash string
        /// send bulk transfer POST request
        /// </summary>
/*
        public async Task<string> TransferItems(List<TransferItem> transferItems)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("collection_address", transferItems[0].collectionAddress);
            queryParams.Add("token_id", transferItems[0].tokenId);
            queryParams.Add("to_address", transferItems[0].toAddress);
            Response<string> response = await Post<string>("item/transfer", queryParams, transferItems);
            return response.response;
        }

*/



        /// <summary>
        /// Calls the '/api/v2/item/mint' constrainst must be authenticated to use this endpoint
        /// takes in address tokenId, amount and to, returns txnHash string
        /// POST request
        /// </summary>
        
        /*
        public async Task<string> MintItem(string address, string tokenId, string to, decimal amount)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("address", address);
            queryParams.Add("token_id", tokenId);
            queryParams.Add("to", to);
            queryParams.Add("amount", amount.ToString());
            Response<string> response = await Post<string>("item/mint", queryParams, null);
            return response.response;
        }
        */
    }

    [Serializable]
    public class User
    {   
        public string Address;
        public string Name;
        public string Bio;
        public string ProfilePic;
        public string SocialLink;
        public string ProfilePicUrl;
    }

    [Serializable]
    public class AuthCode {
        public string address;
        public string code;
    }

    [Serializable]
    public class AlturaItemProperty {
        public string name;
        public string value;
        public bool isStatic;
    }

    [Serializable]
public class Item
{
  public string name;
  public string description;
  public List<AlturaItemProperty> properties;
  public int chainId;
  public decimal royalty;
  public string creatorAddress;
  public string mintDate;
  public bool stackable;
  public decimal supply;
  public decimal maxSupply;
  public string image;
  public string imageHash;
  public string imageUrl;
  public string fileType;
  public bool isVideo;
  public string otherImageVisibility;
  public decimal holders;
  public decimal listers;
  public decimal likes;
  public decimal views;
  public bool isListed;
  public string mostRecentListing;
  public decimal cheapestListingPrice;
  public string cheapestListingCurrency;
  public decimal cheapestListingUSD;
  public bool nsfw;
  public bool isVerified;
  public bool hasUnlockableContent;
  public int imageIndex;
  public int imageCount;
  public int totalListings;
}

    [Serializable]
    public class Collections 
    {
       public string Address;
  public string Name;
  public string Description;
  public string Genre;
  public string Image;
  public string ImageHash;
  public string OwnerAddress;
  public string Slug;
  public string Uri;
  public string Website;
  public int Holders;
  public int Volume1D;
  public int Volume1W;
  public int Volume30D;
  public int VolumeAll;
  public string ImageUrl;
  public int ChainId;
  public string MintDate;
    }

[Serializable]
    public class AlturaEvent {
  public string id;
  public string amount;
  public long blockNumber;
  public long chainId;
  public string eventz;
  public string from;
  public string itemCollection;
  public string itemRef;
  public long timestamp;
  public string to;
  public long tokenId;
  public string transactionHash;
}


}

 /// <summary>
    /// Class to enable asynchronous web requests via Unity 
    /// </summary>
    public class UnityWebRequestAwaiter : INotifyCompletion {
        private UnityWebRequestAsyncOperation asyncOp;
        private Action continuation;

        public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp) {
            this.asyncOp = asyncOp;
            asyncOp.completed += OnRequestCompleted;
        }

        public bool IsCompleted { get { return asyncOp.isDone; } }

        public void GetResult() { }

        public void OnCompleted(Action continuation) {
            this.continuation = continuation;
        }

        private void OnRequestCompleted(AsyncOperation obj) {
            continuation();
        }
    }

    public static class ExtensionMethods {
        public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp) {
            return new UnityWebRequestAwaiter(asyncOp);
        }
    }

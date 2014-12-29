Examples in C#
==============

You can use your own REST client such as RestSharp or just use HttpClient as in example below.

```csharp
  // GsnBaseUrl: https://clientapi.gsn2.com/api/v1
  // GsnClienId: client_id
  // GsnClientSecret: client_secret
  var clientId = ConfigurationManager.AppSettings["GsnClienId"];
  var apiClient = new GsnApiClient(
          ConfigurationManager.AppSettings["GsnBaseUrl"], 
          clientId , 
          ConfigurationManager.AppSettings["GsnClientSecret"]);
          
  // example retrieving stores
  List<dynamic> stores = apiClient.GetContent<List<dynamic>>("/store/List/" + clientId, 
          new Dictionary<string, string>());
  
  // example retrieving profile
  var profile = apiClient.GetContent<dynamic>("/profile/By/" + someProfileId,
          new Dictionary<string, string>() { { "site_id", clientId } });
              
  // Example on posting data
  var result = apiClient.GetPostContent<dynamic>("/profile/Update", 
          new Dictionary<string, string>() { { "FirstName", "Tom" }, { "LastName", "Test" }, etc ... },
          new Dictionary<string, string>() { { "site_id", clientId }, { "profile_id", someProfileId } });
  
````

Note
=====

There are quirky issues with the provided generic GetContent<T> or PostContent<T> method.  It doesn't perform json deserialization very well for Strongly Typed object or Enumerable such as generic List or array.  This is to be expected because default .NET Json handling is not very good.  You should use something like Json.NET JsonConvert to deserialize these objects as demonstrated in the example below:


```csharp  
  // Example on posting data
  var data = gsnApi.DoGet("/profile/By/" + someProfileId,
          new Dictionary<string, string>() { { "site_id", clientId } });
  var content = data.Content.ReadAsStringAsync();
  return JsonConvert.DeserializeObject<Profile>(content.Result);  
````


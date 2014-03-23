Examples in C#
==============

You can use your own REST client such as RestSharp or just use HttpClient as in example below.

```csharp
  // GsnBaseUrl: https://clientapi.gsn2.com/api/v1
  // GsnClienId: client_id
  // GsnClientSecret: client_secret
  var clientId = ConfigurationManager.AppSettings["GsnClienId"];
  var apiClient = new GsnApiClient(
          ConfigurationManager.AppSettings["GsnBaseUrl"], clientId , ConfigurationManager.AppSettings["GsnClientSecret"]);
          
  // example retrieving stores
  List<dynamic> stores = apiClient.GetContent<List<dynamic>>("/store/List/" + clientId, new Dictionary<string, string>());
  
  // example retrieving profile
  var profile = apiClient.GetContent<dynamic>("/profile/By/" + someProfileId,
              new Dictionary<string, string>() { { "site_id", clientId } });
              
  // Example on posting data
  var result = apiClient.GetPostContent<dynamic>("/profile/Update", 
                        new Dictionary<string, string>() { { "FirstName", "Tom" }, { "LastName", "Test" }, etc ... },
                        new Dictionary<string, string>() { { "site_id", clientId }, { "profile_id", someProfileId } }
                        )
  
````
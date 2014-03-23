// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GsnApiClient.cs" company="GSN">
//   GSN @ 2013
// </copyright>
// <summary>
//   Api client for GSN.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Gsn.Digital.Web.Infrastructure
{
  using System;
  using System.Collections.Generic;
  using System.Net.Http;
  using System.Net.Http.Formatting;
  using System.Net.Http.Headers;

  /// <summary>
  /// GSN rest API client.
  /// </summary>
  public class GsnApiClient
  {
    /// <summary>
    /// The base URL
    /// </summary>
    private readonly string baseUrl;

    /// <summary>
    /// The base path
    /// </summary>
    private readonly string basePath;

    /// <summary>
    /// The client secret
    /// </summary>
    private readonly string clientSecret;

    /// <summary>
    /// The _sync lock
    /// </summary>
    private static Object _syncLock = new Object();

    /// <summary>
    /// Initializes a new instance of the <see cref="GsnApiClient" /> class.
    /// </summary>
    /// <param name="baseUrl">The base URL.</param>
    /// <param name="clientId">The client id.</param>
    /// <param name="clientSecret">The client secret.</param>
    public GsnApiClient(string baseUrl, string clientId, string clientSecret)
    {
      var url = new Uri((baseUrl + string.Empty).Trim());
      this.baseUrl = url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
      this.basePath = (url.PathAndQuery + string.Empty).Trim().TrimEnd('/');
      this.ClientId = clientId;
      this.clientSecret = clientSecret;
    }

    /// <summary>
    /// Gets the client id.
    /// </summary>
    /// <value>
    /// The client id.
    /// </value>
    public string ClientId { get; private set; }

    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>
    /// The token.
    /// </value>
    public GsnApiTokenModel Token { get; private set; }

    /// <summary>
    /// Tries the authentication.
    /// </summary>
    public void TryAuthentication()
    {
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(this.baseUrl);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var content =
            new FormUrlEncodedContent(
                new[]
                                {
                                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                                    new KeyValuePair<string, string>("client_id", this.ClientId),
                                    new KeyValuePair<string, string>("client_secret", this.clientSecret)    
                                });

        if (this.Token == null || (DateTime.Now > this.Token.ExpireDate))
        {
          lock (_syncLock)
          {
            if (this.Token == null || (DateTime.Now > this.Token.ExpireDate))
            {
              var result = client.PostAsync(this.basePath + "/auth/token", content).Result;

              // TODO: handle error from authentication
              // something went wrong?  server is down? send out email?

              // if successful, store the token
              this.Token = result.Content.ReadAsAsync<GsnApiTokenModel>().Result;
              this.Token.ExpireDate = DateTime.Now.AddSeconds(this.Token.expires_in);
            }
          }
        }
      }
    }

    /// <summary>
    /// Perform a HttpPost
    /// </summary>
    /// <param name="methodNameRelativeToBaseUrl">The method name relative to base URL.</param>
    /// <param name="body">The body.</param>
    /// <param name="additionalHeaders">The additional headers.</param>
    /// <returns></returns>
    public HttpResponseMessage DoPost(
        string methodNameRelativeToBaseUrl,
        IEnumerable<KeyValuePair<string, string>> body,
        IDictionary<string, string> additionalHeaders)
    {
      this.TryAuthentication();

      if (!methodNameRelativeToBaseUrl.StartsWith("/"))
      {
        methodNameRelativeToBaseUrl = "/" + methodNameRelativeToBaseUrl;
      }

      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(this.baseUrl);
        client.DefaultRequestHeaders.Add("access_token", this.Token.access_token);
        foreach (var k in additionalHeaders.Keys)
        {
          client.DefaultRequestHeaders.Add(k, additionalHeaders[k]);
        }

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client.PostAsync(this.basePath + methodNameRelativeToBaseUrl, body == null ? null : new FormUrlEncodedContent(body)).Result;
      }
    }

    /// <summary>
    /// Does the post.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="methodNameRelativeToBaseUrl">The method name relative to base URL.</param>
    /// <param name="body">The body.</param>
    /// <param name="additionalHeaders">The additional headers.</param>
    /// <returns></returns>
    public HttpResponseMessage DoPost<T>(
                string methodNameRelativeToBaseUrl,
                T body,
                IDictionary<string, string> additionalHeaders)
    {
      this.TryAuthentication();

      if (!methodNameRelativeToBaseUrl.StartsWith("/"))
      {
        methodNameRelativeToBaseUrl = "/" + methodNameRelativeToBaseUrl;
      }

      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(this.baseUrl);
        client.DefaultRequestHeaders.Add("access_token", this.Token.access_token);
        foreach (var k in additionalHeaders.Keys)
        {
          client.DefaultRequestHeaders.Add(k, additionalHeaders[k]);
        }

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client.PostAsync(this.basePath + methodNameRelativeToBaseUrl, new ObjectContent<T>(body, new JsonMediaTypeFormatter())).Result;
      }
    }

    /// <summary>
    /// Perform a HttpGet
    /// </summary>
    /// <param name="methodNameRelativeToBaseUrl">The method name relative to base URL.</param>
    /// <param name="additionalHeaders">The additional headers.</param>
    /// <returns></returns>
    public HttpResponseMessage DoGet(
                string methodNameRelativeToBaseUrl,
                IDictionary<string, string> additionalHeaders)
    {
      this.TryAuthentication();
      if (!methodNameRelativeToBaseUrl.StartsWith("/"))
      {
        methodNameRelativeToBaseUrl = "/" + methodNameRelativeToBaseUrl;
      }

      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(this.baseUrl);
        client.DefaultRequestHeaders.Add("access_token", this.Token.access_token);
        foreach (var k in additionalHeaders.Keys)
        {
          client.DefaultRequestHeaders.Add(k, additionalHeaders[k]);
        }

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client.GetAsync(this.basePath + methodNameRelativeToBaseUrl).Result;
      }
    }

    /// <summary>
    /// Gets the content.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="methodNameRelativeToBaseUrl">The method name relative to base URL.</param>
    /// <param name="additionalHeaders">The additional headers.</param>
    /// <returns></returns>
    public T GetContent<T>(string methodNameRelativeToBaseUrl, IDictionary<string, string> additionalHeaders)
        where T : class
    {
      var result = this.DoGet(methodNameRelativeToBaseUrl, additionalHeaders);
      if (result.IsSuccessStatusCode)
      {
        return result.Content.ReadAsAsync<T>().Result;
      }

      return null;
    }

    /// <summary>
    /// Gets the content of the post.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="methodNameRelativeToBaseUrl">The method name relative to base URL.</param>
    /// <param name="body">The body.</param>
    /// <param name="additionalHeaders">The additional headers.</param>
    /// <returns></returns>
    public T GetPostContent<T>(string methodNameRelativeToBaseUrl,
        IEnumerable<KeyValuePair<string, string>> body,
        IDictionary<string, string> additionalHeaders)
                where T : class
    {
      var result = this.DoPost(methodNameRelativeToBaseUrl, body, additionalHeaders);
      if (result.IsSuccessStatusCode)
      {
        return result.Content.ReadAsAsync<T>().Result;
      }

      return null;
    }
  }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GsnAuthTokenModel.cs" company="GSN">
//   GSN @ 2013
// </copyright>
// <summary>
//   GSN auth token response model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Gsn.Digital.Web.Infrastructure
{
  using System;

  /// <summary>
  ///  GSN auth token response model.
  /// </summary>
  public class GsnApiTokenModel
  {
    /// <summary>
    /// Gets or sets the access_token.
    /// </summary>
    /// <value>
    /// The access_token.
    /// </value>
    public string access_token { get; set; }

    /// <summary>
    /// Gets or sets the refresh_token.
    /// </summary>
    /// <value>
    /// The refresh_token.
    /// </value>             
    public string refresh_token { get; set; }

    /// <summary>
    /// Gets or sets the token_type.
    /// </summary>
    /// <value>
    /// The token_type.
    /// </value>            
    public string token_type { get; set; }

    /// <summary>
    /// Gets or sets the expiration in seconds.
    /// </summary>
    /// <value>
    /// The expiration in seconds.
    /// </value>
    public int expires_in { get; set; }

    /// <summary>
    /// Gets or sets the scope.
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>            
    public string scope { get; set; }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    /// <value>
    /// The user id.
    /// </value>        
    public int? user_id { get; set; }

    /// <summary>
    /// Gets or sets the expire date.
    /// </summary>
    /// <value>
    /// The expire date.
    /// </value>
    public DateTime ExpireDate { get; set; }
  }
}
﻿using System.Text;


namespace Sitecore.MobileSDK.PublicKey
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using Sitecore.MobileSDK.API;
  using Sitecore.MobileSDK.TaskFlow;
  using Sitecore.MobileSDK.SessionSettings;
  using Sitecore.MobileSDK.UrlBuilder.Rest;
  using Sitecore.MobileSDK.UrlBuilder.SSC;
  using Sitecore.MobileSDK.PasswordProvider.Interface;
  using Sitecore.MobileSDK.API.Exceptions;

  public class GetPublicKeyTasks : IRestApiCallTasks<ISessionConfig, string, string, string>
  {
    #region Private Variables

    private readonly SessionConfigUrlBuilder sessionConfigBuilder;
    private readonly ISSCUrlParameters sscGrammar;
    private readonly HttpClient httpClient;
    private readonly IScCredentials credentials;

    #endregion Private Variables

    public GetPublicKeyTasks(IScCredentials credentials, SessionConfigUrlBuilder sessionConfigBuilder, ISSCUrlParameters sscGrammar, HttpClient httpClient)
    {
      this.sessionConfigBuilder = sessionConfigBuilder;
      this.sscGrammar = sscGrammar;
      this.httpClient = httpClient;
      this.credentials = credentials;
    }

    #region IRestApiCallTasks

    public string BuildRequestUrlForRequestAsync(ISessionConfig request, CancellationToken cancelToken)
    {
      return this.PrepareRequestUrl(request);
    }

    public async Task<string> SendRequestForUrlAsync(string requestUrl, CancellationToken cancelToken)
    {
      Debug.WriteLine("REQUEST: " + requestUrl);

      //TODO: @igk extract
      var stringContent = new StringContent("{\"domain\":\""
                                            + this.credentials.Domain
                                            +"\",\"username\":\""
                                            + this.credentials.Username
                                            + "\",\"password\":\""
                                            + this.credentials.Password
                                            + "\"}", Encoding.UTF8, "application/json");
      
      HttpResponseMessage httpResponse = await this.httpClient.PostAsync(requestUrl, stringContent, cancelToken);

      if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK) {
        throw new SitecoreMobileSdkException("status code is " + httpResponse.StatusCode.ToString());
      }

      return httpResponse.StatusCode.ToString();
    }

    public async Task<string> ParseResponseDataAsync(string status, CancellationToken cancelToken)
    {
        Func<string> syncParsePublicKey = () =>
        {
          return status;
        };
        string result = await Task.Factory.StartNew(syncParsePublicKey, cancelToken);
        return result;
    }

    private string PrepareRequestUrl(ISessionConfig instanceUrl)
    {
      string url = this.sessionConfigBuilder.BuildUrlString(instanceUrl);

      //FIXME: hack to force https protocol
      if (!url.StartsWith("https", StringComparison.CurrentCulture))
      {
        url = url.Insert(4, "s");
      }

      url = url + this.sscGrammar.ItemSSCAuthEndpoint
                + this.sscGrammar.ItemSSCLoginAction;
      
      return url;
    }

    #endregion IRestApiCallTasks
  }
}


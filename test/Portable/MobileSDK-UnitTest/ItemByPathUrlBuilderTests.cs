﻿namespace Sitecore.MobileSdkUnitTest
{
  using System;
  using NUnit.Framework;
  using Sitecore.MobileSDK.API;
  using Sitecore.MobileSDK.API.Request;
  using Sitecore.MobileSDK.API.Request.Parameters;
  using Sitecore.MobileSDK.MockObjects;
  using Sitecore.MobileSDK.SessionSettings;
  using Sitecore.MobileSDK.UrlBuilder.ItemByPath;
  using Sitecore.MobileSDK.UrlBuilder.QueryParameters;
  using Sitecore.MobileSDK.UrlBuilder.Rest;
  using Sitecore.MobileSDK.UrlBuilder.SSC;
  using Sitecore.MobileSDK.UserRequest;

  [TestFixture]
  public class ItemByPathUrlBuilderTests
  {
    private ItemByPathUrlBuilder builder;
    private ISessionConfig sessionConfig;
    private QueryParameters payload;

    [SetUp]
    public void SetUp()
    {
      IRestServiceGrammar restGrammar = RestServiceGrammar.ItemSSCV2Grammar();
      ISSCUrlParameters webApiGrammar = SSCUrlParameters.ItemSSCV2UrlParameters();

      this.builder = new ItemByPathUrlBuilder(restGrammar, webApiGrammar);

      SessionConfigPOD mutableSession = new SessionConfigPOD();
      mutableSession.InstanceUrl = "http://mobiledev1ua1.dk.sitecore.net";
      this.sessionConfig = mutableSession;

      this.payload = new QueryParameters(null);
    }

    [TearDown]
    public void TearDown()
    {
      this.builder = null;
      this.sessionConfig = null;
      this.payload = null;
    }

    [Test]
    public void TestNullPayloadIsNotReplacedWithDefault()
    {
      MockGetItemsByPathParameters mutableParameters = new MockGetItemsByPathParameters();
      mutableParameters.ItemSource = LegacyConstants.DefaultSource();
      mutableParameters.ItemPath = "/path/TO/iTEm";
      mutableParameters.SessionSettings = this.sessionConfig;
      mutableParameters.QueryParameters = new QueryParameters(null);

      IReadItemsByPathRequest request = mutableParameters;

      string result = this.builder.GetUrlForRequest(request);
      string expected = "http://mobiledev1ua1.dk.sitecore.net/sitecore/api/ssc/item?database=web&language=en&path=%2fpath%2fto%2fitem";

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void TestNullPayloadStructIsIgnored()
    {
      MockGetItemsByPathParameters mutableParameters = new MockGetItemsByPathParameters();
      mutableParameters.ItemSource = LegacyConstants.DefaultSource();
      mutableParameters.ItemPath = "/path/TO/iTEm";
      mutableParameters.SessionSettings = this.sessionConfig;
      mutableParameters.QueryParameters = null;

      IReadItemsByPathRequest request = mutableParameters;

      string result = this.builder.GetUrlForRequest(request);
      string expected = "http://mobiledev1ua1.dk.sitecore.net/sitecore/api/ssc/item?database=web&language=en&path=%2fpath%2fto%2fitem";

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void TestBuildWithValidPath()
    {
      MockGetItemsByPathParameters mutableParameters = new MockGetItemsByPathParameters();
      mutableParameters.ItemSource = LegacyConstants.DefaultSource();
      mutableParameters.ItemPath = "/path/TO/iTEm";
      mutableParameters.SessionSettings = this.sessionConfig;
      mutableParameters.QueryParameters = this.payload;

      IReadItemsByPathRequest request = mutableParameters;

      string result = this.builder.GetUrlForRequest(request);
      string expected = "http://mobiledev1ua1.dk.sitecore.net/sitecore/api/ssc/item?database=web&language=en&path=%2fpath%2fto%2fitem";

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void TestBuildWithoutPayloadIsAllowed()
    {
      MockGetItemsByPathParameters mutableParameters = new MockGetItemsByPathParameters();
      mutableParameters.ItemSource = LegacyConstants.DefaultSource();
      mutableParameters.ItemPath = "/path/TO/iTEm";
      mutableParameters.SessionSettings = this.sessionConfig;
      mutableParameters.QueryParameters = null;

      IReadItemsByPathRequest request = mutableParameters;
      string result = this.builder.GetUrlForRequest(request);
      string expected = "http://mobiledev1ua1.dk.sitecore.net/sitecore/api/ssc/item?database=web&language=en&path=%2fpath%2fto%2fitem";

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void TestBuildWithUnEscapedPath()
    {
      MockGetItemsByPathParameters mutableParameters = new MockGetItemsByPathParameters();
      mutableParameters.ItemSource = LegacyConstants.DefaultSource();
      mutableParameters.ItemPath = "/path TO iTEm";
      mutableParameters.SessionSettings = this.sessionConfig;
      mutableParameters.QueryParameters = this.payload;

      IReadItemsByPathRequest request = mutableParameters;

      string result = this.builder.GetUrlForRequest(request);
      string expected = "http://mobiledev1ua1.dk.sitecore.net/sitecore/api/ssc/item?database=web&language=en&path=%2fpath%20to%20item";

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void TestBuildWithEmptyConfig()
    {
      TestDelegate action = () => this.builder.GetUrlForRequest(null);
      Assert.Throws<ArgumentNullException>(action);
    }

    [Test]
    public void TestPathMustStartWithSlash()
    {
      MockGetItemsByPathParameters mutableParameters = new MockGetItemsByPathParameters();
      mutableParameters.ItemSource = LegacyConstants.DefaultSource();
      mutableParameters.ItemPath = "path without starting slash";
      mutableParameters.SessionSettings = this.sessionConfig;
      mutableParameters.QueryParameters = this.payload;

      IReadItemsByPathRequest request = mutableParameters;

      TestDelegate action = () => this.builder.GetUrlForRequest(request);
      Assert.Throws<ArgumentException>(action);
    }

    [Test]
    public void TestBuildWithEmptyPathCausesArgumentException()
    {
      MockGetItemsByPathParameters mutableParameters = new MockGetItemsByPathParameters();
      mutableParameters.ItemSource = LegacyConstants.DefaultSource();
      mutableParameters.ItemPath = "";
      mutableParameters.SessionSettings = this.sessionConfig;
      mutableParameters.QueryParameters = this.payload;

      IReadItemsByPathRequest request = mutableParameters;

      TestDelegate action = () => this.builder.GetUrlForRequest(request);
      Assert.Throws<ArgumentException>(action);
    }


    [Test]
    public void TestBuildWithWhitespacePathCausesArgumentException()
    {
      MockGetItemsByPathParameters mutableParameters = new MockGetItemsByPathParameters();
      mutableParameters.ItemSource = LegacyConstants.DefaultSource();
      mutableParameters.ItemPath = "\r\n\t";
      mutableParameters.SessionSettings = this.sessionConfig;
      mutableParameters.QueryParameters = this.payload;


      IReadItemsByPathRequest request = mutableParameters;

      TestDelegate action = () => this.builder.GetUrlForRequest(request);
      Assert.Throws<ArgumentException>(action);
    }


    [Test]
    public void TestBuildRequestWithPathAndFieldList()
    {
      MockGetItemsByPathParameters mutableParameters = new MockGetItemsByPathParameters();
      mutableParameters.ItemSource = LegacyConstants.DefaultSource();
      mutableParameters.ItemPath = "/path/TO/iTEm";
      mutableParameters.SessionSettings = this.sessionConfig;

      QueryParameters fieldsList = new QueryParameters( new string[2] { "x", "y" });
      mutableParameters.QueryParameters = fieldsList;

      IReadItemsByPathRequest request = mutableParameters;

      string result = this.builder.GetUrlForRequest(request);
      string expected = "http://mobiledev1ua1.dk.sitecore.net/sitecore/api/ssc/item?database=web&language=en&fields=x%2cy%2citemid%2citemname%2citempath%2cparentid%2ctemplateid%2ctemplatename%2citemlanguage%2citemversion%2cdisplayname&path=%2fpath%2fto%2fitem";
      Assert.AreEqual(expected, result);
    }


    [Test]
    public void TestOptionalSourceInSessionAndUserRequest()
    {
      var connection = new SessionConfig("localhost");

      var request = ItemSSCRequestBuilder.ReadItemsRequestWithPath("/sitecore/content/oO").Build();
      var requestMerger = new UserRequestMerger(connection, null, null);
      var mergedRequest = requestMerger.FillReadItemByPathGaps(request);

      var urlBuilder = new ItemByPathUrlBuilder(RestServiceGrammar.ItemSSCV2Grammar(), SSCUrlParameters.ItemSSCV2UrlParameters());

      string result = urlBuilder.GetUrlForRequest(mergedRequest);
      string expected = "http://localhost/sitecore/api/ssc/item?path=%2fsitecore%2fcontent%2foo";

      Assert.AreEqual(expected, result);
    }


    [Test]
    public void TestOptionalSourceAndExplicitPayload()
    {
      var connection = new SessionConfig("localhost");

      var request = ItemSSCRequestBuilder.ReadItemsRequestWithPath("/sitecore/content/oO")
        .Build();
      var requestMerger = new UserRequestMerger(connection, null, null);
      var mergedRequest = requestMerger.FillReadItemByPathGaps(request);

      var urlBuilder = new ItemByPathUrlBuilder(RestServiceGrammar.ItemSSCV2Grammar(), SSCUrlParameters.ItemSSCV2UrlParameters());

      string result = urlBuilder.GetUrlForRequest(mergedRequest);
      string expected = "http://localhost/sitecore/api/ssc/item?path=%2fsitecore%2fcontent%2foo";

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void TestExplicitDatabase()
    {
      var connection = new SessionConfig("localhost");

      var request = ItemSSCRequestBuilder.ReadItemsRequestWithPath("/sitecore/content/oO")
        .Database("master")
        .Build();
      var requestMerger = new UserRequestMerger(connection, null, null);
      var mergedRequest = requestMerger.FillReadItemByPathGaps(request);

      var urlBuilder = new ItemByPathUrlBuilder(RestServiceGrammar.ItemSSCV2Grammar(), SSCUrlParameters.ItemSSCV2UrlParameters());

      string result = urlBuilder.GetUrlForRequest(mergedRequest);
      string expected = "http://localhost/sitecore/api/ssc/item?database=master&path=%2fsitecore%2fcontent%2foo";

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void TestDatabaseAndExplicitLanguageAndPayload()
    {
      var connection = new SessionConfig("localhost");

      var request = ItemSSCRequestBuilder.ReadItemsRequestWithPath("/sitecore/content/oO")
        .Language("da")
        .Build();
      var requestMerger = new UserRequestMerger(connection, null, null);
      var mergedRequest = requestMerger.FillReadItemByPathGaps(request);

      var urlBuilder = new ItemByPathUrlBuilder(RestServiceGrammar.ItemSSCV2Grammar(), SSCUrlParameters.ItemSSCV2UrlParameters());

      string result = urlBuilder.GetUrlForRequest(mergedRequest);
      string expected = "http://localhost/sitecore/api/ssc/item?language=da&path=%2fsitecore%2fcontent%2foo";

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void TestDuplicateFieldsCauseException()
    {
      var mutableParameters = new MockGetItemsByPathParameters();
      mutableParameters.SessionSettings = this.sessionConfig;
      mutableParameters.ItemSource = LegacyConstants.DefaultSource();
      mutableParameters.ItemPath = "/aaa/bbb/ccc/ddd";

      string[] fields = { "x", "y", "x" };
      IQueryParameters duplicatedFields = new QueryParameters(fields);
      mutableParameters.QueryParameters = duplicatedFields;


      IReadItemsByPathRequest parameters = mutableParameters;
      Assert.Throws<ArgumentException>(() => this.builder.GetUrlForRequest(parameters));
    }


    [Test]
    public void TestDuplicateFieldsWithDifferentCaseCauseException()
    {
      var mutableParameters = new MockGetItemsByPathParameters();
      mutableParameters.SessionSettings = this.sessionConfig;
      mutableParameters.ItemSource = LegacyConstants.DefaultSource();
      mutableParameters.ItemPath = "/aaa/bbb/ccc/ddd";


      string[] fields = { "x", "y", "X" };
      IQueryParameters duplicatedFields = new QueryParameters(fields);
      mutableParameters.QueryParameters = duplicatedFields;


      IReadItemsByPathRequest parameters = mutableParameters;
      Assert.Throws<ArgumentException>(() => this.builder.GetUrlForRequest(parameters));
    }
  }
}

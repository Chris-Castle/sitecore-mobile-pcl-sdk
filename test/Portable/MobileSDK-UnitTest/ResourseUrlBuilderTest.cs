﻿namespace Sitecore.MobileSdkUnitTest
{
  using System;
  using NUnit.Framework;
  using Sitecore.MobileSDK.API;
  using Sitecore.MobileSDK.API.MediaItem;
  using Sitecore.MobileSDK.API.Request.Parameters;
  using Sitecore.MobileSDK.Items;
  using Sitecore.MobileSDK.MockObjects;
  using Sitecore.MobileSDK.SessionSettings;
  using Sitecore.MobileSDK.UrlBuilder.MediaItem;
  using Sitecore.MobileSDK.UrlBuilder.Rest;
  using Sitecore.MobileSDK.UrlBuilder.SSC;

  [TestFixture]
  public class ResourseUrlBuilderTest
  {
    MediaItemUrlBuilder builder;
    RestServiceGrammar restGrammar;
    SSCUrlParameters webApiGrammar;

    [SetUp]
    public void SetUp()
    {
      this.webApiGrammar = SSCUrlParameters.ItemSSCV2UrlParameters();
      this.restGrammar = RestServiceGrammar.ItemSSCV2Grammar();

      SessionConfigPOD sessionConfig = new SessionConfigPOD();
      sessionConfig.InstanceUrl = "http://test.host";
      sessionConfig.MediaLibraryRoot = "/sitecore/media library";
      sessionConfig.DefaultMediaResourceExtension = "ashx";
      sessionConfig.MediaPrefix = "~/media";

      ISessionConfig sessionSettings = sessionConfig;
      IMediaLibrarySettings mediaSettings = sessionConfig;

      ItemSource itemSource = LegacyConstants.DefaultSource();
      this.builder = new MediaItemUrlBuilder(this.restGrammar, this.webApiGrammar, sessionSettings, mediaSettings, itemSource);
    }

    [TearDown]
    public void TearDown()
    {
      this.builder = null;
    }

    [Test]
    public void RequestBuilderTest()
    {
      IDownloadMediaOptions options = new MediaOptionsBuilder().Set
        .Width(100)
        .Height(200)
        .BackgroundColor("white")
        .DisplayAsThumbnail(true)
        .Build();

      var request = ItemSSCRequestBuilder.DownloadResourceRequestWithMediaPath("/sitecore/media library/1 dot png")
        .DownloadOptions(options)
        .Database("master")
        .Build();
      //      request.ItemSource.Database = "web";

      Assert.AreEqual("master", request.ItemSource.Database);
    }

    [Test]
    public void TestPathWithoutSlashAndMediaHookThrowsException()
    {
      TestDelegate action = () => this.builder.BuildUrlStringForPath("sitecore/media library/1.png", null);
      var exception = Assert.Throws<ArgumentException>(action);
      Assert.True(exception.Message.Contains("Should begin with '/' or '~'"));
    }

    [Test]
    public void TestNullPathException()
    {
      TestDelegate action = () => this.builder.BuildUrlStringForPath(null, null);
      var exception = Assert.Throws<ArgumentNullException>(action);
      Assert.True(exception.Message.Contains("MediaItemUrlBuilder.Path"));
    }

    [Test]
    public void AbsolutePathWithoutOptionsTest()
    {
      string result = this.builder.BuildUrlStringForPath("/sitecore/media library/2/1 dot png", null);
      const string Expected = "http://test.host/~/media/2/1%20dot%20png.ashx?sc_database=web&sc_language=en";

      Assert.AreEqual(Expected, result);
    }

    [Test]
    public void RelativePathTest()
    {
      string result = this.builder.BuildUrlStringForPath("/mediaXYZ/1 dot png", null);
      const string Expected = "http://test.host/~/media/mediaxyz/1%20dot%20png.ashx?sc_database=web&sc_language=en";

      Assert.AreEqual(Expected, result);
    }

    [Test]
    public void TestExtensionOnItemPathThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() =>
        this.builder.BuildUrlStringForPath("/mediaXYZ/1.png", null)
      );
    }

    [Test]
    public void TestDoubleExtensionOnMediaHookIsAllowed()
    {
      string result = this.builder.BuildUrlStringForPath("~/media/XYZ/1.png.ashx", null);
      string expected = "http://test.host/~/media/xyz/1.png.ashx?sc_database=web&sc_language=en";

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void TestIncorrectMediaHookCausesArgumentException()
    {
      Assert.Throws<ArgumentException>(() =>
        this.builder.BuildUrlStringForPath("~/mediaXYZ/1.png", null)
      );
    }

    [Test]
    public void TestCustomMediaHook()
    {
      SessionConfigPOD sessionConfig = new SessionConfigPOD();
      sessionConfig.InstanceUrl = "htTP://CUSTOM.hoST";
      sessionConfig.MediaLibraryRoot = "/SiteCore/Other Media Library";
      sessionConfig.DefaultMediaResourceExtension = "ZIP";
      sessionConfig.MediaPrefix = "~/MediaXyZ";

      ISessionConfig sessionSettings = sessionConfig;
      IMediaLibrarySettings mediaSettings = sessionConfig;


      ItemSource itemSource = LegacyConstants.DefaultSource();
      this.builder = new MediaItemUrlBuilder(this.restGrammar, this.webApiGrammar, sessionSettings, mediaSettings, itemSource);

      var customBuilder =
        new MediaItemUrlBuilder(
          this.restGrammar,
          this.webApiGrammar,
          sessionSettings,
          mediaSettings,
          itemSource);


      string result = customBuilder.BuildUrlStringForPath("~/mediaXYZ/1.png.ashx", null);
      string expected = "http://custom.host/~/mediaxyz/1.png.ashx?sc_database=web&sc_language=en";

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void PathContaignMediaHookTest()
    {
      string result = this.builder.BuildUrlStringForPath("~/media/1", null);
      const string Expected = "http://test.host/~/media/1.ashx?sc_database=web&sc_language=en";

      Assert.AreEqual(Expected, result);
    }

    [Test]
    public void PathContaignMediaHookAndExtensionTest()
    {
      string result = this.builder.BuildUrlStringForPath("~/media/1.ashx", null);
      const string Expected = "http://test.host/~/media/1.ashx?sc_database=web&sc_language=en";

      Assert.AreEqual(Expected, result);
    }

    [Test]
    public void PathProperlyEscapedTest()
    {
      string result = this.builder.BuildUrlStringForPath("~/media/Images/test image", null);
      const string Expected = "http://test.host/~/media/images/test%20image.ashx?sc_database=web&sc_language=en";

      Assert.AreEqual(Expected, result);
    }


    [Test]
    public void ResourceNameIsCaseInsensitiveTest()
    {
      string result = this.builder.BuildUrlStringForPath("~/media/Images/SoMe ImAGe", null);
      const string Expected = "http://test.host/~/media/images/some%20image.ashx?sc_database=web&sc_language=en";

      Assert.AreEqual(Expected, result);
    }

    [Test]
    public void EmptyDownloadOptionsTest()
    {
      var options = new DownloadMediaOptions();

      string result = this.builder.BuildUrlStringForPath("~/media/1", options);
      const string Expected = "http://test.host/~/media/1.ashx?sc_database=web&sc_language=en";

      Assert.AreEqual(Expected, result);
    }

    [Test]
    public void CorrectDownloadOptionsTest()
    {
      var options = new DownloadMediaOptions();
      options.SetWidth(100);
      options.SetBackgroundColor("white");
      options.SetDisplayAsThumbnail(true);

      string result = this.builder.BuildUrlStringForPath("~/media/1/2", options);
      const string Expected = "http://test.host/~/media/1/2.ashx?w=100&bc=white&thn=1&sc_database=web&sc_language=en";

      Assert.AreEqual(Expected, result);
    }

    [Test]
    public void CorrectDownloadOptionsWithNullValuesTest()
    {
      var options = new DownloadMediaOptions();
      options.SetWidth(100);
      options.SetBackgroundColor("white");
      options.SetBackgroundColor(null);

      options.SetDisplayAsThumbnail(true);
      options.SetDisplayAsThumbnail(false);

      string result = this.builder.BuildUrlStringForPath("~/media/1", options);
      const string expected = "http://test.host/~/media/1.ashx?w=100&thn=0&sc_database=web&sc_language=en";

      Assert.AreEqual(expected, result);
    }
    [Test]
    public void CorrectDownloadOptionsWithAllParamsTest()
    {
      var options = new DownloadMediaOptions();
      options.SetWidth(10);
      options.SetHeight(10);
      options.SetBackgroundColor("3F0000");
      options.SetAllowStrech(false);
      options.SetDisableMediaCache(false);
      options.SetDisplayAsThumbnail(true);
      options.SetMaxHeight(10);
      options.SetMaxWidth(10);
      options.SetScale(2.5f);

      string result = this.builder.BuildUrlStringForPath("~/media/1.png", options);
      const string expected = "http://test.host/~/media/1.png?w=10&h=10&mw=10&mh=10&bc=3f0000&dmc=0&as=0&sc=2.5&thn=1&sc_database=web&sc_language=en";

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void CorrectDownloadOptionsWithScaleAndMaxWidthTest()
    {
      var options = new DownloadMediaOptions();
      options.SetScale(3.0005f);
      options.SetMaxWidth(10);


      string result = this.builder.BuildUrlStringForPath("~/media/1", options);
      const string Expected = "http://test.host/~/media/1.ashx?mw=10&sc=3.0005&sc_database=web&sc_language=en";

      Assert.AreEqual(Expected, result);
    }

    [Test]
    public void DownloadOptionsWithNegativeHeightTest()
    {
      var options = new DownloadMediaOptions();

      TestDelegate action = () => options.SetHeight(-4);
      var exception = Assert.Throws<ArgumentException>(action);
      Assert.True(exception.Message.Contains("height must be > 0"));
    }

    [Test]
    public void DownloadOptionsWithZeroMaxWidthTest()
    {
      var options = new DownloadMediaOptions();

      TestDelegate action = () => options.SetMaxWidth(0);
      var exception = Assert.Throws<ArgumentException>(action);
      Assert.True(exception.Message.Contains("maxWidth must be > 0"));
    }

    [Test]
    public void DownloadOptionsWithInvalidScaleTest()
    {
      var options = new DownloadMediaOptions();

      TestDelegate action = () => options.SetScale(-0.00f);
      var exception = Assert.Throws<ArgumentException>(action);
      Assert.True(exception.Message.Contains("scale must be > 0"));
    }
  }
}


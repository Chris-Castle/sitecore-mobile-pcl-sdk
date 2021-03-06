﻿namespace Sitecore.MobileSDK.MockObjects
{
  using Sitecore.MobileSDK.API.Items;
  using Sitecore.MobileSDK.Items;

  public class MutableItemSource : ItemSource
  {
    public MutableItemSource(string database, string language, int? version = null) 
      : base(database, language, version)
    {
    }

    public void SetDatabase(string value)
    {
      this.Database = value;
    }

    public void SetLanguage(string value)
    {
      this.Language = value;
    }

    public void SetVersion(int? value)
    {
      this.VersionNumber = value;
    }

    public override IItemSource ShallowCopy()
    {
      ++this.copyInvocationCount;
      return base.ShallowCopy();
    }

    public int CopyInvocationCount 
    { 
      get 
      { 
        return this.copyInvocationCount;
      } 
    }
    private int copyInvocationCount = 0;
  }
}


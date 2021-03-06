namespace Sitecore.MobileSDK.API.Items
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// A set of items returned by the Item Web API service.
  /// 
  /// It is used by the following operations :
  /// * Reading items
  /// * Creating a new item
  /// * Updating fields of an existing item
  /// </summary>
  public class ScItemsResponse : IEnumerable<ISitecoreItem>
  {
    public ScItemsResponse(List<ISitecoreItem> items, int statusCode)
    {
      this.Items = items;
      this.StatusCode = statusCode;
    }

    #region Paging

    /// <summary>
    /// Returns items count received for a given page.
    /// 
    /// </summary>
    public int ResultCount { 
      get {
        if (this.Items == null) {
          return 0;
        }
        return this.Items.Count;
      }
    }
    #endregion Paging

    #region IEnumerable 
    /// <summary>
    ///     Returns an enumerator that iterates through the items list. 
    ///     Note : Required by the compiler to conform the non-generic IEnumerable interface.
    /// </summary>
    /// <returns>
    ///      <see cref="IEnumerator{T}"/> that can be used to iterate through the items.
    /// </returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.Items.GetEnumerator();
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the items list.
    ///     A generic version of the iterator.    
    /// 
    /// </summary>
    /// <returns>
    ///      <see cref="IEnumerator{T}"/> that can be used to iterate through the items.
    /// </returns>
    public IEnumerator<ISitecoreItem> GetEnumerator()
    {
      return this.Items.GetEnumerator();
    }

    /// <summary>
    ///     Gets the item that was received.
    /// </summary>
    /// <param name="index">The index of item.</param>
    ///
    /// <returns>
    ///     <see cref="ISitecoreItem"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"> index is less than 0 or index is equal to or greater than <see cref="List{T}.Count"/>.</exception>
    public ISitecoreItem this[int index]
    {
      get
      {
        return this.Items[index];
      }
    }

    private List<ISitecoreItem> Items
    {
      get;
      /*private*/
      set;
    }
    #endregion IEnumerable


    #region httpResponse 
    public int StatusCode {
      get;
      private set;
    }
    #endregion httpResponse
  }
}
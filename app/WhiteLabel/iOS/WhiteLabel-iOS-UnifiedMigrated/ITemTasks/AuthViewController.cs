﻿namespace WhiteLabeliOS
{
  using System;
  using System.Drawing;
  using System.Net.Http;

  using Foundation;
  using UIKit;
  using Sitecore.MobileSDK.API;
  using Sitecore.MobileSDK.PasswordProvider;

  public partial class AuthViewController : BaseTaskViewController
  {
    public AuthViewController (IntPtr handle) : base (handle)
    {
      Title = NSBundle.MainBundle.LocalizedString("authTestVC", null);
    }

    public override void ViewDidLoad()
    {
      base.ViewDidLoad();

      this.loginField.ShouldReturn = this.HideKeyboard;
      this.passwordField.ShouldReturn = this.HideKeyboard;

      //TODO: remove later, default values
      this.urlField.Text = "https://cms80u2.test24dk1.dk.sitecore.net/";
      this.loginField.Text = "admin";
      this.passwordField.Text = "b";
      this.siteField.Text = "sitecore";

      this.loginField.Placeholder = NSBundle.MainBundle.LocalizedString("user_login_placeholder", null);
      this.passwordField.Placeholder = NSBundle.MainBundle.LocalizedString("password_placeholder", null);
      this.urlField.Placeholder = NSBundle.MainBundle.LocalizedString("Instance Url", null);
      this.siteField.Placeholder = NSBundle.MainBundle.LocalizedString("Site", null);

      string authButtonTitle = NSBundle.MainBundle.LocalizedString("authenticate_button_title", null);
      authButton.SetTitle(authButtonTitle, UIControlState.Normal);
    }

    partial void OnAuthButtonTapped(UIKit.UIButton sender)
    {
      this.SendAuthRequest();
    }

    private async void SendAuthRequest()
    {
      try
      {
        using (var credentials = new ScUnsecuredCredentialsProvider(this.loginField.Text, this.passwordField.Text, this.siteField.Text))
        using 
        ( 
            var session = SitecoreSSCSessionBuilder.AuthenticatedSessionWithHost(this.urlField.Text)
            .Credentials(credentials)
            .BuildReadonlySession()
        )
        {
          this.ShowLoader();

          var response = await session.AuthenticateAsync();

          if (response.IsSuccessful)
          {
            AlertHelper.ShowLocalizedAlertWithOkOption("Message", "This user exist");
          }
          else
          {
            AlertHelper.ShowLocalizedAlertWithOkOption("Message", "This user not exist");
          }
        }
      }
      catch(Exception e) 
      {
        AlertHelper.ShowLocalizedAlertWithOkOption("Error", e.Message);
      }
      finally
      {
        BeginInvokeOnMainThread(delegate
        {
          this.HideLoader();
        });
      }
    }
  }
}


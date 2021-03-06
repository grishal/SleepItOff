﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;

namespace SleepItOff.Models
{
    public sealed partial class AuthPage : Page
    {
        private LiveIdCredentials creds = new LiveIdCredentials();

        private const string client_id = "05c0fa93-a596-4484-a671-98a5e35db4ee";
        private const string scope = "mshealth.ReadDevices mshealth.ReadActivityHistory mshealth.ReadActivityLocation mshealth.ReadDevices offline_access";
        private const string redirect_uri = "https://login.microsoftonline.com/common/oauth2/nativeclient";
        public string response_code = ""; //getting after first GET
        private const string BaseHealthUri = "https://api.microsofthealth.net/v1/me/";


        public async Task SigninButton_ClickAsync(object sender, EventArgs e)
        {
            //create URL to send
            UriBuilder uri = new UriBuilder("https://login.live.com/oauth20_authorize.srf");
            var query = new StringBuilder();
            query.AppendFormat("redirect_uri={0}", Uri.EscapeDataString(redirect_uri));
            query.AppendFormat("&client_id={0}", Uri.EscapeDataString(client_id));           
            query.AppendFormat("&scope={0}", Uri.EscapeDataString(scope));
            query.Append("&response_type=code");
            uri.Query = query.ToString();

            //sending URL and waiting for response code
            var httpClient = new HttpClient();                        
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri.Uri);            
            var httpResponse = await httpClient.SendAsync(httpRequestMessage);                    
            response_code = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            //todo extract "code" value from the response (maybe)

            await GettingTokken(sender, e); //after we have the code, trying to get the token

        }

        private async Task SignoutButton_ClickAsync(object sender, EventArgs e)
        {
            UriBuilder uri = new UriBuilder("https://login.live.com/oauth20_logout.srf");
            var query = new StringBuilder();
            query.AppendFormat("redirect_uri={0}", Uri.EscapeDataString(redirect_uri));
            query.AppendFormat("&client_id={0}", Uri.EscapeDataString(client_id));
            uri.Query = query.ToString();

            var httpClient = new HttpClient();           
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri.Uri);
            var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
        }


        private async Task GettingTokken(object sender, EventArgs e)
        {
            //create URL to send
            UriBuilder uri = new UriBuilder("https://login.live.com/oauth20_token.srf");
            var query = new StringBuilder();
            uri.Query = query.ToString();

            //creating POST URL+body in application/x-www-form-urlencoded format
            var httpClient = new HttpClient();
            httpClient.BaseAddress = uri.Uri;
            String urlParameters = "client_id=" + client_id + "&redirect_uri=" + redirect_uri + "&code=" + response_code + "&grant_type=authorization_code";
            StringContent bodyParam = new StringContent(urlParameters, Encoding.UTF8, "application/x-www-form-urlencoded");
            
            //waiting to response
            var response = await httpClient.PostAsync(uri.Uri, bodyParam);

            //converting the response to JSON format
            var stringResponse = response.Content.ReadAsStringAsync().Result;
            var jsonResponse = JObject.Parse(stringResponse);

            //reading JSON response and updating credentials
            creds.AccessToken = (string)jsonResponse["access_token"];
            creds.ExpiresIn = (long)jsonResponse["expires_in"];
            creds.RefreshToken = (string)jsonResponse["refresh_token"];
            string error = (string)jsonResponse["error"];
        }


        private async Task RefreshToken(object sender, EventArgs e)
        {
            //create URL to send
            UriBuilder uri = new UriBuilder("https://login.live.com/oauth20_token.srf");
            var query = new StringBuilder();
            query.AppendFormat("redirect_uri={0}", Uri.EscapeDataString(redirect_uri));
            query.AppendFormat("&client_id={0}", Uri.EscapeDataString(client_id));
            query.AppendFormat("&refresh_token={0}", Uri.EscapeDataString(creds.RefreshToken));
            query.Append("&grant_type=refresh_token");
            uri.Query = query.ToString();

            //sending URL and waiting for response JSON
            var httpClient = new HttpClient();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri.Uri);
            var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            var stringResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            //reading JSON response and updating credentials
            var jsonResponse = JObject.Parse(stringResponse);
            creds.AccessToken = (string)jsonResponse["access_token"];
            creds.ExpiresIn = (long)jsonResponse["expires_in"];
            creds.RefreshToken = (string)jsonResponse["refresh_token"];
            string error = (string)jsonResponse["error"];


        }


    }
}

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json;
using OidcClientDemo.WPF.Infrastructure;
using OidcClientDemo.WPF.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OidcClientDemo.WPF.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly HttpClient httpClient;
        private readonly string settingsFilePath;

        private CancellationTokenSource cancellationTokenSource;

        public MainWindowViewModel(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OidcClientDemoWpf.config");
            if (IsInDesignMode)
            {
                PrepareDesignData();
            }
            else
            {
                PrepareRuntimeData();
            }
        }

        private void PrepareRuntimeData()
        {
            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => this.PropertyChanged += h, h => this.PropertyChanged -= h)
                .Select(_ => new SavedSettings()
                {
                    Authority = this.Authority,
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    Scopes = this.Scopes,
                    ListenPort = this.ListenPort,
                    RedirectUrl = this.RedirectUrl,
                    ResponseType = this.ResponseType,
                })
                .Throttle(TimeSpan.FromSeconds(1))
                .Distinct(new SavedSettingsComparer())
                .Select(x => Unit.Default)
                .Subscribe(x => SaveToDefaultFile());

            if (!LoadFromDefaultFile()) LoadSampleData();
        }

        private bool LoadFromDefaultFile()
        {
            if (File.Exists(settingsFilePath))
            {
                var json = File.ReadAllText(settingsFilePath);
                return LoadFromString(json);
            }
            else return false;
        }

        private bool LoadFromString(string json)
        {
            try
            {
                var saved = JsonConvert.DeserializeObject<SavedSettings>(json);
                this.Authority = saved.Authority;
                this.ClientId = saved.ClientId;
                this.ClientSecret = saved.ClientSecret;
                this.Scopes = saved.Scopes;
                this.ListenPort = saved.ListenPort;
                this.RedirectUrl = saved.RedirectUrl;
                this.ResponseType = saved.ResponseType;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void PrepareDesignData()
        {
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            this.Authority = "http://myauthority.local";
            this.ClientId = "<CLIENT ID>";
            this.ClientSecret = "<CLIENT SECRET>";
            this.Scopes = "scope1 scope2 scope3 scope4";
            this.ListenPort = 8080;
            this.RedirectUrl = "http://localhost:8080/callback";
        }

        private void SaveToDefaultFile()
        {
            var json = SaveToString();
            File.WriteAllText(settingsFilePath, json);
        }

        private string SaveToString()
        {
            return JsonConvert.SerializeObject(new SavedSettings()
            {
                Authority = this.Authority,
                ClientId = this.ClientId,
                ClientSecret = this.ClientSecret,
                Scopes = this.Scopes,
                ListenPort = this.ListenPort,
                RedirectUrl = this.RedirectUrl,
                ResponseType = this.ResponseType,
            }, Formatting.Indented);
        }

        private string authority = string.Empty;
        public string Authority
        {
            get
            {
                return authority;
            }
            set
            {
                this.discoveryResponse = null;
                Set(() => this.Authority, ref this.authority, value);
            }
        }

        private string clientId = string.Empty;
        public string ClientId
        {
            get
            {
                return clientId;
            }
            set
            {
                Set(() => this.ClientId, ref this.clientId, value);
            }
        }

        private string clientSecret = string.Empty;
        public string ClientSecret
        {
            get
            {
                return clientSecret;
            }
            set
            {
                Set(() => this.ClientSecret, ref this.clientSecret, value);
            }
        }

        private string scopes = string.Empty;
        public string Scopes
        {
            get
            {
                return scopes;
            }
            set
            {
                Set(() => this.Scopes, ref this.scopes, value);
            }
        }

        private int listenPort = 9095;
        public int ListenPort
        {
            get
            {
                return listenPort;
            }
            set
            {
                Set(() => this.ListenPort, ref this.listenPort, value);
            }
        }

        private string redirectUrl = string.Empty;
        public string RedirectUrl
        {
            get
            {
                return redirectUrl;
            }
            set
            {
                Set(() => this.RedirectUrl, ref this.redirectUrl, value);
            }
        }

        private string authorizationCode = string.Empty;
        public string AuthorizationCode
        {
            get
            {
                return authorizationCode;
            }
            set
            {
                Set(() => this.AuthorizationCode, ref this.authorizationCode, value);
                ExchangeCodeForTokens.RaiseCanExecuteChanged();
            }
        }

        private string idToken = string.Empty;
        public string IdToken
        {
            get
            {
                return idToken;
            }
            private set
            {
                Set(() => this.IdToken, ref this.idToken, value);
            }
        }

        private string accessToken = string.Empty;
        public string AccessToken
        {
            get
            {
                return accessToken;
            }
            private set
            {
                Set(() => this.AccessToken, ref this.accessToken, value);
            }
        }

        private long expiresIn = 0;
        public long ExpiresIn
        {
            get
            {
                return expiresIn;
            }
            private set
            {
                Set(() => this.ExpiresIn, ref this.expiresIn, value);
            }
        }

        private string tokenType = null;
        public string TokenType
        {
            get
            {
                return tokenType;
            }
            private set
            {
                Set(() => this.TokenType, ref this.tokenType, value);
            }
        }

        private string refreshToken = string.Empty;
        public string RefreshToken
        {
            get
            {
                return refreshToken;
            }
            set
            {
                Set(() => this.RefreshToken, ref this.refreshToken, value);
                RefreshTheToken.RaiseCanExecuteChanged();
            }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            private set
            {
                Set(() => this.IsBusy, ref this.isBusy, value);
                StartCodeFlow.RaiseCanExecuteChanged();
                Abort.RaiseCanExecuteChanged();
                ExchangeCodeForTokens.RaiseCanExecuteChanged();
                RefreshTheToken.RaiseCanExecuteChanged();
            }
        }

        private string responseType = string.Empty;
        public string ResponseType
        {
            get
            {
                return responseType;
            }
            set
            {
                Set(() => this.ResponseType, ref this.responseType, value);
                StartCodeFlow.RaiseCanExecuteChanged();
            }
        }

        private RelayCommand cmdStartCodeFlow;
        public RelayCommand StartCodeFlow
        {
            get
            {
                if (cmdStartCodeFlow == null)
                    cmdStartCodeFlow = new RelayCommand(StartCodeFlowExecute, () => !IsBusy && !string.IsNullOrWhiteSpace(ResponseType));
                return cmdStartCodeFlow;
            }
        }
        public async void StartCodeFlowExecute()
        {
            await DirtyJob(async () =>
            {
                var disco = await GetAddressesAsync(cancellationTokenSource.Token);

                var authorizeUrl = new RequestUrl(disco.AuthorizeEndpoint).CreateAuthorizeUrl(
                    clientId: this.ClientId,
                    responseType: ResponseType,
                    scope: this.Scopes,
                    redirectUri: this.RedirectUrl
                );

                var taskListen = SingleRequestListener.HandleRequestAsync(this.RedirectUrl, async (ctx, ct) =>
                {
                    await ctx.Response.WriteAsync("Success.", ct);
                    var result = new AuthorizeResponse(ctx.Request.Uri.ToString());
                    return result;
                }, cancellationTokenSource.Token);

                using (var browser = Browser.Navigate(authorizeUrl))
                {
                    var cb = await taskListen;
                    this.AuthorizationCode = cb.Code;
                }
                MessengerInstance.Send(new AlertMessage()
                {
                    Title = "Information",
                    Message = "Authorization Code obtained successfully!",
                    Type = AlertMessage.AlertType.Information,
                });
            });
        }

        private RelayCommand cmdAbort;
        public RelayCommand Abort
        {
            get
            {
                if (cmdAbort == null)
                    cmdAbort = new RelayCommand(AbortExecute, () => IsBusy);
                return cmdAbort;
            }
        }
        public void AbortExecute()
        {
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        private RelayCommand cmdExchangeCodeForTokens;
        public RelayCommand ExchangeCodeForTokens
        {
            get
            {
                if (cmdExchangeCodeForTokens == null)
                    cmdExchangeCodeForTokens = new RelayCommand(ExchangeCodeForTokensExecute, () => !IsBusy && !string.IsNullOrWhiteSpace(AuthorizationCode));
                return cmdExchangeCodeForTokens;
            }
        }
        public async void ExchangeCodeForTokensExecute()
        {
            await DirtyJob(async () =>
            {
                var disco = await GetAddressesAsync(cancellationTokenSource.Token);
                var request = new AuthorizationCodeTokenRequest()
                {
                    Address = disco.TokenEndpoint,
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    RedirectUri = this.RedirectUrl,
                    Code = this.AuthorizationCode,
                };
                var tokens = await httpClient.RequestAuthorizationCodeTokenAsync(request, cancellationTokenSource.Token);
                if (tokens.IsError)
                {
                    this.IdToken = null;
                    this.AccessToken = null;
                    this.ExpiresIn = 0;
                    this.TokenType = null;
                    this.RefreshToken = null;
                    MessengerInstance.Send(new AlertMessage()
                    {
                        Title = "Error",
                        Message = tokens.ErrorDescription ?? tokens.Error,
                        Type = AlertMessage.AlertType.Error,
                    });
                }
                else
                {
                    this.IdToken = tokens.IdentityToken;
                    this.AccessToken = tokens.AccessToken;
                    this.ExpiresIn = tokens.ExpiresIn;
                    this.TokenType = tokens.TokenType;
                    this.RefreshToken = tokens.RefreshToken;
                    MessengerInstance.Send(new AlertMessage()
                    {
                        Title = "Information",
                        Message = "Authorization Code exchanged for tokens successfully!",
                        Type = AlertMessage.AlertType.Information,
                    });
                }
            });
        }

        private RelayCommand cmdRefreshTheToken;
        public RelayCommand RefreshTheToken
        {
            get
            {
                if (cmdRefreshTheToken == null)
                    cmdRefreshTheToken = new RelayCommand(RefreshTheTokenExecute, () => !IsBusy && !string.IsNullOrWhiteSpace(RefreshToken));
                return cmdRefreshTheToken;
            }
        }
        public async void RefreshTheTokenExecute()
        {
            await DirtyJob(async () =>
            {
                var disco = await GetAddressesAsync(cancellationTokenSource.Token);
                var request = new RefreshTokenRequest()
                {
                    Address = disco.TokenEndpoint,
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    Scope = this.scopes,
                    RefreshToken = this.RefreshToken
                };
                var tokens = await httpClient.RequestRefreshTokenAsync(request, cancellationTokenSource.Token);
                this.IdToken = tokens.IdentityToken;
                this.AccessToken = tokens.AccessToken;
                this.ExpiresIn = tokens.ExpiresIn;
                this.TokenType = tokens.TokenType;
                this.RefreshToken = tokens.RefreshToken;
                MessengerInstance.Send(new AlertMessage()
                {
                    Title = "Information",
                    Message = "Token refreshed successfully!",
                    Type = AlertMessage.AlertType.Information,
                });
            });
        }

        private RelayCommand cmdSaveAs;
        public RelayCommand SaveAs
        {
            get
            {
                if (cmdSaveAs == null)
                    cmdSaveAs = new RelayCommand(SaveAsExecute);
                return cmdSaveAs;
            }
        }
        public void SaveAsExecute()
        {
            MessengerInstance.Send(new SelectFileAndSaveMessage()
            {
                WriteAction = stream =>
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        var json = SaveToString();
                        sw.Write(json);
                    }
                },
            });
        }

        private RelayCommand cmdOpen;
        public RelayCommand Open
        {
            get
            {
                if (cmdOpen == null)
                    cmdOpen = new RelayCommand(OpenExecute);
                return cmdOpen;
            }
        }
        public void OpenExecute()
        {
            MessengerInstance.Send(new SelectFileAndOpenMessage()
            {
                ReadAction = stream =>
                {
                    using (var sr = new StreamReader(stream))
                    {
                        var json = sr.ReadToEnd();
                        LoadFromString(json);
                    }
                },
            });
        }

        private DiscoveryResponse discoveryResponse;
        public async Task<DiscoveryResponse> GetAddressesAsync(CancellationToken? cancellationToken)
        {
            if (this.discoveryResponse == null)
            {
                var request = new DiscoveryDocumentRequest()
                {
                    Address = Authority,
                };
                var result = await httpClient.GetDiscoveryDocumentAsync(request, cancellationToken ?? CancellationToken.None);
                this.discoveryResponse = result;
            }
            return this.discoveryResponse;
        }

        public async Task DirtyJob(Func<Task> action)
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            IsBusy = true;
            try
            {
                await action();
            }
            catch (TaskCanceledException)
            {
                MessengerInstance.Send(new AlertMessage()
                {
                    Title = "Warning",
                    Message = "Authorization Code obtained successfully!",
                    Type = AlertMessage.AlertType.Warning,
                });
            }
            catch (Exception ex)
            {
                MessengerInstance.Send(new AlertMessage()
                {
                    Title = "Error",
                    Message = $"An error has occurred: '{ex.Message}'.",
                    Type = AlertMessage.AlertType.Error,
                });
            }
            finally { IsBusy = false; }
        }

        private class SavedSettings
        {
            public string Authority { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string Scopes { get; set; }
            public int ListenPort { get; set; }
            public string RedirectUrl { get; set; }
            public string ResponseType { get; set; }
        }
        private class SavedSettingsComparer : IEqualityComparer<SavedSettings>
        {
            public bool Equals(SavedSettings x, SavedSettings y)
            {
                var result = x.Authority == y.Authority
                    && x.ClientId == y.ClientId
                    && x.ClientSecret == y.ClientSecret
                    && x.Scopes == y.Scopes
                    && x.ListenPort == y.ListenPort
                    && x.RedirectUrl == y.RedirectUrl;
                return result;
            }

            public int GetHashCode(SavedSettings obj)
            {
                var result = Tuple.Create(
                    obj.Authority,
                    obj.ClientId,
                    obj.ClientSecret,
                    obj.Scopes,
                    obj.ListenPort,
                    obj.RedirectUrl
                ).GetHashCode();
                return result;
            }
        }
    }
}
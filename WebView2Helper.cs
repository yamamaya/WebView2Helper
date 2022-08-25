using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.WinForms;
using System.Windows.Forms;

namespace OaktreeLab.Utils.WebView2Helper {
    public class WebView2Helper {
        public WebView2Helper( WebView2 webView2 ) {
            webview = webView2;
            OnInitialized += OnInitialized_default;
            OnLoad += OnLoad_default;
        }

        public void Initialize() {
            Initialize( null );
        }

        public async void Initialize( string? uri ) {
            initialPage = uri;
            webview.CoreWebView2InitializationCompleted += Webview_CoreWebView2InitializationCompleted;
            webview.NavigationCompleted += Webview_NavigationCompleted;
            await webview.EnsureCoreWebView2Async( null );
        }

        private string? initialPage;

        private void Webview_CoreWebView2InitializationCompleted( object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e ) {
            OnInitialized( webview, new EventArgs() );
        }

        private void Webview_NavigationCompleted( object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e ) {
            OnLoad( webview, new EventArgs() );
        }

        private WebView2 webview {
            get;
            set;
        }

        public event EventHandler OnInitialized;
        public event EventHandler OnLoad;

        private void OnInitialized_default( object? sender, EventArgs e ) {
            if ( initialPage != null ) {
                Navigate( initialPage );
            }
        }

        private void OnLoad_default( object? sender, EventArgs e ) {
        }

        public void Navigate( string url ) {
            webview.CoreWebView2.Navigate( url );
        }

        public string Uri {
            get {
                return webview.CoreWebView2.Source;
            }
            set {
                Navigate( value );
            }
        }

        public string Title {
            get {
                return webview.CoreWebView2.DocumentTitle;
            }
        }

        public void GoBack() {
            webview.CoreWebView2.GoBack();
        }

        public void GoForward() {
            webview.CoreWebView2.GoForward();
        }

        public void Reload() {
            webview.CoreWebView2.Reload();
        }

        public WVElement GetElementById( string id ) {
            id = ScriptBuilder.EscapeString( id );
            ScriptBuilder script = new ScriptBuilder( $@"document.getElementById( '{id}' )" );
            return new WVElement( this, script );
        }

        public WVElements GetElementsByTagName( string tagName ) {
            tagName = ScriptBuilder.EscapeString( tagName );
            ScriptBuilder script = new ScriptBuilder( $@"Array.prototype.slice.call( document.getElementsByTagName( '{tagName}' ) )" );
            return new WVElements( this, script );
        }

        public WVElements GetElementsByName( string name ) {
            name = ScriptBuilder.EscapeString( name );
            ScriptBuilder script = new ScriptBuilder( $@"Array.prototype.slice.call( document.getElementsByName( '{name}' ) )" );
            return new WVElements( this, script);
        }

        internal async Task<JsonElement?> RunScript( string script ) {
            string resultJSON = await webview.ExecuteScriptAsync( script );
            JsonDocument json = JsonDocument.Parse( resultJSON );
            if ( json.RootElement.GetProperty( "status" ).GetString() == "success" ) {
                if ( json.RootElement.EnumerateObject().Where( obj => obj.Name == "result" ).Count() > 0 ) {
                    return json.RootElement.GetProperty( "result" );
                } else {
                    return null;
                }
            } else {
                System.Diagnostics.Debug.Print( json.RootElement.GetProperty( "result" ).GetString() );
                System.Diagnostics.Debug.Print( script );
                throw new ScriptException( json.RootElement.GetProperty( "result" ).GetString() );
            }
        }

        public class ScriptException : Exception {
            public ScriptException() : base() {
            }

            public ScriptException( string? message ) : base( message ) {
            }

            public ScriptException( string? message, Exception? innerException ) : base( message, innerException ) {
            }
        }
    }
}
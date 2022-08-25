using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace OaktreeLab.Utils.WebView2Helper {
    public class WVElement {
        internal WVElement( WebView2Helper helper, ScriptBuilder script) {
            this.helper = helper;
            this._script = script;
        }

        private WebView2Helper helper {
            get;
            set;
        }

        private ScriptBuilder _script {
            get;
            set;
        }

        public async Task<string> GetValue() {
            string script = _script.AppendToTail( ".value" ).FinishScript();
            JsonElement? res = await helper.RunScript( script );
            return res.HasValue ? ( res.Value.GetString() ?? "" ) : "";
        }

        public async Task SetValue( string value ) {
            value = ScriptBuilder.EscapeString( value );
            string script = _script.AppendToTail( $".value = '{value}'" ).FinishScript();
            await helper.RunScript( script );
        }

        public async Task<string> Get( string key ) {
            string script = _script.AppendToTail( $".{key}" ).FinishScript();
            JsonElement? res = await helper.RunScript( script );
            return res.HasValue ? ( res.Value.GetString() ?? "" ) : "";
        }

        public async Task Set( string key, string value ) {
            value = ScriptBuilder.EscapeString( value );
            string script = _script.AppendToTail( $".{key} = '{value}'" ).FinishScript();
            await helper.RunScript( script );
        }

        public WVElements GetElementsByTagName( string tagName ) {
            tagName = ScriptBuilder.EscapeString( tagName );
            ScriptBuilder script = _script.Append( 
                "Array.prototype.slice.call( ",
                $".getElementsByTagName( '{tagName}' ) )" 
            );
            return new WVElements( helper, script );
        }

        public WVElement Parent() {
            ScriptBuilder script = _script.AppendToTail( ".parentElement" );
            return new WVElement( helper, script );
        }

        public WVElements Children() {
            ScriptBuilder script = _script.Append( "Array.prototype.slice.call( ", ".children )" );
            return new WVElements( helper, script );
        }

        public async Task SetChecked( bool check ) {
            string check_str = check ? "true" : "false";
            string script = _script.AppendToTail( $".checked = {check_str}" ).FinishScript();
            await helper.RunScript( script );
        }

        public async Task<bool> GetChecked() {
            string script = _script.AppendToTail( ".checked" ).FinishScript();
            JsonElement? res = await helper.RunScript( script );
            return res.HasValue ? res.Value.GetBoolean() : false;
        }

        public async Task<string> GetInnerText() {
            string script = _script.AppendToTail( ".innerText" ).FinishScript();
            JsonElement? res = await helper.RunScript( script );
            return res.HasValue ? res.Value.ToString() ?? "" : "";
        }

        public async Task<string[][]?> ReadTableContents() {
            string script = _script.Append(
                "Array.prototype.slice.call( ",
                ".getElementsByTagName( 'tr' ) ).map( row => Array.prototype.slice.call( row.children ).filter( obj => obj.tagName == 'TH' || obj.tagName == 'TD' ).map( obj => obj.innerText ) )"
            ).FinishScript();
            JsonElement? res = await helper.RunScript( script );
            if ( res.HasValue && res.Value.GetArrayLength() > 0 ) {
                return res.Value.EnumerateArray().Select( row => row.EnumerateArray().Select( cell => cell.GetString() ?? "" ).ToArray() ).ToArray();
            } else {
                return null;
            }
        }

        public async Task Click() {
            string script = _script.AppendToTail( ".click()" ).FinishScript();
            await helper.RunScript( script );
        }

        public async Task Submit() {
            string script = _script.AppendToTail( ".submit()" ).FinishScript();
            await helper.RunScript( script );
        }
    }
}

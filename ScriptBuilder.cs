using System;
using System.Collections.Generic;
using System.Text;

namespace OaktreeLab.Utils.WebView2Helper {
    internal class ScriptBuilder {
        public ScriptBuilder( string scriptFragment ) {
            _script = scriptFragment;
        }

        private string _script;

        public ScriptBuilder Append( string head, string tail ) {
            return new ScriptBuilder( head + _script + tail );
        }

        public ScriptBuilder AppendToTail( string tail ) {
            return new ScriptBuilder( _script + tail );
        }

        public ScriptBuilder AppendToHead( string head ) {
            return new ScriptBuilder( head + _script );
        }

        public string FinishScript() {
            return @"
                function __WebViewHelper2_script__() {
                    try {
                        var res = " + _script + @";
                        return { status: 'success', result: res };
                    } catch ( error ) {
                        return { status: 'error', result: error.message };
                    }
                }
                __WebViewHelper2_script__();
            ";
        }

        public static string EscapeString( string src ) {
            StringBuilder dest = new StringBuilder();
            bool escape = false;
            foreach ( char c in src ) {
                if ( escape ) {
                    dest.Append( c );
                    escape = false;
                } else if ( c == '\\' ) {
                    dest.Append( c );
                    escape = true;
                } else if ( c == '\'' ) {
                    dest.Append( @"\'" );
                } else if ( c == '\n' ) {
                    dest.Append( @"\n" );
                } else if ( c == '\t' ) {
                    dest.Append( @"\t" );
                } else {
                    dest.Append( c );
                }
            }
            return dest.ToString();
        }

        public static explicit operator ScriptBuilder( string script ) {
            return new ScriptBuilder( script );
        }

        public static explicit operator string( ScriptBuilder script ) {
            return script._script;
        }
    }
}

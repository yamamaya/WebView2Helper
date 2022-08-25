using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OaktreeLab.Utils.WebView2Helper {
    public class WVElements {
        internal WVElements( WebView2Helper helper, ScriptBuilder script ) {
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

        public WVElement ElementAt( int index ) {
            ScriptBuilder script = _script.AppendToTail( $".at( {index} )" );
            return new WVElement( helper, script );
        }

        public WVElement First() {
            ScriptBuilder script = _script.AppendToTail( ".at( 0 )" );
            return new WVElement( helper, script );
        }

        public WVElement Last() {
            ScriptBuilder script = _script.AppendToTail( ".slice( -1 ).at( 0 )" );
            return new WVElement( helper, script );
        }

        public async Task<uint> Count() {
            string script = _script.AppendToTail( ".length" ).FinishScript();
            JsonElement? res = await helper.RunScript( script );
            if ( res.HasValue ) {
                uint count;
                if ( res.Value.TryGetUInt32( out count ) ) {
                    return count;
                }
            }
            return 0;
        }

        private static readonly Dictionary<WVFilterCondition, string> FilterConditionDictionary = new Dictionary<WVFilterCondition, string>() {
            { WVFilterCondition.Equal,                "{0} == {1}" },
            { WVFilterCondition.NotEqual,             "{0} != {1}" },
            { WVFilterCondition.LessThan,             "{0} < {1}" },
            { WVFilterCondition.LessThanOrEqual,      "{0} <= {1}" },
            { WVFilterCondition.GreaterThan,          "{0} > {1}" },
            { WVFilterCondition.GreaterThanOrEqual,   "{0} >= {1}" },
            { WVFilterCondition.Includes,             "{0}.indexOf( {1} ) >= 0" },
            { WVFilterCondition.NotIncludes,          "{0}.indexOf( {1} ) < 0" }
        };

        public WVElements Filter( string attribute, string value ) {
            return Where( attribute, WVFilterCondition.Equal, value );
        }

        public WVElements Where( string attribute, WVFilterCondition condition, string value ) {
            value = ScriptBuilder.EscapeString( value );
            string cond = string.Format( FilterConditionDictionary[ condition ], $"obj.{attribute}", $"'{value}'" );
            ScriptBuilder script = _script.AppendToTail( $".filter( obj => {cond} )" );
            return new WVElements( helper, script );
        }

        public async Task SetValue( string value ) {
            value = ScriptBuilder.EscapeString( value );
            string script = _script.AppendToTail( $".forEach( obj => obj.value = '{value}' )" ).FinishScript();
            await helper.RunScript( script );
        }

        public async Task Set( string key, string value ) {
            value = ScriptBuilder.EscapeString( value );
            string script = _script.AppendToTail( $".forEach( obj => obj.{key} = '{value}' )" ).FinishScript();
            await helper.RunScript( script );
        }
    }

    public enum WVFilterCondition {
        Equal,
        NotEqual,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
        Includes,
        NotIncludes
    }
}

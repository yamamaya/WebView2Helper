using OaktreeLab.Utils.WebView2Helper;
using static System.Net.Mime.MediaTypeNames;

namespace WebView2HelperSample {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();

            panel1.Enabled = false;

            webview = new WebView2Helper( webView );
            webview.OnLoad += WebView_OnLoad;

            string currentDirectory = Environment.CurrentDirectory;
            Uri uri = new Uri( $"{currentDirectory}/sample1.html" );
            webview.Initialize( uri.AbsoluteUri );
        }

        private WebView2Helper webview;

        private string SampleText {
            get {
                return DateTime.Now.ToString();
            }
        }

        private void WebView_OnLoad( object? sender, EventArgs e ) {
            panel1.Enabled = true;
        }

        // Set to text1
        private async void button1_Click( object sender, EventArgs e ) {
            await webview.GetElementById( "text1" ).SetValue( SampleText );
        }

        // Read from text1
        private async void button2_Click( object sender, EventArgs e ) {
            string text = await webview.GetElementById( "text1" ).GetValue();
            MessageBox.Show( $"text1='{text}'" );
        }

        // Refer not-existing element
        private async void button3_Click( object sender, EventArgs e ) {
            try {
                await webview.GetElementById( "not_exists" ).SetValue( "TEST" );
            } catch ( Exception ex ) {
                MessageBox.Show( ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        // Set to second input
        private async void button4_Click( object sender, EventArgs e ) {
            await webview.GetElementsByTagName( "input" ).ElementAt( 1 ).SetValue( SampleText );
        }

        // Set to blue input
        private async void button5_Click( object sender, EventArgs e ) {
            await webview.GetElementsByTagName( "input" ).Where( "style.backgroundColor", WVFilterCondition.Includes, "blue" ).First().SetValue( SampleText );
        }

        // Set to all text
        private async void button6_Click( object sender, EventArgs e ) {
            await webview.GetElementsByTagName( "input" ).Where( "type", WVFilterCondition.Equal, "text" ).SetValue( SampleText );
        }

        // Show/hide text1
        private async void button7_Click( object sender, EventArgs e ) {
            string visibility = await webview.GetElementById( "text1" ).Get( "style.visibility" );
            visibility = visibility == "hidden" ? "visible" : "hidden";
            await webview.GetElementById( "text1" ).Set( "style.visibility", visibility );
        }

        // Set to select1
        private async void button8_Click( object sender, EventArgs e ) {
            string value = await webview.GetElementById( "select1" ).GetValue();
            value = value == "1" ? "2" : "1";
            await webview.GetElementById( "select1" ).SetValue( value );
        }

        // Toggle check1
        private async void button9_Click( object sender, EventArgs e ) {
            bool check = await webview.GetElementById( "check1" ).GetChecked();
            check = !check;
            await webview.GetElementById( "check1" ).SetChecked( check );
        }

        // Count rows in table1
        private async void button10_Click( object sender, EventArgs e ) {
            uint count = await webview.GetElementById( "table1" ).GetElementsByTagName( "tr" ).Count();
            MessageBox.Show( $"count={count}" );
        }

        // Read table1
        private async void button11_Click( object sender, EventArgs e ) {
            var table = await webview.GetElementById( "table1" ).ReadTableContents();
            if ( table != null ) {
                MessageBox.Show( $"table1[1][2]={table[ 1 ][ 2 ]}" );
            }
        }

        // Click on button1
        private async void button12_Click( object sender, EventArgs e ) {
            await webview.GetElementById( "button1" ).Click();
        }

        // Click on link1
        private async void button13_Click( object sender, EventArgs e ) {
            await webview.GetElementById( "link1" ).Click();
        }

        // Set to textarea1
        private async void button14_Click( object sender, EventArgs e ) {
            await webview.GetElementById( "textarea1" ).SetValue( $"textarea1\nmulti-line test\n{SampleText}" );
        }
    }
}
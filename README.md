# WebView2Helper

This library provides **easy access for WebView2**. You don't need to write any JavaScript yourself.

Currently supports WinForms only, but will support WPF in the future.

## How to use

For example, to set value to an element;

```
helper.GetElementById( "text1" ).SetValue( "foobar" );
```

Get value from an element;

```
helper.GetElementById( "text1" ).GetValue();
```

Access to an element via DOM using method chain;

```
helper.GetElementsByTagName( "div" ).ElementAt( 1 ).Children().ElementAt( 2 ).GetValue();
```

Click on a button;

```
helper.GetElementById( "button1" ).Click();
```

## Document

Refer the sample project included.

## How it works?

This library generates JavaScript from method chain and inject it to WebView2. For example;

```
helper.GetElementsByTagName( "input" ).ElementAt( 1 ).SetValue( "foobar" )
        ⬇
Array.prototype.slice.call( document.getElementsByTagName( 'input' ) ).at( 1 ).value = 'foobar';
```
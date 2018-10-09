Blazor Interop Code generator
===============================

How to hack

    npm install
    build.cmd
    generate-csharp.cmd

Now generated code in the `Blazor.WebApiInterop/Generated` folder


Manually create file `Blazor.WebApiInterop/Generated/Content/vibration.js` with content below.

```js
class BlazorNavigatorProxy {
    static vibrate(pattern) {
        return navigator.vibrate(pattern);
    }
}

window["BlazorNavigatorProxy"] = BlazorNavigatorProxy;
```

Run `Blazor.WebApiInterop.Samples` project from VS, and go to the `/vibration` page.
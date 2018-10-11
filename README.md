Blazor Interop Code generator
===============================

Make sure that close this report recursively.

How to hack

```bash
git clone --depth=1 https://github.com/Microsoft/TSJS-lib-generator.git ./generator
npm install
build.cmd
generate-csharp.cmd
```

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
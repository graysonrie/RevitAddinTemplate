### Revit Add In Template (Only tested for Revit 2024)

The **App** class provides a starter if you need to create your own tab ribbon with custom panel buttons.

The **Command** class provides an outline for a custom command.

#### Using WPF Windows:

It is very important that you call `RevitContext.Setup();` in your `OnStartup` method for the app or the command, otherwise, WPF will not be able to call Revit events.
To call Revit events in WPF, use:

```c#
RevitContext.Events.Execute(uiapp =>
{
    // Your code here
});
```

Working on adding support for Next.js so that you will not have to work with crappy WPF. To make this work:
In the C# project, ensure **EmbedIO** and **Microsoft.Web.Webview2** are installed.
Use `yarn create next-app` to create the web app.
Ensure your `next.config.ts` file looks like this:

```typescript
import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  output: "export",
  assetPrefix: "./", // Use relative path
  trailingSlash: true,
  images: {
    unoptimized: true,
  },
  webpack: (config, { isServer }) => {
    if (!isServer) {
      config.output = config.output || {};
      config.output.publicPath = "./_next/";
    }
    return config;
  },
};

export default nextConfig;
```

When you are ready to export, use `yarn build` to generate static files (should be inside the `out` directory.)
In order for Revit to be able to use the web assets, you should copy the `out` directory and **paste it into the same directory as your addin DLL file and rename it `web` instead of `out`**.

WPF will spawn a lightweight web server in order to serve the files.

### Project References:

Ensure that you include `RevitAPI.dll` and `RevitAPIUI.dll` in your project references. These files are located in the Revit installation directory, typically found at:
`C:\Program Files\Autodesk\Revit 2024`

#### Hot Reloading:

It is highly recommended to install the Revit Addin Manager here:
https://github.com/chuongmep/RevitAddInManager/releases/tag/1.5.7

### Distributing

You will need to create an `.addin` file with the following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
	<AddIn Type="Application">
		<Name>Template Plugin</Name>
		<Assembly>RealRevitPlugin\RealRevitPlugin.dll</Assembly>
		<AddInId>83A1D773-7A33-4F6A-97A8-DB6E08AB1F0F</AddInId>
		<FullClassName>RealRevitPlugin.App</FullClassName>
		<VendorId>Development</VendorId>
		<VendorDescription>Insert description here</VendorDescription>
		<VendorEmail>youremail@example.com</VendorEmail>
	</AddIn>
</RevitAddIns>
```

Note that `FullClassName` is a combination of the namespace and the entry point class for your app.
Use `Tools > Create Guid` in Visual Studio to create a new GUID for the `AddInId` field.

To hook up the addin to Revit, you will need to place the `.addin` file inside of `AppData\Roaming\Autodesk\Revit\Addins\2024` for local use. In that same folder, you need to create another folder that is the name of your addin, and inside that folder, you will need to place the DLL.

Example: Addin name is 'MyAddin', then the folder structure will look like this:

```
C:\Users\<username>\AppData\Roaming\Autodesk\Revit\Addins\2024
    ├── MyAddin.addin
    └── MyAddin/MyAddin.dll
```

# Tracy
The Anime Tracing System.

# How to start
Follow the steps below to get started.

## Check out code
Currently Tracy is not managed through NuGet. Since Tracy project depends on other open source projects, you need to check out all of them into a single folder, so that Visual Studio can resolve dependencies.
* This project - https://github.com/wrwcmaster/Tracy.git
* [Gaia.Common](https://github.com/wrwcmaster/Gaia.Common) - https://github.com/wrwcmaster/Gaia.Common.git
* [BaiduPanAPI](https://github.com/wrwcmaster/BaiduPanAPI) - https://github.com/wrwcmaster/BaiduPanAPI.git
* [ThunderOfflineDownloadAPI](https://github.com/wrwcmaster/ThunderOfflineDownloadAPI) - https://github.com/wrwcmaster/ThunderOfflineDownloadAPI.git

## Prepare environment
In order to launch Tracy on your server, you need to prepare the following things:
* .Net 4.0 runtime. Tracy supports mono as well, so you can deploy it on a Linux server.
* Mongodb (2.6 or later)
* Nodejs (0.10 or later)

## Build & Launch
Use Visual Studio or Mono to build TracyServerPlugin.csproj

Execute _ConsolePluginLoader.exe TracyServerPlugin.dll_ to launch Tracy Service. (http://localhost:8801)

Note: You may need to build ConsolePluginLoader.csproj first to get the .exe file.

Execute _node WebUI/bin/www_ to start Web UI. (http://localhost:3000)

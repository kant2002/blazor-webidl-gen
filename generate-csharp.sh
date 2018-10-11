#!/usr/bin/env bash

outputpath=Blazor.WebApiInterop/Generated

mkdir -p $outputpath
dotnet build BlazorInteropGenerator/BlazorInteropGenerator.csproj
dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Storage.json" -o $outputpath/Storage.cs
dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Page Visibility.json" -o $outputpath/PageVisibility.cs
dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Web Messaging.json" -o $outputpath/WebMessaging.cs
dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Web Speech API.json" -o $outputpath/WebSpeech.cs
dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Screen Orientation.json" -o $outputpath/ScreenOrientation.cs

# Relied VibratePattern workaround
dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Vibration.json" -o $outputpath/Vibration.cs
dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Notifications.json" -o $outputpath/Notifications.cs

# 7 compilation errors
dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Performance Timeline.json" -o $outputpath/PerformanceTimeline.cs

# These files produce exceptions
dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Push.json" -o $outputpath/Push.cs
dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Indexed Database.json" -o $outputpath/IndexedDatabase.cs
dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Navigation Timing.json" -o $outputpath/NavigationTiming.cs
dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Service Workers.json" -o $outputpath/ServiceWorkers.cs

echo Completed!
@echo off
if not exist generated mkdir generated
SET outputpath=generated
cmd /c dotnet build BlazorInteropGenerator/BlazorInteropGenerator.csproj
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Vibration.json" -o %outputpath%/Vibration.cs
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Storage.json" -o %outputpath%/Storage.cs
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Page Visibility.json" -o %outputpath%/PageVisibility.cs
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Web Messaging.json" -o %outputpath%/WebMessaging.cs
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Web Speech API.json" -o %outputpath%/WebSpeech.cs
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Screen Orientation.json" -o %outputpath%/ScreenOrientation.cs

REM 8 compilation errors
REM cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Performance Timeline.json" -o %outputpath%/PerformanceTimeline.cs
REM 4 compilation errors
REM cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Notifications.json" -o %outputpath%/Notifications.cs
REM 7 compilation errors
REM cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Push.json" -o %outputpath%/Push.cs

REM These files produce exceptions
REM cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Indexed Database.json" -o %outputpath%/IndexedDatabase.cs
REM cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Navigation Timing.json" -o %outputpath%/NavigationTiming.cs
REM cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Service Workers.json" -o %outputpath%/ServiceWorkers.cs

echo Completed!
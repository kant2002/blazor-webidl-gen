@echo off
if not exist generated mkdir generated
cmd /c dotnet build BlazorInteropGenerator/BlazorInteropGenerator.csproj
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Vibration.json"
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Storage.json"
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Indexed Database.json"
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Navigation Timing.json"
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Notifications.json"
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Page Visibility.json"
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Performance Timeline.json"
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Web Messaging.json"
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Web Speech API.json"
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Push.json"
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Screen Orientation.json"
cmd /c dotnet run --no-build --no-launch-profile -p BlazorInteropGenerator -- -n BlazorExtensions -s "json/Service Workers.json"

echo Completed!
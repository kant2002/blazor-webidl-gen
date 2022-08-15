@echo off
cd Blazor.InteropGenerator.JS
set json_dir=../json
git submodule update --init
set idl_location=../generator/inputfiles/idl
set js_location=../Blazor.WebApiInterop/wwwroot
if not exist "%json_dir%" mkdir "%json_dir%"
if not exist "%js_location%" mkdir "%js_location%"

cmd /c npm run compile
cmd /c npm run process -- "%idl_location%/Vibration.widl" "%json_dir%/Vibration.json" "%js_location%/Vibration.js"
cmd /c npm run process -- "%idl_location%/Storage.widl" "%json_dir%/Storage.json" "%js_location%/Storage.js"
cmd /c npm run process -- "%idl_location%/Indexed Database.widl" "%json_dir%/Indexed Database.json" "%js_location%/IndexedDatabase.js"
cmd /c npm run process -- "%idl_location%/Navigation Timing.widl" "%json_dir%/Navigation Timing.json" "%js_location%/NavigationTiming.js"
cmd /c npm run process -- "%idl_location%/Notifications.widl" "%json_dir%/Notifications.json" "%js_location%/Notifications.js"
cmd /c npm run process -- "%idl_location%/Page Visibility.widl" "%json_dir%/Page Visibility.json" "%js_location%/PageVisibility.js"
cmd /c npm run process -- "%idl_location%/Performance Timeline.widl" "%json_dir%/Performance Timeline.json" "%js_location%/PerformanceTimeline.js"
cmd /c npm run process -- "%idl_location%/Web Messaging.widl" "%json_dir%/Web Messaging.json" "%js_location%/WebMessaging.js"
cmd /c npm run process -- "%idl_location%/Web Speech API.widl" "%json_dir%/Web Speech API.json" "%js_location%/WebSpeechAPI.js"
cmd /c npm run process -- "%idl_location%/Push.widl" "%json_dir%/Push.json" "%js_location%/Push.js"
cmd /c npm run process -- "%idl_location%/Screen Orientation.widl" "%json_dir%/Screen Orientation.json" "%js_location%/ScreenOrientation.js"
cmd /c npm run process -- "%idl_location%/Service Workers.widl" "%json_dir%/Service Workers.json" "%js_location%/ServiceWorkers.js"
cd ..

echo Completed!
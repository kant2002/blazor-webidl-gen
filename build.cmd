@echo off
cd Blazor.InteropGenerator.JS
set json_dir=../json
set idl_location=../generator/inputfiles/idl
if not exist "%json_dir%" mkdir "%json_dir%"

cmd /c npm run compile
cmd /c npm run process -- "%idl_location%/Vibration.widl" "%json_dir%/Vibration.json"
cmd /c npm run process -- "%idl_location%/Storage.widl" "%json_dir%/Storage.json"
cmd /c npm run process -- "%idl_location%/Indexed Database.widl" "%json_dir%/Indexed Database.json"
cmd /c npm run process -- "%idl_location%/Navigation Timing.widl" "%json_dir%/Navigation Timing.json"
cmd /c npm run process -- "%idl_location%/Notifications.widl" "%json_dir%/Notifications.json"
cmd /c npm run process -- "%idl_location%/Page Visibility.widl" "%json_dir%/Page Visibility.json"
cmd /c npm run process -- "%idl_location%/Performance Timeline.widl" "%json_dir%/Performance Timeline.json"
cmd /c npm run process -- "%idl_location%/Web Messaging.widl" "%json_dir%/Web Messaging.json"
cmd /c npm run process -- "%idl_location%/Web Speech API.widl" "%json_dir%/Web Speech API.json"
cmd /c npm run process -- "%idl_location%/Push.widl" "%json_dir%/Push.json"
cmd /c npm run process -- "%idl_location%/Screen Orientation.widl" "%json_dir%/Screen Orientation.json"
cmd /c npm run process -- "%idl_location%/Service Workers.widl" "%json_dir%/Service Workers.json"
cd ..

echo Completed!
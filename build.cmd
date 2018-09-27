@echo off
if not exist json mkdir json
cmd /c npm run compile
cmd /c npm run process -- "generator/inputfiles/idl/Vibration.widl" "json/Vibration.json"
cmd /c npm run process -- "generator/inputfiles/idl/Storage.widl" "json/Storage.json"
cmd /c npm run process -- "generator/inputfiles/idl/Indexed Database.widl" "json/Indexed Database.json"
cmd /c npm run process -- "generator/inputfiles/idl/Navigation Timing.widl" "json/Navigation Timing.json"
cmd /c npm run process -- "generator/inputfiles/idl/Notifications.widl" "json/Notifications.json"
cmd /c npm run process -- "generator/inputfiles/idl/Page Visibility.widl" "json/Page Visibility.json"
cmd /c npm run process -- "generator/inputfiles/idl/Performance Timeline.widl" "json/Performance Timeline.json"
cmd /c npm run process -- "generator/inputfiles/idl/Web Messaging.widl" "json/Web Messaging.json"
cmd /c npm run process -- "generator/inputfiles/idl/Web Speech API.widl" "json/Web Speech API.json"
cmd /c npm run process -- "generator/inputfiles/idl/Push.widl" "json/Push.json"
cmd /c npm run process -- "generator/inputfiles/idl/Screen Orientation.widl" "json/Screen Orientation.json"
cmd /c npm run process -- "generator/inputfiles/idl/Service Workers.widl" "json/Service Workers.json"

echo Completed!
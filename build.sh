#!/usr/bin/env bash

cd Blazor.InteropGenerator.JS
json_dir=../json
git submodule update --init
idl_location=../generator/inputfiles/idl
js_location=../Blazor.WebApiInterop/wwwroot
mkdir -p $json_dir

npm install
npm run compile
npm run process -- "$idl_location/Vibration.widl" "$json_dir/Vibration.json" "$js_location/Vibration.js"
npm run process -- "$idl_location/Storage.widl" "$json_dir/Storage.json" "$js_location/Storage.js"
npm run process -- "$idl_location/Indexed Database.widl" "$json_dir/Indexed Database.json" "$js_location/IndexedDatabase.js"
npm run process -- "$idl_location/Navigation Timing.widl" "$json_dir/Navigation Timing.json" "$js_location/NavigationTiming.js"
npm run process -- "$idl_location/Notifications.widl" "$json_dir/Notifications.json" "$js_location/Notifications.js"
npm run process -- "$idl_location/Page Visibility.widl" "$json_dir/Page Visibility.json" "$js_location/PageVisibility.js"
npm run process -- "$idl_location/Performance Timeline.widl" "$json_dir/Performance Timeline.json" "$js_location/PerformanceTimeline.js"
npm run process -- "$idl_location/Web Messaging.widl" "$json_dir/Web Messaging.json" "$js_location/WebMessaging.js"
npm run process -- "$idl_location/Web Speech API.widl" "$json_dir/Web Speech API.json" "$js_location/WebSpeechAPI.js"
npm run process -- "$idl_location/Push.widl" "$json_dir/Push.json" "$js_location/Push.js"
npm run process -- "$idl_location/Screen Orientation.widl" "$json_dir/Screen Orientation.json" "$js_location/ScreenOrientation.js"
npm run process -- "$idl_location/Service Workers.widl" "$json_dir/Service Workers.json" "$js_location/ServiceWorkers.js"
cd ..

echo Completed!
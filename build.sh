#!/usr/bin/env bash

cd Blazor.InteropGenerator.JS
json_dir=../json
idl_location=../generator/inputfiles/idl
mkdir -p $json_dir

npm ci
npm run compile
npm run process -- "$idl_location/Vibration.widl" "$json_dir/Vibration.json"
npm run process -- "$idl_location/Storage.widl" "$json_dir/Storage.json"
npm run process -- "$idl_location/Indexed Database.widl" "$json_dir/Indexed Database.json"
npm run process -- "$idl_location/Navigation Timing.widl" "$json_dir/Navigation Timing.json"
npm run process -- "$idl_location/Notifications.widl" "$json_dir/Notifications.json"
npm run process -- "$idl_location/Page Visibility.widl" "$json_dir/Page Visibility.json"
npm run process -- "$idl_location/Performance Timeline.widl" "$json_dir/Performance Timeline.json"
npm run process -- "$idl_location/Web Messaging.widl" "$json_dir/Web Messaging.json"
npm run process -- "$idl_location/Web Speech API.widl" "$json_dir/Web Speech API.json"
npm run process -- "$idl_location/Push.widl" "$json_dir/Push.json"
npm run process -- "$idl_location/Screen Orientation.widl" "$json_dir/Screen Orientation.json"
npm run process -- "$idl_location/Service Workers.widl" "$json_dir/Service Workers.json"
cd ..

echo Completed!
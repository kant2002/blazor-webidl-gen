#!/usr/bin/env bash

mkdir -p json

npm run compile
npm run process -- "generator/inputfiles/idl/Vibration.widl" "json/Vibration.json"
npm run process -- "generator/inputfiles/idl/Storage.widl" "json/Storage.json"
npm run process -- "generator/inputfiles/idl/Indexed Database.widl" "json/Indexed Database.json"
npm run process -- "generator/inputfiles/idl/Navigation Timing.widl" "json/Navigation Timing.json"
npm run process -- "generator/inputfiles/idl/Notifications.widl" "json/Notifications.json"
npm run process -- "generator/inputfiles/idl/Page Visibility.widl" "json/Page Visibility.json"
npm run process -- "generator/inputfiles/idl/Performance Timeline.widl" "json/Performance Timeline.json"
npm run process -- "generator/inputfiles/idl/Web Messaging.widl" "json/Web Messaging.json"
npm run process -- "generator/inputfiles/idl/Web Speech API.widl" "json/Web Speech API.json"
npm run process -- "generator/inputfiles/idl/Push.widl" "json/Push.json"
npm run process -- "generator/inputfiles/idl/Screen Orientation.widl" "json/Screen Orientation.json"
npm run process -- "generator/inputfiles/idl/Service Workers.widl" "json/Service Workers.json"

echo Completed!
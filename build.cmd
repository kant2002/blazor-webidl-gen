@echo off
if not exist json mkdir json
cmd /c npm run compile
cmd /c npm run process -- "generator/inputfiles/idl/Page Visibility.widl" "json/Page Visibility.json"
cmd /c npm run process -- "generator/inputfiles/idl/Vibration.widl" "json/Vibration.json"

echo Completed!
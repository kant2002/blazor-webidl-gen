import webidl2 = require("webidl2");
import * as fs from "fs";
import * as path from "path";

const fileName = process.argv[2];
if (!fileName) {
    console.error("The file is not specified");
    process.exit(1);
}

if (path.extname(fileName) != ".widl") {
    console.error(`The file ${fileName} should have extension .widl`);
    process.exit(1);
}

const outputFileName = process.argv[3];
const content = fs.readFileSync(fileName);
const parsingStructure = webidl2.parse(content.toString());
fs.writeFileSync(outputFileName, JSON.stringify(parsingStructure, null, 2));

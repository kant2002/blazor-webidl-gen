import * as ts from "typescript";
import * as webidl2 from "webidl2";
import { OperationMemberType } from "webidl2";

const generateJsCode = false;

function createParameters(idl: webidl2.Argument) {
    const paramName = ts.createIdentifier(idl.name);
    const parameter = ts.createParameter(
        /*decorators*/ undefined,
        /*modifiers*/ undefined,
        /*dotDotDotToken*/ undefined,
        paramName,
        undefined,
        createTsType(idl.idlType)
    );
    return parameter;
}

function createTsType(idlType: webidl2.IDLTypeDescription) {
    if (generateJsCode) {
        return undefined;
    }

    if (idlType.generic === null) {
        return ts.createTypeReferenceNode(idlType.idlType as string, []);
    }

    const genericDefinition = idlType.generic as any;
    const genericTypeParameters = idlType.idlType as webidl2.IDLTypeDescription[];
    const returnType = ts.createTypeReferenceNode(
        genericDefinition.value,
        genericTypeParameters.map(createTsType));
    return returnType;
}

function createProxyBody(body: webidl2.OperationMemberType) {
    const functionName = ts.createIdentifier("n2");
    const paramName = ts.createIdentifier("n");
    const condition = ts.createBinary(
        paramName,
        ts.SyntaxKind.LessThanEqualsToken,
        ts.createLiteral(1)
    );

    const ifBody = ts.createBlock(
        [ts.createReturn(ts.createLiteral(1))],
    /*multiline*/ true
    );
    const decrementedArg = ts.createBinary(
        paramName,
        ts.SyntaxKind.MinusToken,
        ts.createLiteral(1)
    );
    const recurse = ts.createBinary(
        paramName,
        ts.SyntaxKind.AsteriskToken,
        ts.createCall(functionName, /*typeArgs*/ undefined, [decrementedArg])
    );
    const statements = [ts.createIf(condition, ifBody), ts.createReturn(recurse)];
    const methodBody = ts.createBlock(statements, /*multiline*/ true);
    return methodBody;
}

function createMember(idl: webidl2.OperationMemberType) {
    if (idl.name === null) {
        throw new Error("Name of the operation could not be null");
    }

    // console.log(idl);
    const paramName = ts.createIdentifier("n");
    const parameter = ts.createParameter(
        /*decorators*/ undefined,
        /*modifiers*/ undefined,
        /*dotDotDotToken*/ undefined,
        paramName
    );
    const body = (idl as any).body;
    const operationName: any = (idl as any).body.name.value;
    const functionName = ts.createIdentifier(operationName);
    const arugments = body.arguments.map(createParameters);
    const returnType = createTsType(body.idlType);
    return ts.createMethod(
        undefined,
        undefined,
        undefined,
        operationName,
        undefined,
        undefined,
        arugments,
        returnType,
        createProxyBody(body),
    );
}

function isOperationMember(idl: webidl2.IDLInterfaceMemberType): idl is OperationMemberType  {
    return idl.type == "operation";
}

function makeFactorialFunction(idl: webidl2.InterfaceType) {
    const proxyClassName = `Blazor${idl.name}Proxy`;
    const members = idl.members.filter(isOperationMember).map(createMember);
    return ts.createClassDeclaration(
        /*decorators*/ undefined,
        /*modifiers*/[ts.createToken(ts.SyntaxKind.ExportKeyword)],
        proxyClassName,
        undefined,
        undefined,
        members);
}

export function generateType(idl: webidl2.InterfaceType) {
    const resultFile = ts.createSourceFile(
        "someFileName.ts",
        "",
        ts.ScriptTarget.Latest,
        /*setParentNodes*/ false,
        ts.ScriptKind.TS
    );
    const printer = ts.createPrinter({
        newLine: ts.NewLineKind.LineFeed
    });
    const result = printer.printNode(
        ts.EmitHint.Unspecified,
        makeFactorialFunction(idl),
        resultFile
    );
    return result;
}
import * as ts from "typescript";
import * as webidl2 from "webidl2";
import { OperationMemberType } from "webidl2";

const generateJsCode = true;

function createParameters(idl: webidl2.Argument) {
    const paramName = ts.createIdentifier(idl.name);
    const parameter = ts.createParameter(
        /*decorators*/ undefined,
        /*modifiers*/ undefined,
        /*dotDotDotToken*/ undefined,
        paramName,
        undefined,
        generateJsCode ? undefined : createTsType(idl.idlType)
    );
    return parameter;
}

function createTsType(idlType: webidl2.IDLTypeDescription) {
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

function createProxyBody(idl: webidl2.OperationMemberType) {
    const body = <webidl2.OperationMemberType>(<any>idl).body;
    const functionName = ts.createIdentifier(getOperationName(idl));
    const mainObject = ts.createPropertyAccess(ts.createIdentifier("navigator"), functionName);
    const proxyCallArguments = body.arguments.map(_ => ts.createIdentifier(_.name));
    const returnValue = ts.createCall(mainObject, /*typeArgs*/ undefined, proxyCallArguments);
    const statements = [ts.createReturn(returnValue)];
    const methodBody = ts.createBlock(statements, /*multiline*/ true);
    return methodBody;
}

function getOperationName(idl: webidl2.OperationMemberType): string {
    const body = (idl as any).body;
    const operationName: any = body.name.value;
    return operationName;
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
    const operationName = getOperationName(idl);
    const functionName = ts.createIdentifier(operationName);
    const arugments = body.arguments.map(createParameters);
    const returnType = generateJsCode ? undefined : createTsType(body.idlType);
    const modifiers = [ts.createToken(ts.SyntaxKind.StaticKeyword)];
    return ts.createMethod(
        undefined,
        modifiers,
        undefined,
        operationName,
        undefined,
        undefined,
        arugments,
        returnType,
        createProxyBody(idl),
    );
}

function isOperationMember(idl: webidl2.IDLInterfaceMemberType): idl is OperationMemberType  {
    return idl.type == "operation";
}

function createProxyTypeDefinition(idl: webidl2.InterfaceType) {
    const proxyClassName = `Blazor${idl.name}Proxy`;
    const members = idl.members.filter(isOperationMember).map(createMember);
    const modifiers = [ts.createToken(ts.SyntaxKind.ExportKeyword)];
    return ts.createClassDeclaration(
        /*decorators*/ undefined,
        /*modifiers*/ undefined,
        proxyClassName,
        undefined,
        undefined,
        members);
}

function createProxyRegistration(idl: webidl2.InterfaceType) {
    const proxyClassName = `Blazor${idl.name}Proxy`;
    const windowObject = ts.createIdentifier("window");    
    const exportProxyExpression = ts.createAssignment(
        ts.createPropertyAccess(windowObject, proxyClassName),
        ts.createIdentifier(proxyClassName)
    );
    return ts.createExpressionStatement(exportProxyExpression);
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
        createProxyTypeDefinition(idl),
        resultFile
    );
    return result;
}

export function generateTypeRegistration(idl: webidl2.InterfaceType) {
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
        createProxyRegistration(idl),
        resultFile
    );
    return result;
}
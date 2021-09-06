module rec Fable.Transforms.Fable2Lua

//cloned from FableToBabel

open System
open System.Collections.Generic
open System.Text.RegularExpressions

open Fable
open Fable.AST
open Fable.AST.Lua
open Fable.Compilers.Lua
open Fable.Naming
open Fable.Core

// type ILuaCompiler =
//     inherit Compiler

// type LuaCompiler(com: Fable.Compiler) =
//     interface ILuaCompiler // with
        //member this.AddType(entref, Type: LuaType) = this.AddType(entref, phpType)
module Transforms =
    module Helpers =
        let transformStatements transformStatements transformReturn exprs = [
                match exprs |> List.rev with
                | h::t ->
                    for x in t |> List.rev do
                        yield transformStatements x
                    yield transformReturn h
                | [] -> ()
            ]
        let ident name = Ident {Name = name; Namespace = None}
        let iife statements = FunctionCall(AnonymousFunc([], statements), [])
        let maybeIife = function
            | [] -> NoOp
            | [Return expr] -> expr
            | statements -> iife statements
        let tryNewObj (names: string list) (values: Expr list) =
            if names.Length = values.Length then
                let pairs = List.zip names values
                NewObj(pairs)
            else sprintf "Names and values do not match %A %A" names values |> Unknown
    let transformValueKind (com: LuaCompiler) = function
        | Fable.NumberConstant(v,_,_) ->
            Const(ConstNumber v)
        | Fable.StringConstant(s) ->
            Const(ConstString s)
        | Fable.BoolConstant(b) ->
            Const(ConstBool b)
        | Fable.UnitConstant ->
            Const(ConstNull)
        | Fable.CharConstant(c) ->
            Const(ConstString (string c))
        // | Fable.EnumConstant(e,ref) ->
        //     convertExpr com e
        | Fable.NewRecord(values, ref, args) ->
            let entity = com.Com.GetEntity(ref)
            if entity.IsFSharpRecord then
                let names = entity.FSharpFields |> List.map(fun f -> f.Name)
                let values = values |> List.map (transformExpr com)
                Helpers.tryNewObj names values
            else sprintf "unknown ety %A %A %A %A" values ref args entity |> Unknown
        | Fable.NewAnonymousRecord(values, names, _) ->
            let transformedValues = values |> List.map (transformExpr com)
            Helpers.tryNewObj (Array.toList names) transformedValues
        | Fable.NewUnion(values, tag, _, _) ->
            let values = values |> List.map(transformExpr com) |> List.mapi(fun i x -> sprintf "p_%i" i, x)
            NewObj(("tag", tag |> float |> ConstNumber |> Const)::values)
        | Fable.NewOption (value, t, _) ->
            value |> Option.map (transformExpr com) |> Option.defaultValue (Const ConstNull)
        | Fable.NewTuple(values, isStruct) ->
            // let fields = values |> List.mapi(fun i x -> sprintf "p_%i" i, transformExpr com x)
            // NewObj(fields)
            NewArr(values |> List.map (transformExpr com))
        | Fable.NewArray(values, t) ->
            NewArr(values |> List.map (transformExpr com))
        | Fable.Null _ ->
            Const(ConstNull)
        | x -> sprintf "unknown %A" x |> ConstString |> Const
    let transformOp com =
        let transformExpr = transformExpr com
        function
        | Fable.OperationKind.Binary (op, left, right) ->
            let op = match op with
                | BinaryMultiply -> Multiply
                | BinaryDivide -> Divide
                | BinaryEqual -> Equals
                | BinaryPlus -> Plus
                | BinaryMinus -> Minus
                | BinaryEqualStrict -> Equals
                | BinaryUnequal -> Unequal
                | BinaryLess -> Less
                | BinaryGreater -> Greater
                | BinaryLessOrEqual -> LessOrEqual
                | BinaryGreaterOrEqual -> GreaterOrEqual
                | x -> sprintf "%A" x |> BinaryTodo
            Binary(op, transformExpr left, transformExpr right )
        | Fable.OperationKind.Unary (op, expr) ->
            match op with
            | UnaryNotBitwise -> transformExpr expr //not sure why this is being added
            | UnaryNot -> Unary(Not, transformExpr expr)
            | _ -> sprintf "%A %A" op expr |> Unknown
        | x -> Unknown(sprintf "%A" x)
    let asSingleExprIife (exprs: Expr list): Expr= //function
        match exprs with
        | [] -> NoOp
        | [h] ->
            h
        | exprs ->
            let statements =
                Helpers.transformStatements
                    (Do)
                    (Return)
                    exprs
            statements |> Helpers.maybeIife
    let flattenReturnIifes e =
        let rec collectStatementsRec =
            function
            | Return (FunctionCall(AnonymousFunc([], [Return s]), [])) ->
                [Return s]
            | Return (FunctionCall(AnonymousFunc([], statements), [])) -> //self executing functions only
                statements |> List.collect collectStatementsRec
            | x -> [x]
        let statements = collectStatementsRec e
        match statements with
        | [Return s] -> Return s
        | [] -> NoOp |> Do
        | _ -> FunctionCall(AnonymousFunc([], statements), []) |> Return

    let asSingleExprIifeTr com : Fable.Expr list -> Expr = List.map (transformExpr com) >> asSingleExprIife
    let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
        else None

    let transformExpr (com: LuaCompiler) expr=
        let transformExpr = transformExpr com
        let transformOp = transformOp com
        match expr with
        | Fable.Expr.Value(value, _) -> transformValueKind com value
        | Fable.Expr.Call(expr, callInfo, t, r) ->
            //Unknown(sprintf "call %A %A" expr callInfo)
            FunctionCall(transformExpr expr, List.map transformExpr callInfo.Args)
        | Fable.Expr.Import (info, t, r) ->
            let path =
                match info.Kind, info.Path with
                | LibraryImport, Regex "fable-lib\/(\w+).(?:fs|js)" [name] ->
                    "fable-lib/" + name
                | LibraryImport, Regex"fable-library-lua\/fable\/fable-library\/(\w+).(?:fs|js)" [name] ->
                    "fable-lib/fable-library" + name
                | LibraryImport, Regex"fable-library-lua\/fable\/(\w+).(?:fs|js)" [name] ->
                    "fable-lib/" + name
                | _ ->
                    info.Path.Replace(".fs", "").Replace(".js", "") //todo - make less brittle
            let rcall = FunctionCall(Ident { Namespace=None; Name= "require" }, [Const (ConstString path)])
            match info.Selector with
            | "" -> rcall
            | s -> GetField(rcall, s)
        | Fable.Expr.IdentExpr(i) when i.Name <> "" ->
            Ident {Namespace = None; Name = i.Name }
        | Fable.Expr.Operation (kind, _, _) ->
            transformOp kind
        | Fable.Expr.Get(expr, Fable.GetKind.FieldGet(fieldName, isMut), _, _) ->
            GetField(transformExpr expr, fieldName)
        | Fable.Expr.Get(expr, Fable.GetKind.UnionField(caseIdx, fieldIdx), _, _) ->
            GetField(transformExpr expr, sprintf "p_%i" fieldIdx)
        | Fable.Expr.Get(expr, Fable.GetKind.ExprGet(e), _, _) ->
            GetAtIndex(transformExpr expr, transformExpr e)
        | Fable.Expr.Set(expr, Fable.SetKind.ValueSet, t, value, _) ->
            SetValue(transformExpr expr, transformExpr value)
        | Fable.Expr.Set(expr, Fable.SetKind.ExprSet(e), t, value, _) ->
            SetExpr(transformExpr expr, transformExpr e, transformExpr value)
        | Fable.Expr.Sequential exprs ->
            asSingleExprIifeTr com exprs
        | Fable.Expr.Let (ident, value, body) ->
            let statements = [
                Assignment(ident.Name, transformExpr value)
                transformExpr body |> Return
            ]
            Helpers.maybeIife statements
        | Fable.Expr.Emit(m, _, _) ->
            // let argsExprs = m.CallInfo.Args |> List.map transformExpr
            // let macroExpr = Macro(m.Macro, argsExprs)
            // let exprs =
            //     argsExprs
            //     @ [macroExpr]
            // asSingleExprIife exprs
            Macro(m.Macro, m.CallInfo.Args |> List.map transformExpr)
        | Fable.Expr.DecisionTree(expr, lst) ->
            com.DecisionTreeTargets(lst)
            transformExpr expr
        | Fable.Expr.DecisionTreeSuccess(i, boundValues, _) ->
            let idents,target = com.GetDecisionTreeTargets(i)
            if idents.Length = boundValues.Length then
                let statements =
                    [   for (ident, value) in List.zip idents boundValues do
                            yield Assignment(ident.Name, transformExpr value)
                        yield transformExpr target |> Return
                            ]
                statements
                |> Helpers.maybeIife
            else sprintf "not equal lengths %A %A" idents boundValues |> Unknown
        | Fable.Expr.Lambda(arg, body, name) ->
            Function([arg.Name], [transformExpr body |> Return])
        | Fable.Expr.CurriedApply(applied, args, _, _) ->
            FunctionCall(transformExpr applied, args |> List.map transformExpr)
        | Fable.Expr.IfThenElse (guardExpr, thenExpr, elseExpr, _) ->
            Ternary(transformExpr guardExpr, transformExpr thenExpr, transformExpr elseExpr)
        | Fable.Test(expr, kind, b) ->
            match kind with
            | Fable.UnionCaseTest i->
                Binary(Equals, GetField(transformExpr expr, "tag") , Const (ConstNumber (float i)))
            | _ ->
                Unknown(sprintf "test %A %A" expr kind)
        | Fable.Extended(Fable.ExtendedSet.Throw(expr, _), t) ->
            let errorExpr =
                //Const (ConstString "There was an error")
                transformExpr expr
            FunctionCall(Helpers.ident "error", [errorExpr])
        | Fable.Delegate(idents, body, _) ->
            Function(idents |> List.map(fun i -> i.Name), [transformExpr body |> Return |> flattenReturnIifes]) //can be flattened
        | Fable.ForLoop(ident, start, limit, body, isUp, _) ->
            Helpers.maybeIife [
                ForLoop(ident.Name, transformExpr start, transformExpr limit, [transformExpr body |> Do])
                ]
        | Fable.TypeCast(expr, t) ->
            transformExpr expr //typecasts are meaningless
        | Fable.WhileLoop(guard, body, label, range) ->
            Helpers.maybeIife [
                WhileLoop(transformExpr guard, [transformExpr body |> Do])
            ]
        | x -> Unknown (sprintf "%A" x)

    let transformDeclarations (com: LuaCompiler) = function
        | Fable.ModuleDeclaration m ->
            Assignment("moduleDecTest", Expr.Const (ConstString "moduledectest"))
        | Fable.MemberDeclaration m ->
            if m.Args.Length = 0 then
                Assignment(m.Name, transformExpr com m.Body)
            else

                let unwrapSelfExStatements =
                    match transformExpr com m.Body |> Return |> flattenReturnIifes with
                    | Return (FunctionCall(AnonymousFunc([], statements), [])) ->
                        statements
                    | s -> [s]
                FunctionDeclaration(m.Name, m.Args |> List.map(fun a -> a.Name), unwrapSelfExStatements, m.Info.IsPublic)
        | Fable.ClassDeclaration(d) ->
            com.AddClassDecl d
            //todo - build prototype members out
            //SNoOp
            sprintf "ClassDeclaration %A" d |> Unknown |> Do
        | x -> sprintf "%A" x |> Unknown |> Do

let transformFile com (file: Fable.File): File =
    let comp = LuaCompiler(com)
    {
        Filename = "abc"
        Statements =  file.Declarations |> List.map (Transforms.transformDeclarations comp)
        ASTDebug = sprintf "%A" file.Declarations
    }
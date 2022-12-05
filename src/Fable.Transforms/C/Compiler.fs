module rec Fable.Compilers.C

open Fable.AST
open Fable.AST.Fable

type CCompiler(com: Fable.Compiler) =
    let mutable types = Map.empty
    let mutable decisionTreeTargets = []
    let mutable additionalDeclarations = []
    //member this.Com = com
    // member this.AddClassDecl (c: ClassDecl) =
    //     types <- types |> Map.add c.Entity c
    // member this.GetByRef (e: EntityRef) =
    //     types |> Map.tryFind e
    member this.DecisionTreeTargets (exprs: (list<Fable.Ident> * Expr) list) =
        decisionTreeTargets <- exprs
    member this.GetDecisionTreeTargets (idx: int) = decisionTreeTargets.[idx]
    member this.GetEntity entRef= com.TryGetEntity(entRef).Value
    member this.CreateAdditionalDeclaration (declaration: Declaration) =
        additionalDeclarations <- declaration::additionalDeclarations
    member this.GetAdditionalDeclarations() = additionalDeclarations
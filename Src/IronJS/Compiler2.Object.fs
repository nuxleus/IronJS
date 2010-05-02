﻿namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types

module Object =

  let properties expr = 
    Expr.field expr "Properties"

  let classId expr = 
    Expr.field expr "ClassId"

  let private buildSet ctx name target value =
    let cache, cacheId, cacheIndex, fetcher = Runtime.PropertyCache.Create(name)
    Wrap.wrapInBlock target (fun obj -> 
      [
        (Expr.debug (sprintf "Setting property '%s'" name))
        (Expr.ternary
          (Expr.eq (classId obj) cacheId)
          (Utils.Box.assign ctx (Expr.access (properties obj) [cacheIndex]) value.Et)
          (Expr.call cache "UpdateSet" [obj; Utils.Box.wrap value.Et; Context.environmentExpr ctx])
        )
      ]
    )

  let private buildGet ctx name (typ:ClrType option) target =
    let cache, cacheId, cacheIndex, fetcher = Runtime.PropertyCache.Create(name)
    Wrap.wrapInBlock target (fun obj ->
      [
        (Expr.debug (sprintf "Getting property '%s'" name))
        (Expr.ternary
          (Expr.eq (classId obj) cacheId)
          (Utils.Box.fieldIfClrType (Expr.access (properties obj) [cacheIndex]) typ)
          (Expr.ternary 
            (Expr.notDefault fetcher)
            (Utils.Box.fieldIfClrType (Expr.invoke fetcher [cache; obj]) typ)
            (Utils.Box.fieldIfClrType (Expr.call cache "UpdateGet" [obj; Context.environmentExpr ctx]) typ)
          )
        )
      ]
    )
    
  let unboundSet ctx name (target:Wrapped) (value:Wrapped) =
    if Runtime.Utils.Type.isObject target.Type 
      then buildSet ctx name target value
      else 
        if Runtime.Utils.Type.isBox target.Type then
          Wrap.wrapInBlock target (fun tmp -> 
            let target = (Wrap.static' (Utils.Box.fieldByClrTypeT<Runtime.Object> tmp))
            [
              (Expr.debug (sprintf "Type check for setting property '%s'" name))
              (Expr.ternary
                (Utils.Box.typeIsT<Runtime.Object> tmp)
                (buildSet ctx name target value).Et
                (Expr.void')
              )
            ]
          )
        else
          failwith "Dynamic-only object set not supported"

  let unboundGet ctx name typ (target:Wrapped) =
    if Runtime.Utils.Type.isObject target.Type 
      then buildGet ctx name typ target
      else 
        if Runtime.Utils.Type.isBox target.Type then
            Wrap.wrapInBlock target (fun obj -> 
              let target = (Wrap.static' (Utils.Box.fieldByClrTypeT<Runtime.Object> obj))
              [
                (Expr.debug (sprintf "Type check for getting property '%s'" name))
                (Expr.ternary
                  (Utils.Box.typeIsT<Runtime.Object> obj)
                  (buildGet ctx name None target).Et
                  (Expr.defaultT<Runtime.Box>)
                )
              ]
            )
        else
          failwith "Dynamic-only object set not supported"

  let getProperty ctx target name =
    unboundGet ctx name None target

  let setProperty ctx name target value = 
    unboundSet ctx name target value

  let build (ctx:Context) properties id =
    match properties with
    | Some(_) -> failwith "Objects with auto-properties not supported"
    | None    -> 
      if not (ctx.ObjectCaches.ContainsKey id) then
        ctx.ObjectCaches.Add(id, Expr.constant (Runtime.ObjectCache.New(ctx.Environment.BaseClass)))

      let cache = ctx.ObjectCaches.[id]
      let new' = Expr.newArgsT<Runtime.Object> [Expr.field cache "Class"; Expr.defaultT<Runtime.Object> ;Expr.field cache "InitSize"]
      let assn = Expr.assign (Expr.field cache "LastCreated") new'

      Wrap.volatile' (assn)
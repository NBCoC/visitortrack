namespace VisitorTrack.EntityManager

open System

[<AutoOpen>]
module DataTypes =

    type Exception with
        member this.GetInnerMessage () =
            if isNull this.InnerException then
                this.Message
            else this.InnerException.GetInnerMessage()

    type DefaultPassword = DefaultPassword of string

    type HashedPassword = HashedPassword of string

    type EntityId = EntityId of string

    type UserRole =
        | Admin
        | Editor
        | Viewer

    type StorageOptions = {
        DatabaseId: string
        EndpointUrl: string
        AccountKey: string
        CollectionId: string
    }

    type ErrorResult = {
        Message: string
        Details: string option
    } with static member Create message = {
            Message = message
            Details = None
        }

[<RequireQualifiedAccess>]
module private StringType =

    let create propertyName length dataType value =
        if String.IsNullOrEmpty(value) then
            sprintf "%s is required" propertyName |> ErrorResult.Create |> Error
        elif value.Length > length then
            sprintf "%s cannot be longer than %i characters" propertyName length |> ErrorResult.Create |> Error
        else dataType value |> Ok

module DatabaseId =
    type T = private DatabaseId of string

    let create str =
        if String.IsNullOrEmpty(str) then
            ErrorResult.Create "Database ID is required" |> Error
        else DatabaseId str |> Ok

    let value (DatabaseId x) = x

module EndpointUrl =
    type T = private EndpointUrl of string

    let create str =
        if String.IsNullOrEmpty(str) then
            ErrorResult.Create "Endpoint URL is required" |> Error
        else EndpointUrl str |> Ok

    let value (EndpointUrl x) = x

module AccountKey =
    type T = private AccountKey of string

    let create str =
        if String.IsNullOrEmpty(str) then
            ErrorResult.Create "Account Key is required" |> Error
        else AccountKey str |> Ok

    let value (AccountKey x) = x

module CollectionId =
    type T = private CollectionId of string

    let create str =
        if String.IsNullOrEmpty(str) then
            ErrorResult.Create "Collection ID is required" |> Error
        else CollectionId str |> Ok

    let value (CollectionId x) = x


module String254 =
    type T = private String254 of string

    let create propertyName value =
        StringType.create propertyName 254 String254 value

    let value (String254 x) = x


module String75 =
    type T = private String75 of string

    let create propertyName value =
        StringType.create propertyName 75 String75 value

    let value (String75 x) = x

module String15 =
    type T = private String15 of string

    let create propertyName value =
        StringType.create propertyName 15 String15 value

    let value (String15 x) = x

module EmailAddress =
    type T = private EmailAddress of string

    let create value =

        let validate string254 =
            let value = String254.value string254
            
            if value.Contains("@") then
                value.ToLower().Trim() |> EmailAddress |> Ok 
            else ErrorResult.Create "Email Address must contain an '@' sign" |> Error

        String254.create "Email Address" value
        |> Result.bind validate

    let value (EmailAddress x) = x

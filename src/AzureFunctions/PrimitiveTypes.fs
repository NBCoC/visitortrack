namespace AzureFunctions

open System

module DatabaseId =
    type T = private DatabaseId of string

    let create str =
        if String.IsNullOrEmpty(str) then
            Error "Database ID is required"
        else DatabaseId str |> Ok

    let value (DatabaseId x) = x

module EndpointUrl =
    type T = private EndpointUrl of string

    let create str =
        if String.IsNullOrEmpty(str) then
            Error "Endpoint URL is required"
        else EndpointUrl str |> Ok

    let value (EndpointUrl x) = x

module AccountKey =
    type T = private AccountKey of string

    let create str =
        if String.IsNullOrEmpty(str) then
            Error "Account Key is required"
        else AccountKey str |> Ok

    let value (AccountKey x) = x

module CollectionId =
    type T = private CollectionId of string

    let create str =
        if String.IsNullOrEmpty(str) then
            Error "Collection ID is required"
        else CollectionId str |> Ok

    let value (CollectionId x) = x

[<RequireQualifiedAccess>]
module private StringType =

    let create propertyName length dataType value =
        if String.IsNullOrEmpty(value) then
            sprintf "%s is required" propertyName |> Error
        elif value.Length > length then
            sprintf "%s cannot be longer than %i characters" propertyName length |> Error
        else dataType value |> Ok

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
            else Error "Email Address must contain an '@' sign"

        String254.create "Email Address" value
        |> Result.bind validate

    let value (EmailAddress x) = x

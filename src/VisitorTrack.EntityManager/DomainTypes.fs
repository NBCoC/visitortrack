namespace VisitorTrack.EntityManager

open System

[<AutoOpen>]
module DataTypes =

    type Exception with
        member this.GetDetails () =
            if isNull this.InnerException then
                this.Message
            else 
                let message = this.InnerException.GetDetails()
                sprintf "%s %s %s" this.Message Environment.NewLine message 

    type DefaultPassword = DefaultPassword of string

    type HashedPassword = HashedPassword of string

    type EntityId = EntityId of string

    type Token = Token of string

    type CollectionId =
        | UserCollection
        | VisitorCollection
            with static member Value collectionId =
                    match collectionId with
                        | UserCollection -> "UserCollection"
                        | VisitorCollection -> "VisitorCollection"

    type SqlPropertyName =
        | EmailAddressSqlProperty
        | PasswordSqlProperty
            with static member Value propertyName =
                    match propertyName with
                        | EmailAddressSqlProperty -> "EmailAddress"
                        | PasswordSqlProperty -> "Password"

    type PropertyName =
        | DefaultPasswordProperty
        | DisplayNameProperty
        | EmailAddressProperty
        | PasswordProperty
        | OldPasswordProperty
        | NewPasswordProperty
            with static member Value propertyName =
                    match propertyName with
                        | DefaultPasswordProperty -> "Default Password"
                        | DisplayNameProperty -> "Display Name"
                        | EmailAddressProperty -> "Email Address"
                        | PasswordProperty -> "Password"
                        | OldPasswordProperty -> "Old Password"
                        | NewPasswordProperty -> "New Password"

    type StorageOptions = {
        DatabaseId: string
        EndpointUrl: string
        AccountKey: string
        CollectionId: CollectionId
    }

module CustomTypes =

    type DatabaseId = private DatabaseId of string

    type EndpointUrl = private EndpointUrl of string

    type AccountKey = private AccountKey of string

    type String254 = private String254 of string

    type String75 = private String75 of string

    type String15 = private String15 of string

    type EmailAddress = private EmailAddress of string

    let private create propertyName length dataType value =
        let propName = PropertyName.Value propertyName

        if String.IsNullOrEmpty(value) then
            sprintf "%s is required" propName |> Error
        elif value.Length > length then
            sprintf "%s cannot be longer than %i characters" propName length |> Error
        else dataType value |> Ok

    module DatabaseId =
        
        let create str =
            if String.IsNullOrEmpty(str) then
                Error "Database ID is required"
            else DatabaseId str |> Ok

        let apply f (DatabaseId x) = f x

        let value x = apply id x

    module EndpointUrl =
        
        let create str =
            if String.IsNullOrEmpty(str) then
                Error "Endpoint URL is required"
            else EndpointUrl str |> Ok

        let apply f (EndpointUrl x) = f x

        let value x = apply id x

    module AccountKey =
        
        let create str =
            if String.IsNullOrEmpty(str) then
                Error "Account Key is required"
            else AccountKey str |> Ok

        let apply f (AccountKey x) = f x

        let value x = apply id x

    module String254 =
        
        let create propertyName value =
            create propertyName 254 String254 value

        let apply f (String254 x) = f x

        let value x = apply id x


    module String75 =
        
        let create propertyName value =
            create propertyName 75 String75 value

        let apply f (String75 x) = f x

        let value x = apply id x

    module String15 =
        
        let create propertyName value =
            create propertyName 15 String15 value

        let apply f (String15 x) = f x

        let value x = apply id x

    module EmailAddress =
        
        let create value =

            let validate string254 =
                let value = String254.value string254
                
                if value.Contains("@") then
                    value.ToLower().Trim() |> EmailAddress |> Ok 
                else Error "Email Address must contain an '@' sign"

            String254.create EmailAddressProperty value
            |> Result.bind validate

        let apply f (EmailAddress x) = f x

        let value x = apply id x

namespace VisitorTrack.EntityManager

module CustomTypes =
    open System
    open VisitorTrack.Entities

    type DatabaseId = private DatabaseId of string

    type EndpointUrl = private EndpointUrl of string

    type AccountKey = private AccountKey of string

    type String254 = private String254 of string

    type String750 = private String750 of string

    type EmailAddress = private EmailAddress of string

    type ContactNumber = private ContactNumber of string

    type Password = private Password of string

    type DefaultPassword = DefaultPassword of string

    type HashedPassword = internal HashedPassword of string

    type EntityId = private EntityId of string

    type ContextUserId = private ContextUserId of string

    type StorageOptions = {
        DatabaseId: string
        EndpointUrl: string
        AccountKey: string
    }

    type ResetPassword = {
        Options: StorageOptions
        ContextUserId: string
        UserId: string
        Password: string
    }

    type UpdatePassword = {
        Options: StorageOptions
        ContextUserId: string
        Model: UpdateUserPassword
    }

    type AuthenticateUser = {
        Options: StorageOptions
        Model: UserAuthentication
    }

    type UpdateEntity<'Entity> = {
        Options: StorageOptions
        ContextUserId: string
        EntityId: string
        Model: 'Entity
    }

    type CreateEntity<'Entity> = {
        Options: StorageOptions
        ContextUserId: string
        Model: 'Entity
    }

    type DeleteEntity = {
        Options: StorageOptions
        ContextUserId: string
        EntityId: string
    }

    type VisitorSearch = {
        Options: StorageOptions
        Text: string
    }

    type UpdateVisitorCheckListItem = {
        Options: StorageOptions
        ContextUserId: string
        VisitorId: string
        Model: VisitorCheckListItem
    }

    type CollectionId =
        | User
        | Visitor
        | CheckList
            with static member Value collectionId =
                    match collectionId with
                        | User -> "UserCollection"
                        | Visitor -> "VisitorCollection"
                        | CheckList -> "CheckListCollection"

    let private create propertyName length dataType value =
        if String.IsNullOrEmpty(value) then
            sprintf "%s is required" propertyName |> Error
        elif value.Length > length then
            sprintf "%s cannot be longer than %i characters" propertyName length |> Error
        else dataType value |> Ok

    module EntityId =
        
        let create str =
            if String.IsNullOrEmpty(str) then
                Error "Entity ID is required"
            else EntityId str |> Ok

        let apply f (EntityId x) = f x

        let value x = apply id x

    module ContextUserId =
        
        let create str =
            if String.IsNullOrEmpty(str) then
                Error "Context User ID is required"
            else ContextUserId str |> Ok

        let apply f (ContextUserId x) = f x

        let value x = apply id x

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

        let equals (String254 x) (String254 y) =
            x = y

    module String750 =
        
        let create propertyName value =
            create propertyName 750 String750 value

        let apply f (String750 x) = f x

        let value x = apply id x

        let equals (String750 x) (String750 y) =
            x = y

    module Password =
        
        let create propertyName value =
            create propertyName 15 Password value

        let apply f (Password x) = f x

        let value x = apply id x

        let equals (Password x) (Password y) =
            x = y


    module EmailAddress =
        
        let create value =

            let validate string254 =
                let value = String254.value string254
                
                if value.Contains("@") then
                    value.ToLower().Trim() 
                    |> EmailAddress 
                    |> Ok 
                else Error "Email Address must contain an '@' sign"

            String254.create "Email Address" value
            |> Result.bind validate

        let apply f (EmailAddress x) = f x

        let value x = apply id x

        let equals (EmailAddress x) (EmailAddress y) =
            x = y

    module ContactNumber =
        open System.Text.RegularExpressions

        let create value =
            if String.IsNullOrEmpty(value) then
                Error "Contact # is required"
            elif Regex(@"\d{3}-\d{3}-\d{4}").Match(value).Success |> not then
                Error "Contact # is not valid. Format should be XXX-XXX-XXXX"
            else ContactNumber value |> Ok

        let apply f (ContactNumber x) = f x

        let value x = apply id x
   
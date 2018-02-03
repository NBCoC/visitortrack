namespace VisitorTrack.EntityManager

open System
open Newtonsoft.Json

module Models =

    [<CLIMutable>]
    type User = {
        [<JsonProperty(PropertyName = "id")>]
        Id: string
        Email: string
        Password: string
        Token: string
        DisplayName: string
        RoleId: int
    } 

module Dtos =

    type UpsertUserDto = {
        Email: string
        DisplayName: string
        RoleId: int
    }

module DataTypes =
    open Dtos

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

    type Connection = Microsoft.Azure.Documents.Client.DocumentClient * DatabaseId.T * CollectionId.T

    type ICreateUser = DefaultPassword * UpsertUserDto
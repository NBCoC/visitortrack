namespace VisitorTrack.EntityManager

open Dtos

module DataTypes =

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
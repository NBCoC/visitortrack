namespace VisitorTrack.EntityManager

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
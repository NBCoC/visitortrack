namespace VisitorTrack.EntityManager

open System.Runtime.Serialization
open Newtonsoft.Json.Serialization

module Entities =

    type User = {
        [<JsonProperty(PropertyName="id")>] 
        Id: string
        Email: string
        DisplayName: string
        RoleId: int
        Password: string
    }

module Dtos =

    [<DataContract>]
    type UserDto = {
        [<field: DataMember(Name="id")>] 
        Id: string
        [<field: DataMember(Name="email")>] 
        Email: string
        [<field: DataMember(Name="displayName")>] 
        DisplayName: string
        [<field: DataMember(Name="roleId")>] 
        RoleId: int
        [<field: DataMember(Name="roleName")>] 
        RoleName: string
        [<field: DataMember(Name="isAdmin")>] 
        IsAdmin: bool
        [<field: DataMember(Name="isEditor")>] 
        IsEditor: bool
    }

    [<DataContract>]
    type UpsertUserDto = {
        [<field: DataMember(Name="email")>] 
        Email: string
        [<field: DataMember(Name="displayName")>] 
        DisplayName: string
        [<field: DataMember(Name="roleId")>] 
        RoleId: int
    }
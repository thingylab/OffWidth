namespace OffWidth.Core

module Configuration =
    type Environment =
        | Environment of string
        | Staging
        | Qa
        | Uat
        | Production

    type ConnectionInfo =
        | ConnectionString of string

    type EnvironmentDescriptor =
        | EnvironmentDescriptor of Environment * ConnectionInfo

    type Context = {
        environments: EnvironmentDescriptor list
    }

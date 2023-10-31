## Setup client Strawbelly.Shake
1. Create Console Application
2. Open CLI and execute
```
dotnet new tool-manifest
dotnet tool install StrawberryShake.Tools --local --version <version>  (version < 13)
```
3. Nuget install StrawberryShake.CodeGeneration.CSharp.Analyzers `same strawberryshake tools version`
4. dotnet graphql init https://localhost:7267/graphql (depend)
5. Install Strawbelly.Http.Transport & DependencyInjection & StrawberryShake.Transport.Http & StrawberryShake.Transport.WebSockets

## Usages
1. `update graphql schema.graphql` command will regenerate latest graphql schemas. Be note you host port need in live!

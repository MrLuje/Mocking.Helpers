[![Build status](https://ci.appveyor.com/api/projects/status/kh61bjuq3eqb62rf/branch/master?svg=true
)](https://ci.appveyor.com/project/MrLuje/mocking-helpers)

# Mocking.Helpers
Helpers for .net mocking frameworks as a Visual Studio 2017 extension

## Moq
Support autocompletion of _Setup_ method



## Troubleshooting
This extension relies on [Roslyn](https://github.com/dotnet/roslyn) for type/methods parsing.
It will not work if Visual Studio is configured to used another completion mechanism than Intellisense (eg: R#)

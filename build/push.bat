@echo off

..\src\.nuget\nuget push Driven.%1.nupkg %NUGET_API_KEY%
..\src\.nuget\nuget push Driven.Testing.%1.nupkg %NUGET_API_KEY%
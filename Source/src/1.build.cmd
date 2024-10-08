echo off

rem cleaning WPF bin folders (MSBuild fails to do it)

rd /Q /S .\WixSharp.UI.WPF\obj
rd /Q /S .\WixSharp.UI.WPF\bin

.\WixSharp.Samples\cscs.exe /l .\..\bin\set_version.cs

PUSHD .
cd .\NET-Core\WixSharp.Core
dotnet publish . -c Release -o ".\out"
dotnet pack WixSharp.Core.csproj -o .\..\nuget
POPD

PUSHD .
cd .\NET-Core\WixSharp.Msi.Core
dotnet publish . -c Release -o ".\out"
dotnet pack WixSharp.Msi.Core.csproj -o .\..\nuget
POPD

copy .\NET-Core\nuget\*.*nupkg .\..\nuget\WixSharp\

set msbuild=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe
"%msbuild%" /nologo /verbosity:minimal /t:Clean,Restore,Build /p:Configuration=Release /p:Platform="Any CPU" /p:BuildInParallel=true WixSharp.Suite.sln


pause

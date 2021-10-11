all: build

build: restore
	dotnet publish -c Release -o ./dist

restore:
	dotnet restore

start-dev:
	ASPNETCORE_ENVIRONMENT=Development dotnet watch run

start-prod:
	dotnet run
	
align-project: nxa-sc-caas.csproj
	dotnet add $< reference ../../../neo-devpack-dotnet/src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj
	dotnet add $< reference ../../../neo-devpack-dotnet/src/Neo.SmartContract.Framework/Neo.SmartContract.Framework.csproj

clean:
	-@dotnet clean 2>&1 > /dev/null
	-@rm -rf ./dist
	-@rm -rf ./bin
	-@rm -rf ./obj

.PHONY: all build restore align-project clean

.SILENT: clean distclean


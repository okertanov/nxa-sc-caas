all: build
	
build:
	dotnet restore
	dotnet build

start-dev:
	ASPNETCORE_ENVIRONMENT=Development dotnet watch run

start-prod:
	dotnet run

clean:
	-@dotnet clean
	-@rm -rf ./obj
	-@rm -rf ./bin
	
distclean: clean

.PHONY: all build start clean distclean

.SILENT: clean distclean

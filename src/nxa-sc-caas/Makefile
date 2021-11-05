all: build

restore:
	dotnet restore

build: restore
	dotnet publish -c Release -o ./dist 

test:
	dotnet test

start-dev:
	ASPNETCORE_ENVIRONMENT=Development dotnet watch run

start:
	dotnet run

clean:
	-@dotnet clean 2>&1 > /dev/null
	-@rm -rf ./dist
	-@rm -rf ./bin
	-@rm -rf ./obj
	-@rm -rf ./CodeEditor/dist

.PHONY: all restore build start-dev start clean

.SILENT: clean distclean


all: test

test: 
	dotnet test --nologo --no-restore --verbosity normal

clean: 
	-@dotnet clean 2>&1 > /dev/null

.PHONY: all test clean

.SILENT: clean

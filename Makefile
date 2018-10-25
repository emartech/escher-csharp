include .env

clean:
	rm -rf EscherAuth/bin
	rm -rf EscherAuth/obj
	rm -rf EscherAuthTests/bin
	rm -rf EscherAuthTests/obj

test:
	dotnet test EscherAuthTests

package:
	dotnet pack --configuration Release EscherAuth

deploy: clean package
	dotnet nuget push EscherAuth/bin/Release/EscherAuth*.nupkg -k $(NUGET_API_KEY) -s https://api.nuget.org/v3/index.json
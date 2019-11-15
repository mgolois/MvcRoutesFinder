# Mvc Routes Finder
Find the list of routes of given MVC projects

## Build into a single exe

```dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true```

## Usage
#### command line
```MvcRoutesFinder.exe --help```

output
```
  -o, --output       CSV Output file

  -d, --directory    Required. Directories to scan

  --help             Display this help screen.

  --version          Display version information. 
```
### command line

```MvcRoutesFinder.exe -d "C:\solutionFoler" -o "output.cvs"```
  
  

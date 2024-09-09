# AssemblyDependanceChecker
Simple utility to check if all assembly dependencies are present.

A common runtime error that occurs during execution is the assembly not found error. Tracking dependencies, to can be annoying.
This utility uses assembly metadata, to load assemblies in a given directory to determine if the required assemblies are present.
This is still a work in progress, as assemblies in GAC and system utilities are not accounted for.

Additional enhancements coming soo

Ref: https://learn.microsoft.com/en-us/dotnet/standard/assembly/inspect-contents-using-metadataloadcontext

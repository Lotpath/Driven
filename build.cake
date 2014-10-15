// Get arguments passed to the script

var target = Argument("target", "All");
var configuration = Argument("configuration", "Release");
var buildLabel = Argument("buildLabel", string.Empty);
var buildInfo = Argument("buildInfo", string.Empty);

// Parse release notes.
var releaseNotes = ParseReleaseNotes("./ReleaseNotes.md");

// Set version.
var version = releaseNotes.Version.ToString();
var semVersion = version + (buildLabel != "" ? ("-" + buildLabel) : string.Empty);
Information("Building version {0} of Driven.", version);

// Define directories.
var buildResultDir = "./build/";

//////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() =>
{
	CleanDirectories(new DirectoryPath[] {
		buildResultDir });    
});

Task("Restore-NuGet-Packages")
	.IsDependentOn("Clean")
	.Does(context =>
{
	// Restore NuGet packages.
	NuGetRestore("./src/Driven.sln");    
});

Task("Patch-Assembly-Info")
	.Description("Patches the AssemblyInfo files.")
	.IsDependentOn("Restore-NuGet-Packages")
	.Does(() =>
{
	var file = "./src/SolutionInfo.cs";
	CreateAssemblyInfo(file, new AssemblyInfoSettings {
		Product = "Driven",
		Version = version,
		FileVersion = version,
		InformationalVersion = (version + buildInfo).Trim(),
		Copyright = "Copyright (c) Lotpath 2014"
	});
});

Task("Build-Solution")
	.IsDependentOn("Patch-Assembly-Info")
	.Does(() =>
{
	MSBuild("./src/Driven.sln", s => 
		{ 
			s.Configuration = configuration;
			s.ToolVersion = MSBuildToolVersion.NET45;
		});
});

Task("All")
	.Description("Final target.")
	.IsDependentOn("Build-Solution");

//////////////////////////////////////////////////////////////////////////

RunTarget(target);
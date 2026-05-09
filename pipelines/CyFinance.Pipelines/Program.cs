using ModularPipelines;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Extensions;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using ModularPipelines.Options;

var builder = Pipeline.CreateBuilder(args);

builder.Services.RegisterDotNetContext();

builder.Services
	.AddModule<ResolvePackageVersionModule>()
	.AddModule<PublishNuGetModule>();

await builder.Build().RunAsync();

public class ResolvePackageVersionModule : Module<string>
{
	protected override async Task<string?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
	{
		var inputVersion = Environment.GetEnvironmentVariable("INPUT_PACKAGE_VERSION");

		if (!string.IsNullOrWhiteSpace(inputVersion))
		{
			return inputVersion.Trim();
		}

		var gitVersionInformation = await context.Git().Versioning.GetGitVersioningInformation();
		var gitVersionNuGet = gitVersionInformation.SemVer ?? gitVersionInformation.FullSemVer;

		if (!string.IsNullOrWhiteSpace(gitVersionNuGet))
		{
			return gitVersionNuGet.Trim();
		}

		throw new InvalidOperationException("No package version could be resolved from GitVersion. Ensure GitVersion.yml exists at the repository root.");
	}
}

[DependsOn<ResolvePackageVersionModule>]
public class PublishNuGetModule : Module
{
	protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
	{
		var versionResult = await context.GetModule<ResolvePackageVersionModule>();
		var packageVersion = versionResult.ValueOrDefault;

		if (string.IsNullOrWhiteSpace(packageVersion))
		{
			throw new InvalidOperationException("Resolved package version is empty.");
		}

		var nugetApiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");

		if (string.IsNullOrWhiteSpace(nugetApiKey))
		{
			throw new InvalidOperationException("NUGET_API_KEY environment variable is required.");
		}

		await context.DotNet().Restore(new DotNetRestoreOptions
		{
			ProjectSolution = "CyFinance.sln",
		}, null, cancellationToken);

		await context.DotNet().Build(new DotNetBuildOptions
		{
			ProjectSolution = "src/CyFinance.csproj",
			Configuration = "Release",
			NoRestore = true,
		}, null, cancellationToken);

		await context.DotNet().Run(new DotNetRunOptions
		{
			Project = "tests/CyFinance.Tests.csproj",
		}, null, cancellationToken);

		var artifactsDirectory = Path.GetFullPath("./artifacts");
		if (Directory.Exists(artifactsDirectory))
		{
			Directory.Delete(artifactsDirectory, true);
		}

		await context.DotNet().Pack(new DotNetPackOptions
		{
			ProjectSolution = "src/CyFinance.csproj",
			Configuration = "Release",
			NoBuild = true,
			Output = "./artifacts",
			Properties =
			[
				new KeyValue("PackageId", "CyFinance"),
				new KeyValue("PackageVersion", packageVersion),
				new KeyValue("Authors", "Simon Uyttenhove"),
				new KeyValue("Description", "C# Client Library used to interact with the Yahoo Finance API"),
				new KeyValue("PackageProjectUrl", "https://github.com/UyttenhoveSimon/CyFinance"),
				new KeyValue("RepositoryUrl", "https://github.com/UyttenhoveSimon/CyFinance"),
				new KeyValue("RepositoryType", "git"),
			],
		}, null, cancellationToken);

		await PushPackagesAsync(context, nugetApiKey, cancellationToken);
	}

	private static async Task PushPackagesAsync(IModuleContext context, string nugetApiKey, CancellationToken cancellationToken)
	{
		var artifactsDirectory = Path.GetFullPath("./artifacts");
		var packageFiles = Directory.Exists(artifactsDirectory)
			? Directory.GetFiles(artifactsDirectory, "*.nupkg", SearchOption.TopDirectoryOnly)
			: Array.Empty<string>();

		if (packageFiles.Length == 0)
		{
			throw new InvalidOperationException("No .nupkg files found in artifacts directory.");
		}

		foreach (var packageFile in packageFiles)
		{
			await context.Shell.Command.ExecuteCommandLineTool(
				new GenericCommandLineToolOptions("dotnet")
				{
					Arguments =
					[
						"nuget",
						"push",
						packageFile,
						"--api-key",
						nugetApiKey,
						"--source",
						"https://api.nuget.org/v3/index.json",
						"--skip-duplicate",
					],
				},
				null,
				cancellationToken);
		}
	}
}

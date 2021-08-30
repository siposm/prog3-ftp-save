1. Take code from 05_ProjectDemo.sln
	Rename SLN
2. Add BrandRepository + Edit CarLogic + Edit Program
3. Nuget Package Manager for CarShop.Tests
	Microsoft.NET.Test.Sdk
	NUnit
	NUnit3TestAdapter
	Moq
4. Project references for CarShop.Tests
	Logic, Repository, Data
5. Write CarTests

6. Install Microsoft.CodeAnalysis.NetAnalyzers + Stylecop.Analyzers
7. EnableNETAnalyzers:
    main <PropertyGroup>:
        <EnableNETAnalyzers>true</EnableNETAnalyzers>
        <AnalysisMode>AllEnabledByDefault</AnalysisMode>
        <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
	OR check: Project Properties / Code Analysis
8. suppress in global .editorconfig (known locations only)
[*]
dotnet_diagnostic.CA1012.severity = none
9. suppress in GlobalSuppressions.cs
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("", "CA1014", Justification = "<NikGitStats>", Scope = "module")]


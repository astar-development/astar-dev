
    param(
        [Parameter(Mandatory=$true)]
        [ValidateSet("lib", "app", "test")]
        [string]$Type,

        [Parameter(Mandatory=$true)]
        [string]$Name
    )

    $root = (git rev-parse --show-toplevel)

    switch ($Type) {
        "lib" {
            $path = "$root/libs/$Name"
            New-Item -ItemType Directory -Path $path -Force | Out-Null
            Set-Content -Path "$path/$Name.csproj" -Value @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>
"@
            Set-Content -Path "$path/Class1.cs" -Value "namespace $Name; public class Class1 { }"
            Write-Host "Library '$Name' created."
        }

        "app" {
            $path = "$root/apps/$Name"
            New-Item -ItemType Directory -Path $path -Force | Out-Null
            Set-Content -Path "$path/$Name.csproj" -Value @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>
"@
            Set-Content -Path "$path/Program.cs" -Value 'Console.WriteLine("Hello from app.");'
            Write-Host "App '$Name' created."
        }

        "test" {
            $path = "$root/tests/$Name.Tests"
            New-Item -ItemType Directory -Path $path -Force | Out-Null
            Set-Content -Path "$path/$Name.Tests.csproj" -Value @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="xunit" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="FluentAssertions" />
  </ItemGroup>
</Project>
"@
            Set-Content -Path "$path/Class1Tests.cs" -Value "using Xunit; public class Class1Tests { [Fact] public void Placeholder() { } }"
            Write-Host "Test project '$Name.Tests' created."
        }
    }

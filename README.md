# Demo of .NET Core Attributes and Versioning #

## Add XML elements to CSPROJ ##

Here are typical attributes, you can add additional elements:

```XML
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.1</TargetFramework>
    <!-- These are the custom ones, you can add your own -->
    <Copyright>Copyright (c) 2018</Copyright>
    <Company>Magenic</Company>
    <Description>Assembly Info Demo</Description>
    <!-- 
        this is the version element 
        major.minor.release.build
        matching semver of 
        major.minor.patch[.not-used]
    -->
    <FileVersion>1.2.3.4</FileVersion>
  </PropertyGroup>

</Project>
```

## Semantic Versioning ##

All projects should use semantic versioning, following the .net convention of 

```bash
major.minor.release.build
```

this matches the <a href='https://semver.org/' target='_blank'>https://semver.org/</a> versioning of

```bash
major.minor.patch[.(not-used)]

Where:

* major: Major new version, Incremented (manually) for every push to production, should start at one (1)

* minor: Backward compatible feature release, Incremented (manually) for every push to production, reset to zero when any of the above changes

* release: Incremented (manually) for every push to production, reset to zero when any of the above changes
	- Release == Patch in SemVer.

* build: Incremented every build, reset to zero when any of the above changes
	- A build that results in a release, should result in this being reset to zero, and release + 1

## C# Code to Fetch Them ##

Here is a sample loop that will dump all of the attributes:

```C#
var assembly = typeof(Program).Assembly;
foreach (var attribute in assembly.GetCustomAttributesData())
{
    if (!attribute.TryParse(out string value)) value = string.Empty;
    Console.WriteLine($"{attribute.AttributeType.Name} - {value}");
}
```

## Attribute Parser ##

```C#
public static class AssembyInfoHelper
{
    public static bool TryParse(this System.Reflection.CustomAttributeData attribute, out string s)
    {
        var flag = false;
        s = attribute.ToString();
        var i = s.IndexOf('"');
        if (i >= 0) { s = s.Substring(i + 1); flag = true; }
        i = s.IndexOf('"');
        if (i >= 0) { s = s.Substring(0, i); flag = true; }
        return flag;
    }
}
```

## update_semver ##

This tool increments the sematic version build number by 1 for the `FileVersion` elements (as above).

1. Build the project 
2. Publish to get a platform specific binary using the scripts:

```bash
UpdateSemVer/publish-linux.sh
UpdateSemVer/publish-windows.sh
```
3. Copy outputs to where you want to use the tool

4. Use the tool

Assuming your project is called `my_project.csproj` and the two attributes are both set to `1.2.3.4`

```bash 
update_semver "my_project.csproj"
```

this yields the message below to STDOUT:

```bash
my_project.csproj updated to 1.2.3.5
```

The project can now be built, the CSPROJ file should be checked into source control. 
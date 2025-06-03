## This is outdated and now separately maintained in https://github.com/CSharpGodotTools/TemplateFramework

## What is this?
An ever expanding utils library for Godot C#. This is the library I am using across all my games, now open source for everyone else to enjoy as well.

## Install
Add this as a submodule to your GitHub repo
```
git submodule add https://github.com/CSharpGodotTools/GodotUtils GodotUtils
```

Add the following to your `.csproj`
```xml
<ItemGroup>
    <PackageReference Include="ENet-CSharp" Version="2.4.8" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
</ItemGroup>
```

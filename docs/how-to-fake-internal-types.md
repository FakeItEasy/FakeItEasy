# How to fake internal (Friend in VB) types

This guide will show you how to set up your project in order to be
able to fake internal types in your tested system.

## Details

The assembly that generates the proxy instances must have access to
your internal types, therefore a `InternalsVisibleTo` attribute must
be added to your tested assembly. Note that it is the assembly under
test, not your test-assembly that needs this attribute.

### Unsigned assemblies

If your assembly is not signed with a strong name, it's as easy as
adding the following to your project file (assuming you're using
.NET 5 SDK or higher):

```xml
<ItemGroup>
  <InternalsVisibleTo Include="DynamicProxyGenAssembly2" />
</ItemGroup>
```

If you're using an older SDK, you can add this to your
AssemblyInfo.cs/vb file instead:

=== "C#"
    ```csharp
    [assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
    ```
=== "VB"
    ```vb
    <Assembly:InternalsVisibleTo("DynamicProxyGenAssembly2")>
    ```

### Signed assemblies

For signed assemblies, you have to specify the public key of the
proxy-generating assembly in your project file:

```xml
<ItemGroup>
  <InternalsVisibleTo Include="DynamicProxyGenAssembly2" Key="0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7" />
</ItemGroup>
```

Or, if using an older SDK, in your AssemblyInfo.cs/vb file:
=== "C#"
    ```csharp
    [assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]
    ```
=== "VB"
    ```vb
    <Assembly:InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")>
    ```

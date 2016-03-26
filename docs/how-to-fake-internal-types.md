# How to fake internal (Friend in VB) types

This guide will show you how to set up your project in order to be
able to fake internal types in your tested system.

#Details

The assembly that generates the proxy instances must have access to
your internal types, therefore a `InternalsVisibleTo` attribute must
be added to your tested assembly. Note that it is the assembly under
test, not your test-assembly that needs this attribute.

##Unsigned assemblies

If your assembly is not signed with a strong name it's as easy as
adding the equivalent of the following to your AssemblyInfo.cs/vb
file:

```csharp
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
```

##Signed assemblies

For signed assemblies you have to specify the strong name of the
proxy-generating assembly:

```csharp
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]
```

# Slow startup time

##The Problem

Users with test solutions that include many (or very large) assemblies
sometimes report slow test runs with FakeItEasy. Often this manifests
as a very long time taken for the initial test to complete.

##The Cause

This behaviour is usually caused by the way FakeItEasy looks for
user-defined extension points. Some older versions load and scan all
DLLs in the working directory that aren't already in the AppDomain. If
there are many such DLLs, this can take several seconds. The time is
only spent on the first call to FakeItEasy, but it can be quite
annoying.

##Remedy

1. If you're **using a FakeItEasy older than 1.19.0**, upgrade
  now. Releases 1.13.0 and 1.19.0 included improvements to the
  scanning procedure that greatly reduced the startup time. If you
  upgrade to the latest FakeItEasy from a pre-1.19.0 release causes
  FakeItEasy to stop loading some extensible behaviour defined in an
  external assembly, you can configure the
  [Bootstrapper](bootstrapper.md) to get it back.
2. If you're already using the newest FakeItEasy and startup times
  seem unreasonably long, [create an issue][newissue], providing as
  much detail as possible so we can try to help.
  
[newissue]:https://github.com/FakeItEasy/FakeItEasy/issues/new

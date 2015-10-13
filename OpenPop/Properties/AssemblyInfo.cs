using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;

[assembly: InternalsVisibleTo("OpenPopUnitTests")]

[assembly: AssemblyTitle("OpenPop POP3 Mail Library")]
[assembly: AssemblyDescription("POP3 Mail Library")]

// The Assembly is compliant to CLS rules
[assembly: CLSCompliant(true)]

// Allow that assemblies can reference this assembly
// even though they do not have the FullTrust priviledge.
// This will not defeat security, since normal security policies
// like DNS, IO permissions are still in effect
[assembly: AllowPartiallyTrustedCallers]

// This file is linked to by other OpenPop projects to make
// sure that commen assembly information is shared correctly
[assembly: AssemblyVersion("2.0.6")]
[assembly: AssemblyFileVersion("2.0.6.0")]
[assembly: AssemblyInformationalVersion("2.0.6.0")]

// This is the configuration when building the assembly
[assembly: AssemblyConfiguration("Release mode assembly. Built for .NET 2.0")]

// Just some plain boring Assembly information
[assembly: AssemblyCompany("OpenPop")]
[assembly: AssemblyProduct("OpenPop")]
[assembly: AssemblyTrademark("OpenPop")]

// To clarify this License:
// See http://creativecommons.org/publicdomain/zero/1.0/
[assembly: AssemblyCopyright("Public Domain")]

// OpenPop is culture neutral, which is specified with an emptry string
[assembly: AssemblyCulture("")]

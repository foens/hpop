using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;

[assembly: InternalsVisibleTo("OpenPopUnitTests, PublicKey=002400000480000094000000060200000024000052534131000400000100010039266498B64729E5E4F937B37DD45F2502E51F542CDB5B3C79708E9FA5B57EB2A9B6772A6CFCBB83365568F34BD75DB85B6F27535349C734BBC220549026C12A1EB5679958029C3A64D2117BCD50DFB005FD5EDFC08D99BCA6DCA2A80B5B60C069308CFC30B2C348C8D9F63755CE9CFCA79F20C4B316C4A2082C2A733B4E6F8E")]

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

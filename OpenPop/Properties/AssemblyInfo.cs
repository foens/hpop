using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;

[assembly: InternalsVisibleTo("OpenPopUnitTests")]

[assembly: AssemblyDescription("POP3 Mail Library")]

// The Assembly is compliant to CLS rules
[assembly: CLSCompliant(true)]

// Allow that assemblies can reference this assembly
// even though they do not have the FullTrust priviledge.
// This will not defeat security, since normal security policies
// like DNS, IO permissions are still in effect
[assembly: AllowPartiallyTrustedCallers]


[assembly: AssemblyTrademark("OpenPop")]

// OpenPop is culture neutral, which is specified with an empty string
[assembly: AssemblyCulture("")]

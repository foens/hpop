using System.Reflection;
using System.Runtime.CompilerServices;

// Make sure that the unit tests can access internal classes
[assembly: InternalsVisibleTo("OpenPOPUnitTests")]

[assembly: AssemblyTitle("OpenPOP")]
[assembly: AssemblyDescription("POP3 Mail Library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("OpenPOP")]
[assembly: AssemblyProduct("OpenPOP")]
[assembly: AssemblyCopyright("Public Domain")]
[assembly: AssemblyTrademark("OpenPOP")]
[assembly: AssemblyCulture("")]		

[assembly: AssemblyVersion("2010.07.01.1600")]

// TODO When releasing, a strong-named assembly and a non-strong-named assembly should be created
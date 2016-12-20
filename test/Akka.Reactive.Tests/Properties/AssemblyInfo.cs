using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Akka.Reactive.Tests")]
[assembly: AssemblyDescription("Test suites for Akka.NET Rx integration")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("tin oscillator")]
[assembly: AssemblyProduct("Akka.Reactive")]
[assembly: AssemblyCopyright("Copyright © Adam Friedman 2016")]

[assembly: Guid("41817054-1718-4546-9d25-0ff16ef39a2d")]

using System.Reflection;
using System.Runtime.InteropServices;
#if VRC_SDK_VRCSDK3
using nadena.dev.ndmf;
#endif  // VRC_SDK_VRCSDK3

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Koturn.LilOptimized.NDMF.Editor")]
[assembly: AssemblyDescription("NDMF plugin for koturn/LilOptimized.")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif  // DEBUG
[assembly: AssemblyCompany("koturn")]
[assembly: AssemblyProduct("Koturn.LilOptimized.NDMF.Editor")]
[assembly: AssemblyCopyright("Copyright (C) 2024 koturn All Rights Reserverd.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("e2d9d48e-cf4b-4574-3829-d146cc6f52ec")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.2.2.0")]
[assembly: AssemblyFileVersion("1.2.2.0")]
[assembly: AssemblyInformationalVersion("1.2.2.0")]

// Metadata
[assembly: AssemblyMetadata("RepositoryUrl", "https://github.com/koturn/LillKustomShaders")]

#if VRC_SDK_VRCSDK3
// Export plugin
[assembly: ExportsPlugin(typeof(Koturn.LilOptimized.NDMF.Editor.ShaderReplacePlugin))]
#endif  // VRC_SDK_VRCSDK3

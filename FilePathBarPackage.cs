using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace FilePathBar;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[Guid(PackageGuidString)]
public sealed class FilePathBarPackage : AsyncPackage
{
    public const string PackageGuidString = "10f83ff5-af08-4dc7-bf3f-9d4cd2bfc2c8";

    protected override Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        return Task.CompletedTask;
    }
}

param (
    [Parameter(Mandatory=$true)]
    [string]$NuGetRoot,
    [Parameter(Mandatory=$true)]
    [string]$FullBuildNumber,
    [string]$VSPackageResxRelativePath="src\NuGet.Clients\VsExtension\VSPackage.resx",
    [string]$ProductVersionResourceId="116")

trap
{
    Write-Host "UpdateVSIXProductVersion.ps1 threw an exception: " $_.Exception -ForegroundColor Red
    exit 1
}

$NuGetRoot = (Resolve-Path $NuGetRoot).ToString()

$VSPackageResxPath = Join-Path $NuGetRoot $VSPackageResxRelativePath

Write-Verbose "Updating $VSPackageResxPath"
$xml = [xml](Get-Content $VSPackageResxPath)

$node = $xml.root.data | where { $_.Name -eq $ProductVersionResourceId }

$node.Value = $FullBuildNumber

$xml.Save($VSPackageResxPath)
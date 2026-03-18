# Update-Version.ps1
# Finds PublicVariablesAndSubs.vb, reads version "vX.Y.Z", bumps it (Z++ with carry to Y/X),
# and writes the updated file back. Prints the old -> new version.

param(
  [string]$Root = (Get-Location).Path,         # Solution root (e.g., $(SolutionDir))
  [string]$FileName = 'PublicVariablesAndSubs.vb'
)

$ErrorActionPreference = 'Stop'

# 1) Locate the .vb file
$rootPath = (Resolve-Path -LiteralPath $Root).Path
$target = Get-ChildItem -Path $rootPath -Recurse -Filter $FileName | Where-Object { -not $_.PSIsContainer } | Select-Object -First 1
if (-not $target) {
  Write-Host "[Version] File not found: $FileName under $rootPath" -ForegroundColor Yellow
  exit 0  # don't fail the build if missing
}

# 2) Read all lines
$lines = Get-Content -LiteralPath $target.FullName

# 3) Find version pattern: Public version As String = "v2.5.0"
$pattern = 'Public\s+version\s+As\s+String\s*=\s*"v(\d+)\.(\d+)\.(\d+)"'
$found = $false
$newLines = @()
$oldVer = ""
$newVer = ""

foreach ($line in $lines) {
  if (-not $found) {
    $m = [System.Text.RegularExpressions.Regex]::Match($line, $pattern, 'IgnoreCase')
    if ($m.Success) {
      $major = [int]$m.Groups[1].Value
      $minor = [int]$m.Groups[2].Value
      $patch = [int]$m.Groups[3].Value
      $oldVer = "v{0}.{1}.{2}" -f $major, $minor, $patch

      # bump: patch++ with carry to minor/major at 10
      $patch += 1
      if ($patch -ge 10) {
        $patch = 0
        $minor += 1
        if ($minor -ge 10) {
          $minor = 0
          $major += 1
        }
      }
      $newVer = "v{0}.{1}.{2}" -f $major, $minor, $patch

      # replace in line (preserve spacing/format of the rest)
      $line = [System.Text.RegularExpressions.Regex]::Replace(
        $line, $pattern, ('Public version As String = "' + $newVer + '"'), 'IgnoreCase'
      )
      $found = $true
    }
  }
  $newLines += $line
}

if ($found) {
  Set-Content -LiteralPath $target.FullName -Value $newLines -Encoding UTF8
  Write-Host "[Version] $oldVer -> $newVer in $($target.FullName)" -ForegroundColor Green
} else {
  Write-Host "[Version] No matching version line found in $($target.FullName)" -ForegroundColor Yellow
}

exit 0

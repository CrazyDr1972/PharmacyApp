<# 
  ExportAllVbAsTxt.ps1
  - Εξάγει ΜΟΝΟ τα .vb που περιέχονται στο .vbproj (Compile Include/Update)
  - Χωρίς υποφακέλους στον προορισμό
  - Σβήνει παλιά .txt πριν το export
  - Naming:
      *.Designer.vb -> *.designer.txt (lowercase, χωρίς .vb)
      *.vb          -> *.vb.txt
#>

param(
  [string]$Root = (Get-Location).Path,
  [string]$Project,
  [string]$Output = 'E:\- Documents & Projects\Visual Studio 2012 - LEARNING\Pharmacy\Pharmacy\Chat\Latest'
)

$ErrorActionPreference = 'Stop'

function Resolve-ProjectFile {
  param([string]$root, [string]$projectHint)
  if ($projectHint) {
    if (-not (Test-Path -LiteralPath $projectHint)) { throw "Το .vbproj δεν βρέθηκε: $projectHint" }
    return (Resolve-Path -LiteralPath $projectHint).Path
  }
  $candidates = Get-ChildItem -LiteralPath $root -Recurse -Filter *.vbproj -File | Sort-Object FullName
  if (-not $candidates) { throw "Δεν βρέθηκε κανένα .vbproj μέσα στο: $root" }
  if ($candidates.Count -gt 1) {
    Write-Host "[Export][Info] Βρέθηκαν πολλαπλά .vbproj. Χρησιμοποιώ το πρώτο:" -ForegroundColor Yellow
    $candidates | ForEach-Object { "  - " + $_.FullName } | Write-Host
  }
  return $candidates[0].FullName
}

try {
  $rootPath = (Resolve-Path -LiteralPath $Root).Path
  $projPath = Resolve-ProjectFile -root $rootPath -projectHint $Project
  $projDir  = Split-Path -Parent $projPath

  Write-Host "[Export] Project: $projPath"
  Write-Host "[Export] Output:  $Output"

  if (-not (Test-Path -LiteralPath $Output)) { New-Item -ItemType Directory -Path $Output -Force | Out-Null }

  # Καθάρισε ΠΑΛΙΑ .txt
  Get-ChildItem -LiteralPath $Output -Filter *.txt -File -ErrorAction SilentlyContinue |
    Remove-Item -Force -ErrorAction SilentlyContinue

  # ---- Διάβασε τα αρχεία από το .vbproj (αγνοώντας namespaces) ----
  [xml]$projXml = Get-Content -LiteralPath $projPath -Raw
  $compileNodes = Select-Xml -Xml $projXml -XPath '//*[local-name()="Compile" and (@Include or @Update)]'
  if (-not $compileNodes) { throw "Το project δεν έχει Compile Include/Update entries." }

  $files = @()
  foreach ($node in $compileNodes) {
    $rel = $node.Node.GetAttribute('Include'); if ([string]::IsNullOrWhiteSpace($rel)) { $rel = $node.Node.GetAttribute('Update') }
    if ([string]::IsNullOrWhiteSpace($rel)) { continue }
    $candidate = Join-Path $projDir $rel
    try { $resolved = (Resolve-Path -LiteralPath $candidate -ErrorAction Stop).Path } catch { continue }
    if ([System.IO.Path]::GetExtension($resolved).ToLower() -eq '.vb') { $files += $resolved }
  }
  $files = $files | Sort-Object -Unique
  if (-not $files -or $files.Count -eq 0) { throw "Δεν βρέθηκαν .vb αρχεία στο project (Compile Include/Update)." }

  function Get-DestName {
    param([string]$fullPath)
    $base = [System.IO.Path]::GetFileName($fullPath)
    if ($base -match '\.Designer\.vb$') {
      return ($base -replace '\.Designer\.vb$','.designer.txt')
    } elseif ($base -match '\.vb$') {
      return ($base + '.txt')
    } else {
      return ($base + '.txt')
    }
  }

  # Για αποφυγή συγκρούσεων ίδιων ονομάτων από διαφορετικούς φακέλους
  $used = @{}
  $count = 0

  foreach ($full in $files) {
    $rel = $full.Substring($projDir.Length).TrimStart('\','/')
    $destName = Get-DestName -fullPath $full

    if ($used.ContainsKey($destName)) {
      # Αν υπάρξει σύγκρουση, χρησιμοποίησε επίπεδο rel-path
      $destName = ($rel -replace '[:\\/]', '__') + '.txt'
    }
    $used[$destName] = $true

    $dest = Join-Path $Output $destName

    $header = @(
      "' Exported: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss K')"
      "' Project:  $(Split-Path -Leaf $projPath)"
      "' Source:   $rel"
      "'"
    )
    $content = Get-Content -LiteralPath $full
    $toWrite = $header + $content
    Set-Content -LiteralPath $dest -Value $toWrite -Encoding UTF8

    $count++
    Write-Host ("[Export] {0} -> {1}" -f $rel, $destName)
  }

  # index.txt για γρήγορη αναφορά
  $indexPath = Join-Path $Output 'index.txt'
  $relList = $files | ForEach-Object { $_.Substring($projDir.Length).TrimStart('\','/') } | Sort-Object
  Set-Content -LiteralPath $indexPath -Value $relList -Encoding UTF8

  Write-Host ("`n[Export] Ολοκληρώθηκε. Σύνολο .vb: {0}" -f $count) -ForegroundColor Green
  exit 0
}
catch {
  Write-Host ("[Export][ERROR] {0}" -f $_.Exception.Message) -ForegroundColor Red
  exit 1
}

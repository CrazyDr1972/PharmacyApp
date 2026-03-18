<# 
  ExportAllVbAsTxt.ps1
  - Εξάγει όλα τα .vb σε .txt, χωρίς υποφακέλους
  - Σβήνει τα παλιά .txt στον φάκελο προορισμού
  - Αγνοεί bin/obj/.vs/.git/packages
#>

param(
  # Ρίζα project (π.χ. $(ProjectDir) από VS)
  [string]$Root = (Get-Location).Path,

  # Προορισμός (σταθερός όπως ζητήθηκε)
  [string]$Output = 'E:\- Documents & Projects\Visual Studio 2012 - LEARNING\Pharmacy\Pharmacy\Chat\Latest'
)

$ErrorActionPreference = 'Stop'

try {
  # Resolve absolute paths
  $rootPath = (Resolve-Path -LiteralPath $Root).Path
  $outputPath = $Output

  Write-Host "[Export] Root:   $rootPath"
  Write-Host "[Export] Output: $outputPath"

  # Δημιουργία φακέλου προορισμού
  if (-not (Test-Path -LiteralPath $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath -Force | Out-Null
  }

  # Σβήσιμο παλαιότερων .txt (μόνο .txt, ώστε να μην πειράξουμε άλλα αρχεία)
  Get-ChildItem -LiteralPath $outputPath -Filter *.txt -File -ErrorAction SilentlyContinue | Remove-Item -Force -ErrorAction SilentlyContinue

  # Εντοπισμός όλων των .vb αρχείων (αναδρομικά), με φίλτρα αποκλεισμού
  $excluded = '\(bin|obj|packages|\.vs|\.git)(\\|/)'
  $files = Get-ChildItem -LiteralPath $rootPath -Recurse -Include *.vb -File |
           Where-Object { $_.FullName -notmatch $excluded }

  if (-not $files -or $files.Count -eq 0) {
    Write-Host "[Export] Δεν βρέθηκαν αρχεία .vb στον φάκελο: $rootPath" -ForegroundColor Yellow
    exit 0
  }

  $count = 0
  foreach ($file in $files) {
    # Σχετικό μονοπάτι από root
    $rel = $file.FullName.Substring($rootPath.Length).TrimStart('\','/')

    # Επίπεδη ονομασία (χωρίς υποφακέλους): αντικατάσταση \ / : με __
    $flatName = ($rel -replace '[:\\/]', '__') + '.txt'
    $dest = Join-Path $outputPath $flatName

    # Προαιρετική επικεφαλίδα με πληροφορία εξαγωγής
    $header = @(
      "' Exported: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss K')"
      "' Source:   $rel"
      "' Root:     $rootPath"
      "'"
    )

    # Ανάγνωση & εγγραφή UTF-8
    $content = Get-Content -LiteralPath $file.FullName
    $toWrite = $header + $content
    Set-Content -LiteralPath $dest -Value $toWrite -Encoding UTF8

    $count++
    Write-Host ("[Export] {0} -> {1}" -f $rel, $flatName)
  }

  # Προαιρετικά: δημιουργία index.txt με τα σχετικα paths (για γρήγορη αναφορά)
  $indexPath = Join-Path $outputPath 'index.txt'
  $relList = @()
  foreach ($f in $files) {
    $relList += $f.FullName.Substring($rootPath.Length).TrimStart('\','/')
  }
  $relList = $relList | Sort-Object
  Set-Content -LiteralPath $indexPath -Value $relList -Encoding UTF8

  Write-Host ("`n[Export] Ολοκληρώθηκε. Σύνολο αρχείων: {0}" -f $count) -ForegroundColor Green
  exit 0
}
catch {
  Write-Host ("[Export][ERROR] {0}" -f $_.Exception.Message) -ForegroundColor Red
  exit 1
}

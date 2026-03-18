param(
  [string]$Root = (Get-Location).Path,            # π.χ. "$(ProjectDir)"
  [string]$FileName = 'GlobalVariables.vb'        # αρχείο-στόχος
)

$ErrorActionPreference = 'Stop'

try {
  # 1) Normalize root & target
  $rootPath = (Resolve-Path -LiteralPath $Root).Path
  $target = Join-Path -Path $rootPath -ChildPath $FileName

  if (-not (Test-Path -LiteralPath $target)) {
    # fallback: ψάξε αναδρομικά μόνο αν δεν υπάρχει στο root
    $targetItem = Get-ChildItem -Path $rootPath -Recurse -File -Filter $FileName | Select-Object -First 1
    if ($null -eq $targetItem) {
      Write-Host "[Update-Version] ❌ Δεν βρέθηκε το $FileName κάτω από $rootPath" -ForegroundColor Red
      exit 1
    }
    $target = $targetItem.FullName
  }

  # 2) Διάβασε ολόκληρο το αρχείο
  $text = Get-Content -LiteralPath $target -Raw -Encoding UTF8

  # 3) Regex για τη γραμμή έκδοσης (case-insensitive, multiline)
  # Πιάνει Public/Friend/Private (προαιρετικά), Shared (προαιρετικά), ακριβώς "Version As String = ",
  # και την έκδοση vX.Y.Z (το 'v' προαιρετικό).
  $rx = [regex]'(?im)^\s*(?<lhs>(?:Public|Friend|Private)?\s*(?:Shared\s+)?Version\s+As\s+String\s*=\s*)"(?:v)?(?<maj>\d+)\.(?<min>\d+)\.(?<pat>\d+)"\s*$'
  $m = $rx.Match($text)
  if (-not $m.Success) {
    Write-Host "[Update-Version] ⚠ Δεν βρέθηκε γραμμή τύπου: Public version As String = ""vX.Y.Z""" -ForegroundColor Yellow
    exit 0
  }

  $major = [int]$m.Groups['maj'].Value
  $minor = [int]$m.Groups['min'].Value
  $patch = [int]$m.Groups['pat'].Value

  $oldVer = "v{0}.{1}.{2}" -f $major, $minor, $patch

  # 4) Bump με carry (προαιρετικό όριο στα 100)
  $patch++
  if ($patch -ge 10) {
    $patch = 0
    $minor++
    if ($minor -ge 10) {
      $minor = 0
      $major++
    }
  }

  $newVer = "v{0}.{1}.{2}" -f $major, $minor, $patch

  # 5) Αντικατάσταση ΜΟΝΟ στη γραμμή της έκδοσης (preserve το υπόλοιπο formatting)
  $newText = $rx.Replace($text, {
  param($mm)
  $lhs = $mm.Groups['lhs'].Value   # ό,τι έχεις πριν τα εισαγωγικά: διατηρεί κενά/κεφαλαία/Shared
  "$lhs`"$newVer`""
  }, 1)


  if ($newText -ne $text) {
    Set-Content -LiteralPath $target -Value $newText -Encoding UTF8
    Write-Host "[Update-Version] ✅ $oldVer → $newVer" -ForegroundColor Green
    Write-Host "[Update-Version] 📄 $target"
    exit 0
  } else {
    Write-Host "[Update-Version] ⚠ Δεν έγινε καμία αλλαγή (πιθανώς ίδια έκδοση)" -ForegroundColor Yellow
    exit 0
  }
}
catch {
  Write-Host "[Update-Version] ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
  exit 1
}

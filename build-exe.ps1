# ساخت خروجی exe تک‌فایلی برای ویندوز (x64)
# اجرا در PowerShell:  ./build-exe.ps1
$ErrorActionPreference = "Stop"

$proj = Join-Path $PSScriptRoot "DentalCenter.csproj"
$out  = Join-Path $PSScriptRoot "publish"

Write-Host "Restoring..." -ForegroundColor Cyan
dotnet restore $proj -r win-x64

Write-Host "Publishing single-file exe..." -ForegroundColor Cyan
dotnet publish $proj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:IncludeAllContentForSelfExtract=true `
    -p:PublishTrimmed=false `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=embedded `
    -o $out

Write-Host ""
Write-Host "Done. Output:" -ForegroundColor Green
Get-ChildItem $out -Filter *.exe | ForEach-Object {
    Write-Host ("  " + $_.FullName + "  (" + [math]::Round($_.Length/1MB,1) + " MB)")
}
Write-Host ""
Write-Host "همین فایل DentalCenter.exe را اجرا کنید. نیازی به نصب .NET روی سیستم مقصد نیست." -ForegroundColor Yellow

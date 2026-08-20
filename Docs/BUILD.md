# ساخت خروجی EXE ویندوز — DentalCenter (Avalonia 12 / .NET 10)

## روش ۱ — بیلد خودکار روی GitHub (پیشنهادی)

ورک‌فلو در `ci/build-exe.yml` آماده است. برای فعال‌سازی روی مخزن، آن را در مسیر استاندارد ورک‌فلو کپی کنید:

### لینوکس / مک

```bash
git clone https://github.com/rwm540/C--class.git
cd C--class
mkdir -p .github/workflows
cp ci/build-exe.yml .github/workflows/build-exe.yml
git add .github/workflows/build-exe.yml
git commit -m "Enable Windows exe build workflow"
git push origin main
```

### ویندوز (PowerShell)

```powershell
git clone https://github.com/rwm540/C--class.git
cd C--class
New-Item -ItemType Directory -Force .github\workflows | Out-Null
Copy-Item ci\build-exe.yml .github\workflows\build-exe.yml
git add .github\workflows\build-exe.yml
git commit -m "Enable Windows exe build workflow"
git push origin main
```

بعد از push، بیلد به‌صورت خودکار شروع می‌شود:

| خروجی | آدرس |
|---|---|
| Artifact با نام `DentalCenter-win-x64` | <https://github.com/rwm540/C--class/actions> |
| Release با تگ `win-build-<شماره>` | <https://github.com/rwm540/C--class/releases> |

---

## روش ۲ — بیلد محلی روی ویندوز

پیش‌نیاز: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

```powershell
./build-exe.ps1
```

خروجی: `publish\DentalCenter.exe` — تک‌فایل، self-contained، حدود ۷۰ تا ۹۰ مگابایت.
روی سیستم مقصد نیازی به نصب .NET یا فونت فارسی نیست.

### حالت‌های دیگر

| هدف | تغییر در دستور |
|---|---|
| exe سبک (نیازمند .NET 10 روی مقصد) | `--self-contained false` |
| ویندوز ARM64 | `-r win-arm64` |
| بیلد از لینوکس/مک برای ویندوز | افزودن `-p:EnableWindowsTargeting=true` |

---

## نکات فنی مهم پروژه

۱. **publish تک‌فایل باید native libها را unpack کند.** بدون
`IncludeNativeLibrariesForSelfExtract=true` کتابخانه‌های Skia بارگذاری نمی‌شوند و
پنجره اصلاً باز نمی‌شود. این تنظیم در `DentalCenter.csproj` و در ورک‌فلو اعمال شده است.

۲. **مسیر منابع بعد از publish.** همهٔ تصاویر و فونت‌ها به‌صورت `AvaloniaResource`
داخل خود اسمبلی جاسازی می‌شوند و `AssetsHelper` ابتدا از `avares://` می‌خواند و
فقط در صورت نبود، سراغ دیسک می‌رود.

۳. **نبود فایل نباید برنامه را بخواباند.** `AssetsHelper.LoadImage` در صورت نبود تصویر
`null` برمی‌گرداند و رابط کاربری متن جایگزین نشان می‌دهد. دکمهٔ PDF هم اگر فایل موجود
نباشد غیرفعال می‌شود.

۴. **فونت فارسی.** وزیرمتن (`Assets/Fonts`) در `Program.cs` به‌عنوان
`DefaultFamilyName` ثبت شده تا روی هر ویندوزی متن فارسی درست نمایش داده شود.

۵. **سازگاری با Avalonia 12.** در نسخهٔ ۱۲ متد `IClipboard.SetTextAsync` از خود
اینترفیس حذف و به extension method تبدیل شده؛ به همین دلیل
`using Avalonia.Input.Platform;` در `ContactView` لازم است.

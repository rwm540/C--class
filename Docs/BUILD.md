# ساخت خروجی EXE ویندوز — DentalCenter (Avalonia 12 / .NET 10)

## روش ۱ — بیلد خودکار روی GitHub (پیشنهادی)

ورک‌فلوی آمادهٔ ساخت exe در `ci/build-exe.yml` قرار دارد، اما در `.github/workflows/`
نیست؛ چون توکن GitHub App این نشست اجازهٔ نوشتن در مسیر workflowها را ندارد:

```
! [remote rejected] refusing to allow a GitHub App to create or update workflow
  `.github/workflows/build-exe.yml` without `workflows` permission
```

این محدودیت سمت گیت‌هاب است. با **یک دستور** روی سیستم خودتان فعال می‌شود:

### لینوکس / مک

```bash
git clone https://github.com/rwm540/C--class.git
cd C--class
git checkout arena/01a011a0-c-class
mkdir -p .github/workflows
cp ci/build-exe.yml .github/workflows/build-exe.yml
git add .github/workflows/build-exe.yml
git commit -m "Enable Windows exe build workflow"
git push origin arena/01a011a0-c-class
```

### ویندوز (PowerShell)

```powershell
git clone https://github.com/rwm540/C--class.git
cd C--class
git checkout arena/01a011a0-c-class
New-Item -ItemType Directory -Force .github\workflows | Out-Null
Copy-Item ci\build-exe.yml .github\workflows\build-exe.yml
git add .github\workflows\build-exe.yml
git commit -m "Enable Windows exe build workflow"
git push origin arena/01a011a0-c-class
```

بعد از push، بیلد به‌صورت خودکار شروع می‌شود:

| خروجی | آدرس |
|---|---|
| Artifact با نام `DentalCenter-win-x64` | <https://github.com/rwm540/C--class/actions> |
| Release با تگ `win-build-<شماره>` | <https://github.com/rwm540/C--class/releases> |

گزینهٔ دیگر: در Arena اتصال GitHub را با دسترسی `workflows` دوباره وصل کنید و به من بگویید تا خودم push کنم.

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

۲. **مسیر منابع بعد از publish.** کدِ قدیمی برای پیدا کردن تصاویر دنبال فایل
`DentalCenter.csproj` می‌گشت که کنار exe نهایی وجود ندارد. حالا همهٔ تصاویر و فونت‌ها
به‌صورت `AvaloniaResource` داخل خود اسمبلی جاسازی می‌شوند و `AssetsHelper` ابتدا از
`avares://` می‌خواند و فقط در صورت نبود، سراغ دیسک می‌رود.

۳. **نبود فایل نباید برنامه را بخواباند.** `AssetsHelper.LoadImage` در صورت نبود تصویر
`null` برمی‌گرداند و رابط کاربری متن جایگزین نشان می‌دهد. دکمهٔ PDF هم اگر فایل موجود
نباشد غیرفعال می‌شود.

۴. **فونت فارسی.** وزیرمتن (`Assets/Fonts`) در `Program.cs` به‌عنوان
`DefaultFamilyName` ثبت شده تا روی هر ویندوزی متن فارسی درست نمایش داده شود.

۵. **سازگاری با Avalonia 12.** در نسخهٔ ۱۲ متد `IClipboard.SetTextAsync` از خود
اینترفیس حذف و به extension method تبدیل شده؛ به همین دلیل
`using Avalonia.Input.Platform;` در `ContactView` لازم است.

---

## چرا بیلد در محیط ایجنت انجام نشد

شبکهٔ سندباکس این دامنه‌ها را مسدود کرده است:

- `dot.net`, `builds.dotnet.microsoft.com`, `download.visualstudio.microsoft.com` → نصب .NET SDK ناممکن
- `api.nuget.org` و میرورها → restore پکیج‌های Avalonia ناممکن
- مخازن apt نیز در دسترس نیستند

فقط `github.com`، PyPI و npm باز هستند و هیچ‌کدام .NET SDK ندارند. ضمناً برای ساخت exe
با manifest درست، runner ویندوزی لازم است — به همین دلیل GitHub Actions راه درست است.

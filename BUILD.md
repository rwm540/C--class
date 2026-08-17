# ساخت خروجی EXE — DentalCenter (Avalonia / .NET 10)

## وضعیت این سندباکس
در محیط اجرای این ایجنت (لینوکس، بدون اینترنت آزاد) دسترسی به دامنه‌های زیر مسدود است:

- `dot.net`, `builds.dotnet.microsoft.com`, `download.visualstudio.microsoft.com` → نصب .NET SDK ممکن نیست
- `api.nuget.org`, `nuget.org` → بازیابی پکیج‌های Avalonia ممکن نیست
- مخازن apt دبیان هم در دسترس نیستند

به همین دلیل خود فایل `.exe` نمی‌تواند این‌جا تولید شود. دو مسیر آماده در ادامه هست که هر کدام
روی سیستم شما یا روی GitHub، خروجی exe را می‌سازد.

---

## روش ۱ — ساخت روی ویندوز خودتان (سریع‌ترین)

پیش‌نیاز: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

```powershell
git clone https://github.com/rwm540/C--class.git
cd C--class
./build-exe.ps1
```

یا به‌صورت دستی:

```powershell
dotnet publish DentalCenter.csproj -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
```

خروجی: `publish\DentalCenter.exe` — تک‌فایلی و self-contained (بدون نیاز به نصب .NET روی
سیستم مقصد، حدود ۷۰–۹۰ مگابایت).

### حالت‌های دیگر
| هدف | دستور |
|---|---|
| exe سبک (نیازمند نصب .NET 10 روی مقصد) | `dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish` |
| ویندوز ARM64 | همان دستور با `-r win-arm64` |
| حجم کمتر (trim) | افزودن `-p:PublishTrimmed=true` (برای Avalonia با احتیاط تست شود) |

> نکته: اگر بخواهید از لینوکس/مک برای ویندوز بیلد کنید، `-p:EnableWindowsTargeting=true` را هم اضافه کنید.

---

## روش ۲ — ساخت خودکار روی GitHub Actions

فایل ورک‌فلوی آماده در `ci/build-exe.yml` قرار دارد. توکن این ایجنت اجازهٔ نوشتن در
`.github/workflows/` را ندارد (`workflows` permission نداشت)، پس یک بار خودتان آن را جابه‌جا و
کامیت کنید:

```bash
mkdir -p .github/workflows
git mv ci/build-exe.yml .github/workflows/build-exe.yml
git commit -m "Add Windows exe build workflow"
git push
```

بعد از push، در تب **Actions** ورک‌فلو «Build Windows EXE» اجرا می‌شود و فایل exe به‌عنوان
artifact با نام `DentalCenter-win-x64` قابل دانلود است. اجرای دستی هم از طریق
**Run workflow** ممکن است.

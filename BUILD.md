# ساخت خروجی EXE ویندوز — DentalCenter (Avalonia / .NET 10)

## ⚠️ یک مانع که فقط شما می‌توانید رفع کنید

ورک‌فلوی کامل و آماده در `ci/build-exe.yml` نوشته شده، اما GitHub اجازه نداد من آن را
در مسیر `.github/workflows/` قرار دهم:

```
! [remote rejected] refusing to allow a GitHub App to create or update workflow
  `.github/workflows/build-exe.yml` without `workflows` permission
```

توکن GitHub App این نشست، دسترسی `workflows` ندارد (از طریق REST API هم `403 Resource not
accessible by integration`). این محدودیت سمت گیت‌هاب است، نه چیزی که با کد دور بزنم.

**یک دستور، و بیلد شروع می‌شود:**

```bash
git pull origin arena/01a010af-c-class
bash enable-ci.sh
```

اسکریپت `enable-ci.sh` فایل ورک‌فلو را از `ci/` به `.github/workflows/` کپی، کامیت و push می‌کند.
(اگر Windows/PowerShell دارید: سه خط زیر معادل آن است)

```powershell
mkdir .github\workflows -Force
copy ci\build-exe.yml .github\workflows\build-exe.yml
git add .github\workflows\build-exe.yml; git commit -m "Enable Windows exe build workflow"; git push origin arena/01a010af-c-class
```

گزینهٔ دیگر: در Arena اتصال GitHub را با دسترسی `workflows` دوباره وصل کنید، بعد به من بگویید تا خودم push کنم.

---

## بعد از فعال‌سازی چه اتفاقی می‌افتد

ورک‌فلو روی `windows-latest` اجرا می‌شود و:

1. .NET 10 SDK را نصب می‌کند
2. `dotnet restore` و سپس publish تک‌فایلی self-contained برای `win-x64`
3. خروجی را در دو جای قابل دانلود می‌گذارد:

| کجا | لینک |
|---|---|
| **Artifact** با نام `DentalCenter-win-x64` | https://github.com/rwm540/C--class/actions |
| **Release** با تگ `win-build-<شماره>` | https://github.com/rwm540/C--class/releases |

فایل نهایی: **`DentalCenter.exe`** — تک‌فایلی، self-contained (روی مقصد نیازی به نصب .NET نیست، حدود ۷۰–۹۰ مگابایت).

دستور publish که اجرا می‌شود:

```
dotnet publish DentalCenter.csproj -c Release -r win-x64 --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=none -o publish
```

---

## چرا اینجا (در سندباکس) بیلد نشد

شبکهٔ محیط اجرای ایجنت این دامنه‌ها را مسدود کرده:

- `dot.net`, `builds.dotnet.microsoft.com`, `download.visualstudio.microsoft.com` → نصب .NET SDK ناممکن
- `api.nuget.org`, `nuget.org` و میرورها (Tencent/Huawei/Azure CN) → restore پکیج‌های Avalonia ناممکن
- مخازن apt دبیان نیز در دسترس نیستند

فقط `github.com` (git)، PyPI و npm باز هستند و هیچ‌کدام .NET SDK ندارند. ضمناً ویندوز هم لازم است
تا exe با manifest درست ساخته شود — به همین دلیل GitHub Actions با runner ویندوزی راه درست است.

---

## روش جایگزین: بیلد محلی روی ویندوز

پیش‌نیاز: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

```powershell
git clone https://github.com/rwm540/C--class.git
cd C--class
./build-exe.ps1
```

خروجی در `publish\DentalCenter.exe`.

### حالت‌های دیگر
| هدف | تغییر در دستور |
|---|---|
| exe سبک (نیازمند .NET 10 روی مقصد) | `--self-contained false` |
| ویندوز ARM64 | `-r win-arm64` |
| حجم کمتر | افزودن `-p:PublishTrimmed=true` (برای Avalonia تست شود) |
| بیلد از لینوکس/مک برای ویندوز | افزودن `-p:EnableWindowsTargeting=true` |

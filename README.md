# Dental Center

برنامهٔ دسکتاپ (Avalonia UI / .NET 10) برای ارائهٔ پروژهٔ
**«طراحی ساختمان مرکز دندانپزشکی با رویکرد بهره‌وری انرژی»**.

## بخش‌های برنامه

| بخش | توضیح |
|---|---|
| 🏠 صفحه اصلی | معرفی پروژه، آمار کلیدی و شناسنامهٔ پروژه |
| 🦷 تجهیزات | یونیت، رادیوگرافی، تهیه مواد، کمپرسور و اتوکلاو — با نکات طراحی، جدول مشخصات فنی و نکتهٔ انرژی |
| 🏢 فضای فیزیکی | اتاق درمان، انتظار، پذیرش، تصویربرداری و سرویس بهداشتی |
| 💡 بهره‌وری انرژی | روشنایی، سرمایش، گرمایش، تهویه، پوستهٔ ساختمان و انرژی خورشیدی |
| 👶 بخش کودکان | فضا و رنگ، فضای بازی، ایمنی و ارگونومی |
| 🧮 محاسبهٔ انرژی | ابزار تعاملی برآورد مصرف سالانه و صرفه‌جویی ناشی از راهکارها |
| ☎ تماس با ما | اطلاعات تماس با قابلیت کپی در کلیپ‌بورد |
| 📝 ثبت نظرات | ثبت نظر با امتیاز ستاره‌ای، ذخیره روی دیسک و نمایش فهرست نظرات |

امکانات عمومی: چیدمان کاملاً راست‌به‌چپ، فونت فارسی **وزیرمتن** جاسازی‌شده در خود برنامه
(روی سیستم مقصد نیازی به نصب فونت نیست)، پوستهٔ روشن/تیره، و مقاوم بودن در برابر نبود فایل تصویر یا PDF.

## دریافت فایل اجرایی آماده

هر push روی این مخزن یک بیلد ویندوزی می‌سازد:

- **Artifacts:** <https://github.com/rwm540/C--class/actions> → آخرین اجرا → `DentalCenter-win-x64`
- **Releases:** <https://github.com/rwm540/C--class/releases> → آخرین `win-build-*`

فایل `DentalCenter.exe` تک‌فایل و self-contained است؛ روی سیستم مقصد نصب .NET لازم نیست.

## ساخت روی سیستم خودتان (ویندوز)

پیش‌نیاز: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

```powershell
git clone https://github.com/rwm540/C--class.git
cd C--class
./build-exe.ps1
```

خروجی: `publish\DentalCenter.exe`

یا در Visual Studio: راست‌کلیک روی پروژه → **Publish** → پروفایل `win-x64`.

> فقط فایل exe داخل `bin\Debug` را جدا کپی نکنید؛ آن نسخه بدون DLLهای کنارش اجرا نمی‌شود.
> نسخهٔ درست همان `publish\DentalCenter.exe` است.

## ساختار پروژه

```
Assets/           تصاویر، فونت وزیرمتن و پوشهٔ PDF (همه داخل exe جاسازی می‌شوند)
Data/             محتوای علمی و متن‌های برنامه (ContentData.cs, ContactData.cs)
Models/           مدل‌های داده (Topic, Spec, Feedback)
Services/         محاسبهٔ انرژی و ذخیرهٔ نظرات
Controls/         کنترل مشترک نمایش جزئیات (DetailPageView)
Views/            صفحات برنامه
Helpers/          بارگذاری تصویر و باز کردن PDF
```

### ویرایش متن‌ها

برای تغییر نام دانشجو، استاد راهنما، دانشگاه و همهٔ متن‌های علمی فقط فایل
`Data/ContentData.cs` را ویرایش کنید. اطلاعات تماس در `Data/ContactData.cs` است.

### افزودن PDF

فایل‌ها را با همین نام‌ها در `Assets/PDF/` بگذارید — دکمهٔ «مشاهده PDF» به‌صورت خودکار فعال می‌شود:

```
unit.pdf  radio.pdf  material.pdf  compressor.pdf  autoclave.pdf
room.pdf  waiting.pdf  reception.pdf  imaging.pdf  service.pdf
light.pdf cooling.pdf heating.pdf ventilation.pdf window.pdf solar.pdf
```

## محل ذخیرهٔ داده‌های کاربر

```
%LOCALAPPDATA%\DentalCenter\feedback.json    نظرات ثبت‌شده
%LOCALAPPDATA%\DentalCenter\crash.log        گزارش خطا در صورت بروز مشکل
```

## مجوز فونت

فونت وزیرمتن تحت مجوز SIL Open Font License است — متن مجوز در `Docs/Vazirmatn-OFL.txt`.

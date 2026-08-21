# Dental Center

برنامهٔ دسکتاپ (Avalonia UI / .NET 10) برای ارائهٔ پروژهٔ
**«طراحی ساختمان مرکز دندانپزشکی با رویکرد بهره‌وری انرژی»**.

## بخش‌های برنامه

| بخش | توضیح |
|---|---|
| 🏠 صفحه اصلی | معرفی پروژه، آمار کلیدی و شناسنامهٔ پروژه |
| 🦷 تجهیزات | یونیت، رادیوگرافی، تهیه مواد، کمپرسور و اتوکلاو |
| 🏢 فضای فیزیکی | اتاق درمان، انتظار، پذیرش، تصویربرداری و سرویس بهداشتی |
| 💡 بهره‌وری انرژی | روشنایی، سرمایش، گرمایش، تهویه، پوستهٔ ساختمان، انرژی خورشیدی و منابع |
| 👶 بخش کودکان | فضا، رنگ و طراحی دیوارها، فضای بازی، نگهداری وسایل بازی، ایمنی و ارگونومی |
| 🧮 محاسبهٔ انرژی | ابزار تعاملی برآورد مصرف سالانه و صرفه‌جویی ناشی از راهکارها |
| ☎ تماس با ما | اطلاعات تماس با قابلیت کپی + فرم ارسال پیام با ذخیره در دیتابیس |
| 📝 ثبت نظرات | ثبت نظر با امتیاز ستاره‌ای و ذخیره در پایگاه دادهٔ SQLite |
| 💬 نظرات و پیام‌ها | مرور نظرها با جست‌وجو، فیلتر امتیاز و نمودار توزیع + مشاهدهٔ پیام‌های فرم تماس |

امکانات عمومی: چیدمان کاملاً راست‌به‌چپ، فونت فارسی **وزیرمتن** جاسازی‌شده در خود برنامه
(روی سیستم مقصد نیازی به نصب فونت نیست)، پوستهٔ روشن/تیره، و مقاوم بودن در برابر نبود فایل تصویر یا PDF.
هر زیرمنو صفحهٔ تصویر جداگانه و غیرتکراری خودش را دارد و در برنامه فقط «خلاصهٔ موضوع»،
«تصویر» و «نکات کلیدی طراحی» را نشان می‌دهد. سه بخش «مشخصات فنی»، «واژگان کلیدی» و
«نکتهٔ بهره‌وری انرژی» از صفحه‌های برنامه حذف شده‌اند و فقط داخل فایل PDF همان موضوع
(دکمهٔ «مشاهده PDF») قرار دارند.

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
Services/         پایگاه داده SQLite، محاسبهٔ انرژی، ذخیرهٔ نظرات و پیام‌ها
Controls/         کنترل مشترک نمایش جزئیات (DetailPageView)
Views/            صفحات برنامه
Helpers/          بارگذاری تصویر و باز کردن PDF
tools/            ساخت PDFها (generate_pdfs.py) و متن جزئیات فنی (pdf_extras.json)
```

### ویرایش متن‌ها

برای تغییر نام دانشجو، استاد راهنما، دانشگاه و همهٔ متن‌های علمی فقط فایل
`Data/ContentData.cs` را ویرایش کنید. اطلاعات تماس در `Data/ContactData.cs` است.

### جزئیات فنی و فایل‌های PDF

«مشخصات فنی»، «واژگان کلیدی» و «نکتهٔ بهره‌وری انرژی» در رابط کاربری نمایش داده نمی‌شوند؛
متن آن‌ها در `tools/pdf_extras.json` نگهداری می‌شود و اسکریپت زیر آن را با محتوای
`Data/ContentData.cs` (عنوان، خلاصه و نکات کلیدی) ترکیب کرده و برای هر موضوع یک PDF فارسی
می‌سازد:

```bash
pip install fpdf2 uharfbuzz
python3 tools/generate_pdfs.py     # خروجی در Assets/PDF/
```

برای تغییر این جزئیات فقط `tools/pdf_extras.json` را ویرایش و دوباره اسکریپت را اجرا کنید.

### افزودن PDF

فایل‌ها را با همین نام‌ها در `Assets/PDF/` بگذارید — دکمهٔ «مشاهده PDF» به‌صورت خودکار فعال می‌شود:

```
unit.pdf  radio.pdf  material.pdf  compressor.pdf  autoclave.pdf
room.pdf  waiting.pdf  reception.pdf  imaging.pdf  service.pdf
light.pdf cooling.pdf heating.pdf ventilation.pdf window.pdf solar.pdf
sources.pdf
childspace.pdf  childplay.pdf  childsafety.pdf  childstorage.pdf
```

## پایگاه داده

برنامه از **SQLite** استفاده می‌کند (پکیج `Microsoft.Data.Sqlite`). فایل دیتابیس در اولین
اجرا خودکار ساخته می‌شود و نیازی به نصب چیزی روی سیستم مقصد نیست.

```
%LOCALAPPDATA%\DentalCenter\dentalcenter.db   پایگاه داده
%LOCALAPPDATA%\DentalCenter\crash.log         گزارش خطا در صورت بروز مشکل
```

### جدول‌ها

**`Feedback`** — نظرهای ثبت‌شده

| ستون | نوع | توضیح |
|---|---|---|
| Id | INTEGER | کلید اصلی، خودافزا |
| Name | TEXT | نام کاربر |
| Email | TEXT | ایمیل (اختیاری) |
| Subject | TEXT | موضوع نظر |
| Rating | INTEGER | امتیاز ۱ تا ۵ |
| Message | TEXT | متن نظر |
| CreatedAt | TEXT | تاریخ ثبت (`yyyy-MM-dd HH:mm:ss`) |

**`ContactMessage`** — پیام‌های فرم تماس

| ستون | نوع | توضیح |
|---|---|---|
| Id | INTEGER | کلید اصلی، خودافزا |
| Name / Email / Phone | TEXT | مشخصات فرستنده |
| Subject | TEXT | موضوع پیام |
| Message | TEXT | متن پیام |
| CreatedAt | TEXT | تاریخ ثبت |

همهٔ کوئری‌ها **پارامتری** هستند (بدون الحاق رشته) تا در برابر SQL Injection ایمن باشند.

> اگر از نسخه‌های قبلی برنامه فایل `feedback.json` داشته باشید، در اولین اجرا نظرها
> خودکار به دیتابیس منتقل و فایل قدیمی به `feedback.json.migrated` تغییر نام می‌دهد.

## مجوز فونت

فونت وزیرمتن تحت مجوز SIL Open Font License است — متن مجوز در `Docs/Vazirmatn-OFL.txt`.

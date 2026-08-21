# -*- coding: utf-8 -*-
"""
سازندهٔ مستند فنی/پژوهشی نرم‌افزار Dental Center.

خروجی: info/DentalCenter-Documentation.pdf  (فارسی، راست‌به‌چپ، ۱۲۰ صفحه)

اجرا از ریشهٔ مخزن:
    pip install fpdf2 uharfbuzz
    python3 tools/generate_info_pdf.py
"""
import os
import sys

from fpdf import FPDF
from fpdf.enums import XPos, YPos

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
sys.path.insert(0, os.path.join(ROOT, "tools"))

from infodoc import content  # noqa: E402

OUT_DIR = os.path.join(ROOT, "info")
OUT_FILE = os.path.join(OUT_DIR, "DentalCenter-Documentation.pdf")
REGULAR = os.path.join(ROOT, "Assets", "Fonts", "Vazirmatn-Regular.ttf")
BOLD = os.path.join(ROOT, "Assets", "Fonts", "Vazirmatn-Bold.ttf")
FALLBACK = "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf"
MONO = "/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf"

TARGET_PAGES = 120

BRAND = (13, 71, 161)
BRAND_DARK = (8, 46, 110)
BRAND_LIGHT = (227, 242, 253)
GREEN = (27, 94, 32)
GREEN_BG = (232, 245, 233)
GREEN_BORDER = (165, 214, 167)
AMBER_BG = (255, 248, 225)
AMBER_BORDER = (255, 224, 130)
AMBER = (132, 90, 0)
GRAY = (110, 122, 138)
TEXT = (33, 41, 52)
CODE_BG = (245, 247, 250)
CODE_BORDER = (215, 222, 232)

PERSIAN_DIGITS = str.maketrans("0123456789", "۰۱۲۳۴۵۶۷۸۹")


def fa(value):
    """تبدیل ارقام لاتین به فارسی."""
    return str(value).translate(PERSIAN_DIGITS)


class InfoPdf(FPDF):
    """قالب صفحه‌آرایی مستند."""

    def __init__(self, scale=1.0):
        super().__init__(format="A4")
        self.scale = scale
        self.doc_title = content.DOC_TITLE
        self.current_chapter = ""
        self.cover_mode = True
        self.section_pages = {}
        self.last_level = -1

    # ---------------------------------------------------------------- سربرگ
    def header(self):
        if self.cover_mode or self.page_no() == 1:
            return
        self.set_y(8)
        self.set_font("vazir", "", 8.5)
        self.set_text_color(*GRAY)
        w = self.w - self.l_margin - self.r_margin
        self.set_x(self.l_margin)
        self.cell(w / 2, 5, self.current_chapter or self.doc_title, align="R")
        self.cell(w / 2, 5, content.PROJECT_TITLE, align="L")
        self.ln(5)
        self.set_draw_color(215, 222, 232)
        self.set_line_width(0.2)
        self.line(self.l_margin, self.get_y(), self.w - self.r_margin, self.get_y())
        self.set_y(self.t_margin)

    def footer(self):
        if self.cover_mode or self.page_no() == 1:
            return
        self.set_y(-13)
        self.set_draw_color(215, 222, 232)
        self.line(self.l_margin, self.get_y(), self.w - self.r_margin, self.get_y())
        self.ln(1)
        self.set_font("vazir", "", 8.5)
        self.set_text_color(*GRAY)
        w = self.w - self.l_margin - self.r_margin
        self.set_x(self.l_margin)
        self.cell(w / 2, 6, fa(self.page_no()), align="R")
        self.cell(w / 2, 6, content.STUDENT + " — " + content.UNIVERSITY, align="L")

    # -------------------------------------------------------------- ابزارها
    @property
    def content_width(self):
        return self.w - self.l_margin - self.r_margin

    def space(self, mm):
        self.ln(mm * self.scale)

    def keep(self, needed_mm):
        """اگر فضای کافی تا انتهای صفحه نمانده، صفحهٔ جدید باز کن."""
        if self.get_y() + needed_mm > self.h - self.b_margin:
            self.add_page()


def sec(pdf, title, level):
    """ثبت عنوان در فهرست مطالب با رعایت سلسله‌مراتب."""
    lvl = min(level, pdf.last_level + 1)
    pdf.start_section(title, level=lvl)
    pdf.last_level = lvl


def register_fonts(pdf):
    pdf.add_font("vazir", "", REGULAR)
    pdf.add_font("vazir", "B", BOLD)
    if os.path.exists(MONO):
        pdf.add_font("mono", "", MONO)
    fallbacks = []
    if os.path.exists(FALLBACK):
        pdf.add_font("fallback", "", FALLBACK)
        fallbacks.append("fallback")
    if fallbacks:
        pdf.set_fallback_fonts(fallbacks)


# ====================================================================== جلد
def render_cover(pdf):
    pdf.cover_mode = True
    pdf.add_page()

    pdf.set_fill_color(*BRAND)
    pdf.rect(0, 0, pdf.w, 78, style="F")
    pdf.set_fill_color(*BRAND_DARK)
    pdf.rect(0, 78, pdf.w, 3, style="F")

    pdf.set_xy(20, 20)
    pdf.set_font("vazir", "", 12)
    pdf.set_text_color(190, 214, 245)
    pdf.cell(pdf.w - 40, 8, content.UNIVERSITY, align="R")
    pdf.ln(10)
    pdf.set_x(20)
    pdf.set_font("vazir", "B", 20)
    pdf.set_text_color(255, 255, 255)
    pdf.multi_cell(pdf.w - 40, 11, content.DOC_TITLE, align="R")
    pdf.ln(2)
    pdf.set_x(20)
    pdf.set_font("vazir", "", 11.5)
    pdf.set_text_color(205, 225, 250)
    pdf.multi_cell(pdf.w - 40, 7, content.DOC_SUBTITLE, align="R")

    pdf.set_y(96)
    pdf.set_x(20)
    pdf.set_font("vazir", "B", 15)
    pdf.set_text_color(*BRAND_DARK)
    pdf.multi_cell(pdf.w - 40, 9, content.PROJECT_TITLE, align="R")

    pdf.ln(4)
    pdf.set_x(20)
    pdf.set_font("vazir", "", 11)
    pdf.set_text_color(*TEXT)
    pdf.multi_cell(pdf.w - 40, 7.5, content.COVER_BLURB, align="R")

    img = os.path.join(ROOT, "Assets", "Images", "clinic.jpg")
    if os.path.exists(img):
        pdf.image(img, x=20, y=pdf.get_y() + 6, w=pdf.w - 40, h=62)
        pdf.set_y(pdf.get_y() + 72)

    y = max(pdf.get_y() + 6, 196)
    pdf.set_y(y)
    pdf.set_fill_color(*BRAND_LIGHT)
    pdf.set_draw_color(*BRAND_LIGHT)
    rows = [
        ("نگارنده", content.STUDENT),
        ("استاد راهنما", content.SUPERVISOR),
        ("دانشکده / رشته", content.FACULTY),
        ("نسخهٔ نرم‌افزار", content.APP_VERSION),
        ("تعداد فصل‌ها", fa(5) + " فصل"),
        ("سال", content.YEAR),
    ]
    pdf.set_font("vazir", "", 11)
    col = (pdf.w - 40) / 2
    for key, value in rows:
        pdf.set_x(20)
        pdf.set_text_color(*BRAND_DARK)
        pdf.set_font("vazir", "B", 10.5)
        pdf.cell(col, 8.6, value, border=0, fill=True, align="R")
        pdf.set_text_color(*GRAY)
        pdf.set_font("vazir", "", 10.5)
        pdf.cell(col, 8.6, key, border=0, fill=True, align="L")
        pdf.ln(9.4)

    pdf.set_y(-24)
    pdf.set_font("vazir", "", 9.5)
    pdf.set_text_color(*GRAY)
    pdf.cell(0, 6, content.COVER_FOOTER, align="C")
    pdf.cover_mode = False


# ================================================================ فهرست مطالب
def render_toc(pdf, outline):
    start_page = pdf.page_no()
    pdf.current_chapter = "فهرست مطالب"
    pdf.set_xy(pdf.l_margin, pdf.t_margin)
    pdf.set_font("vazir", "B", 18)
    pdf.set_text_color(*BRAND_DARK)
    pdf.multi_cell(0, 11, "فهرست مطالب", align="R",
                   new_x=XPos.LMARGIN, new_y=YPos.NEXT)
    pdf.set_draw_color(*BRAND)
    pdf.set_line_width(0.6)
    pdf.line(pdf.l_margin, pdf.get_y() + 1, pdf.w - pdf.r_margin, pdf.get_y() + 1)
    pdf.set_line_width(0.2)
    pdf.ln(6)

    for entry in outline:
        if entry.level > 1:
            continue
        if pdf.get_y() > pdf.h - pdf.b_margin - 12:
            pdf.add_page()
            pdf.set_xy(pdf.l_margin, pdf.t_margin)

        indent = {0: 0, 1: 8, 2: 16}.get(entry.level, 16)
        if entry.level == 0:
            pdf.ln(2.5)
            pdf.set_font("vazir", "B", 12)
            pdf.set_text_color(*BRAND_DARK)
            line_h = 8.5
        elif entry.level == 1:
            pdf.set_font("vazir", "", 10.5)
            pdf.set_text_color(*TEXT)
            line_h = 6.6
        else:
            pdf.set_font("vazir", "", 9.5)
            pdf.set_text_color(*GRAY)
            line_h = 6.0

        page_txt = fa(entry.page_number)
        width = pdf.content_width - indent
        pdf.set_x(pdf.l_margin)
        pdf.cell(12, line_h, page_txt, align="L")
        pdf.set_x(pdf.l_margin + 12)
        pdf.cell(width - 12, line_h, entry.name, align="R",
                 new_x=XPos.LMARGIN, new_y=YPos.NEXT)

    # تکمیل صفحه‌های رزروشده تا تعداد اعلام‌شده
    while pdf.page_no() - start_page + 1 < content.TOC_PAGES:
        pdf.add_page()


# ================================================================ نمودارها
def draw_layers(pdf, width, height):
    """نمودار معماری لایه‌ای."""
    x0 = pdf.l_margin
    y0 = pdf.get_y()
    layers = [
        ("لایهٔ نمایش  —  Views / Controls (Avalonia XAML)", BRAND),
        ("لایهٔ کنترل  —  Code-behind، ناوبری MainWindow", (21, 101, 192)),
        ("لایهٔ سرویس  —  EnergyCalculator، FeedbackStore، ContactStore", (25, 118, 210)),
        ("لایهٔ داده  —  ContentData، Models، SQLite (Database.cs)", (66, 145, 220)),
        ("لایهٔ منابع  —  Assets: تصاویر، فونت وزیرمتن، فایل‌های PDF", (120, 170, 225)),
    ]
    lh = (height - 4 * 3) / len(layers)
    for label, color in layers:
        pdf.set_fill_color(*color)
        pdf.set_draw_color(*color)
        pdf.rect(x0, y0, width, lh, style="F")
        pdf.set_xy(x0, y0 + lh / 2 - 3.4)
        pdf.set_font("vazir", "B", 9.5)
        pdf.set_text_color(255, 255, 255)
        pdf.cell(width - 6, 7, label, align="R")
        y0 += lh + 3
    pdf.set_y(y0)


def draw_navmap(pdf, width, height):
    """نقشهٔ ناوبری برنامه."""
    x0 = pdf.l_margin
    y0 = pdf.get_y()
    pdf.set_fill_color(*BRAND_DARK)
    pdf.rect(x0 + width / 4, y0, width / 2, 11, style="F")
    pdf.set_xy(x0 + width / 4, y0 + 2)
    pdf.set_font("vazir", "B", 9.5)
    pdf.set_text_color(255, 255, 255)
    pdf.cell(width / 2, 7, "پنجرهٔ اصلی — MainWindow", align="C")

    items = [
        "🏠 صفحه اصلی", "🦷 تجهیزات", "🏢 فضای فیزیکی",
        "💡 بهره‌وری انرژی", "👶 بخش کودکان", "🧮 محاسبهٔ انرژی",
        "☎ تماس با ما", "📝 ثبت نظرات", "💬 نظرات و پیام‌ها",
    ]
    cols, gap = 3, 4
    bw = (width - (cols - 1) * gap) / cols
    bh = (height - 20 - 2 * gap) / 3
    top = y0 + 20
    pdf.set_draw_color(*BRAND)
    pdf.set_line_width(0.3)
    pdf.line(x0 + width / 2, y0 + 11, x0 + width / 2, top - 3)
    for i, item in enumerate(items):
        r, c = divmod(i, cols)
        bx = x0 + (cols - 1 - c) * (bw + gap)
        by = top + r * (bh + gap)
        pdf.set_fill_color(*BRAND_LIGHT)
        pdf.rect(bx, by, bw, bh, style="DF")
        pdf.set_xy(bx, by + bh / 2 - 3)
        pdf.set_font("vazir", "", 9)
        pdf.set_text_color(*BRAND_DARK)
        pdf.cell(bw, 6, item, align="C")
    pdf.set_y(top + 3 * bh + 2 * gap)


def draw_pipeline(pdf, width, height):
    """خط تولید محتوا تا PDF."""
    steps = [
        "ContentData.cs\nمتن علمی",
        "pdf_extras.json\nجزئیات فنی",
        "generate_pdfs.py\nموتور تولید",
        "Assets/PDF\n۲۱ فایل PDF",
        "دکمهٔ مشاهده PDF\nدر برنامه",
    ]
    x0 = pdf.l_margin
    y0 = pdf.get_y()
    gap = 6
    bw = (width - (len(steps) - 1) * gap) / len(steps)
    bh = height
    for i, step in enumerate(steps):
        bx = x0 + (len(steps) - 1 - i) * (bw + gap)
        pdf.set_fill_color(*(BRAND_LIGHT if i % 2 == 0 else (232, 245, 233)))
        pdf.set_draw_color(*(BRAND if i % 2 == 0 else GREEN_BORDER))
        pdf.rect(bx, y0, bw, bh, style="DF")
        pdf.set_xy(bx, y0 + bh / 2 - 6)
        pdf.set_font("vazir", "", 8.2)
        pdf.set_text_color(*BRAND_DARK)
        pdf.multi_cell(bw, 5, step, align="C")
        if i < len(steps) - 1:
            ax = bx - gap
            pdf.set_draw_color(*GRAY)
            pdf.line(ax + 1, y0 + bh / 2, ax + gap - 1, y0 + bh / 2)
            pdf.line(ax + 1, y0 + bh / 2, ax + 2.5, y0 + bh / 2 - 1.5)
            pdf.line(ax + 1, y0 + bh / 2, ax + 2.5, y0 + bh / 2 + 1.5)
    pdf.set_y(y0 + bh)


def draw_erd(pdf, width, height):
    """نمودار جدول‌های پایگاه داده."""
    tables = [
        ("Feedback", ["Id  INTEGER PK", "Name  TEXT", "Email  TEXT",
                      "Subject  TEXT", "Rating  INTEGER", "Message  TEXT",
                      "CreatedAt  TEXT"]),
        ("ContactMessage", ["Id  INTEGER PK", "Name  TEXT", "Email  TEXT",
                            "Phone  TEXT", "Subject  TEXT", "Message  TEXT",
                            "CreatedAt  TEXT"]),
    ]
    x0 = pdf.l_margin
    y0 = pdf.get_y()
    bw = (width - 12) / 2
    for i, (name, cols) in enumerate(tables):
        bx = x0 + i * (bw + 12)
        pdf.set_fill_color(*BRAND)
        pdf.rect(bx, y0, bw, 9, style="F")
        pdf.set_xy(bx, y0 + 1)
        pdf.set_font("vazir", "B", 10)
        pdf.set_text_color(255, 255, 255)
        pdf.cell(bw, 7, name, align="C")
        yy = y0 + 9
        pdf.set_font("mono" if os.path.exists(MONO) else "vazir", "", 8.5)
        pdf.set_draw_color(*CODE_BORDER)
        for j, col in enumerate(cols):
            pdf.set_fill_color(*(CODE_BG if j % 2 == 0 else (255, 255, 255)))
            pdf.rect(bx, yy, bw, 7, style="DF")
            pdf.set_xy(bx + 3, yy)
            pdf.set_text_color(*TEXT)
            with pdf.local_context(text_shaping=False):
                pdf.cell(bw - 6, 7, col, align="L")
            yy += 7
    pdf.set_y(y0 + 9 + 7 * 7 + 2)


def draw_energy_chart(pdf, width, height):
    """نمودار میله‌ای تفکیک مصرف سناریوی نمونه."""
    rows = content.ENERGY_CHART_ROWS
    x0 = pdf.l_margin
    y0 = pdf.get_y()
    top_val = max(max(a, b) for _, a, b in rows) or 1
    row_h = height / len(rows)
    label_w = 42
    bar_area = width - label_w - 26
    for i, (label, base, opt) in enumerate(rows):
        yy = y0 + i * row_h
        pdf.set_xy(pdf.w - pdf.r_margin - label_w, yy + row_h / 2 - 4)
        pdf.set_font("vazir", "", 8.5)
        pdf.set_text_color(*TEXT)
        pdf.cell(label_w - 3, 6, label, align="R")
        bx = x0 + 26
        pdf.set_fill_color(200, 214, 232)
        pdf.rect(bx, yy + 1.5, bar_area * base / top_val, row_h / 2 - 2, style="F")
        pdf.set_fill_color(*BRAND)
        pdf.rect(bx, yy + row_h / 2 + 0.5, bar_area * opt / top_val, row_h / 2 - 2, style="F")
        pdf.set_xy(x0, yy + row_h / 2 - 4)
        pdf.set_font("vazir", "", 7.5)
        pdf.set_text_color(*GRAY)
        pdf.cell(24, 6, fa(f"{int(opt):,}".replace(",", "٬")), align="L")
    pdf.set_y(y0 + height + 1)
    pdf.set_font("vazir", "", 8)
    pdf.set_text_color(*GRAY)
    pdf.cell(0, 5, "میلهٔ روشن: سناریوی پایه   |   میلهٔ پررنگ: سناریوی بهینه (کیلووات‌ساعت در سال)",
             align="R", new_x=XPos.LMARGIN, new_y=YPos.NEXT)




def draw_wire_main(pdf, width, height):
    """طرح‌وارهٔ پنجرهٔ اصلی برنامه."""
    x0, y0 = pdf.l_margin, pdf.get_y()
    pdf.set_draw_color(*BRAND)
    pdf.set_line_width(0.3)
    pdf.set_fill_color(255, 255, 255)
    pdf.rect(x0, y0, width, height, style="DF")

    # سربرگ
    pdf.set_fill_color(*BRAND)
    pdf.rect(x0, y0, width, 14, style="F")
    pdf.set_xy(x0, y0 + 3)
    pdf.set_font("vazir", "B", 9)
    pdf.set_text_color(255, 255, 255)
    pdf.cell(width - 5, 8, "عنوان پروژه  +  زیرعنوان", align="R")
    pdf.set_xy(x0 + 4, y0 + 3)
    pdf.set_font("vazir", "", 8)
    pdf.cell(24, 8, "کلید پوسته", align="L")

    # ستون ناوبری
    nav_w = width * 0.24
    pdf.set_fill_color(*BRAND_LIGHT)
    pdf.set_draw_color(*BRAND_LIGHT)
    pdf.rect(x0 + width - nav_w, y0 + 16, nav_w - 2, height - 26, style="F")
    pdf.set_font("vazir", "", 8)
    pdf.set_text_color(*BRAND_DARK)
    labels = ["صفحه اصلی", "تجهیزات", "فضای فیزیکی", "بهره‌وری انرژی", "بخش کودکان",
              "محاسبهٔ انرژی", "تماس با ما", "ثبت نظرات", "نظرات و پیام‌ها"]
    yy = y0 + 19
    for label in labels:
        pdf.set_xy(x0 + width - nav_w, yy)
        pdf.cell(nav_w - 5, 5.4, label, align="R")
        yy += 5.6

    # ناحیهٔ محتوا
    pdf.set_draw_color(180, 195, 215)
    pdf.set_fill_color(250, 251, 253)
    pdf.rect(x0 + 3, y0 + 16, width - nav_w - 6, height - 26, style="DF")
    pdf.set_xy(x0 + 3, y0 + 16 + (height - 26) / 2 - 4)
    pdf.set_font("vazir", "", 9)
    pdf.set_text_color(*GRAY)
    pdf.cell(width - nav_w - 6, 8, "ناحیهٔ محتوا (صفحهٔ انتخاب‌شده)", align="C")

    # پانویس
    pdf.set_xy(x0, y0 + height - 9)
    pdf.set_font("vazir", "", 7.5)
    pdf.cell(width - 4, 6, "نام دانشجو — سال تحصیلی", align="R")
    pdf.set_y(y0 + height)


def draw_wire_detail(pdf, width, height):
    """طرح‌وارهٔ صفحهٔ جزئیات یک موضوع."""
    x0, y0 = pdf.l_margin, pdf.get_y()
    pdf.set_draw_color(180, 195, 215)
    pdf.set_line_width(0.3)
    pdf.set_fill_color(255, 255, 255)
    pdf.rect(x0, y0, width, height, style="DF")

    list_w = width * 0.26
    pdf.set_fill_color(*BRAND_LIGHT)
    pdf.set_draw_color(*BRAND_LIGHT)
    pdf.rect(x0 + width - list_w, y0 + 3, list_w - 3, height - 6, style="F")
    pdf.set_font("vazir", "B", 8)
    pdf.set_text_color(*BRAND_DARK)
    pdf.set_xy(x0 + width - list_w, y0 + 5)
    pdf.cell(list_w - 6, 6, "فهرست موضوع‌های بخش", align="R")
    yy = y0 + 13
    for i in range(6):
        pdf.set_draw_color(160, 190, 230)
        pdf.set_fill_color(255, 255, 255) if i else pdf.set_fill_color(*BRAND)
        pdf.rect(x0 + width - list_w + 2, yy, list_w - 9, 6, style="DF")
        yy += 8

    inner_x = x0 + 4
    inner_w = width - list_w - 8
    parts = [
        (9, "عنوان موضوع", BRAND_DARK, True),
        (8, "خلاصهٔ موضوع در یک تا دو سطر", GRAY, False),
        (26, "تصویر اختصاصی موضوع", GRAY, False),
        (22, "نکات کلیدی طراحی (فهرست نقطه‌ای)", TEXT, False),
        (18, "کارت راهنما + دکمهٔ مشاهده PDF", BRAND_DARK, False),
    ]
    yy = y0 + 5
    for h, label, color, bold in parts:
        pdf.set_draw_color(190, 205, 222)
        pdf.set_fill_color(250, 251, 253)
        pdf.rect(inner_x, yy, inner_w, h, style="DF")
        pdf.set_xy(inner_x, yy + h / 2 - 3)
        pdf.set_font("vazir", "B" if bold else "", 8.4)
        pdf.set_text_color(*color)
        pdf.cell(inner_w - 3, 6, label, align="R")
        yy += h + 2.5
    pdf.set_y(y0 + height)


DIAGRAMS = {
    "layers": draw_layers,
    "wire_main": draw_wire_main,
    "wire_detail": draw_wire_detail,
    "navmap": draw_navmap,
    "pipeline": draw_pipeline,
    "erd": draw_erd,
    "energy": draw_energy_chart,
}


# ============================================================= اجزای محتوا
def block_h2(pdf, text):
    pdf.keep(24)
    pdf.space(4)
    sec(pdf, text, 1)
    pdf.set_font("vazir", "B", 14.2)
    pdf.set_text_color(*BRAND)
    pdf.multi_cell(0, 8, text, align="R", new_x=XPos.LMARGIN, new_y=YPos.NEXT)
    pdf.set_draw_color(*BRAND)
    pdf.set_line_width(0.4)
    y = pdf.get_y() + 0.5
    pdf.line(pdf.l_margin, y, pdf.w - pdf.r_margin, y)
    pdf.set_line_width(0.2)
    pdf.space(3.5)


def block_h3(pdf, text):
    pdf.keep(20)
    pdf.space(2.5)
    sec(pdf, text, 2)
    pdf.set_font("vazir", "B", 12.2)
    pdf.set_text_color(*BRAND_DARK)
    pdf.multi_cell(0, 7.2, text, align="R", new_x=XPos.LMARGIN, new_y=YPos.NEXT)
    pdf.space(1.5)


def block_h4(pdf, text):
    pdf.keep(16)
    pdf.space(1.5)
    pdf.set_font("vazir", "B", 11.2)
    pdf.set_text_color(*TEXT)
    pdf.multi_cell(0, 6.8, text, align="R", new_x=XPos.LMARGIN, new_y=YPos.NEXT)
    pdf.space(1)


def block_p(pdf, text):
    pdf.set_font("vazir", "", 12.2)
    pdf.set_text_color(*TEXT)
    pdf.multi_cell(0, 7.2 * pdf.scale, text, align="J",
                   new_x=XPos.LMARGIN, new_y=YPos.NEXT)
    pdf.space(2.4)


def block_pen(pdf, text):
    """پاراگراف انگلیسی (چپ‌به‌راست)."""
    pdf.set_font("vazir", "", 11.2)
    pdf.set_text_color(*TEXT)
    with pdf.local_context(text_shaping=True):
        pdf.set_text_shaping(use_shaping_engine=True, direction="ltr")
        pdf.multi_cell(0, 6.9 * pdf.scale, text, align="L",
                       new_x=XPos.LMARGIN, new_y=YPos.NEXT)
    pdf.set_text_shaping(use_shaping_engine=True, direction="rtl")
    pdf.space(2.4)


def block_ul(pdf, items):
    pdf.set_font("vazir", "", 11.5)
    pdf.set_text_color(*TEXT)
    for item in items:
        pdf.keep(12)
        y = pdf.get_y()
        pdf.set_fill_color(*BRAND)
        pdf.ellipse(pdf.w - pdf.r_margin - 2.9, y + 2.9, 1.7, 1.7, style="F")
        pdf.set_x(pdf.l_margin)
        pdf.multi_cell(pdf.content_width - 7, 6.9 * pdf.scale, item, align="R",
                       new_x=XPos.LMARGIN, new_y=YPos.NEXT)
        pdf.space(1.2)
    pdf.space(1.6)


def block_ol(pdf, items):
    pdf.set_font("vazir", "", 11.5)
    pdf.set_text_color(*TEXT)
    for i, item in enumerate(items, 1):
        pdf.keep(12)
        y = pdf.get_y()
        pdf.set_font("vazir", "B", 10)
        pdf.set_text_color(*BRAND)
        pdf.set_xy(pdf.w - pdf.r_margin - 7, y)
        pdf.cell(7, 6.9 * pdf.scale, fa(i) + ")", align="R")
        pdf.set_font("vazir", "", 11.5)
        pdf.set_text_color(*TEXT)
        pdf.set_xy(pdf.l_margin, y)
        pdf.multi_cell(pdf.content_width - 9, 6.9 * pdf.scale, item, align="R",
                       new_x=XPos.LMARGIN, new_y=YPos.NEXT)
        pdf.space(1.2)
    pdf.space(1.6)


def block_kv(pdf, pairs):
    col = pdf.content_width / 3
    for key, value in pairs:
        pdf.keep(12)
        y = pdf.get_y()
        pdf.set_font("vazir", "B", 10.8)
        pdf.set_text_color(*BRAND_DARK)
        pdf.set_xy(pdf.w - pdf.r_margin - col, y)
        pdf.multi_cell(col, 7.1, key, align="R")
        y2 = pdf.get_y()
        pdf.set_font("vazir", "", 10.8)
        pdf.set_text_color(*TEXT)
        pdf.set_xy(pdf.l_margin, y)
        pdf.multi_cell(pdf.content_width - col - 3, 7.1, value, align="R")
        pdf.set_y(max(y2, pdf.get_y()) + 1.4)
    pdf.space(1.6)


def block_table(pdf, spec):
    headers = spec["headers"]
    rows = spec["rows"]
    widths = spec.get("widths")
    caption = spec.get("caption")
    n = len(headers)
    total = pdf.content_width
    if widths:
        s = sum(widths)
        widths = [w / s * total for w in widths]
    else:
        widths = [total / n] * n

    font_size = spec.get("size", 9.8)
    line_h = 6.0

    def measure(row, size):
        pdf.set_font("vazir", "", size)
        height = 0
        for i, cell in enumerate(row):
            lines = len(pdf.multi_cell(widths[i], line_h, str(cell), align="R",
                                       dry_run=True, output="LINES"))
            height = max(height, lines * line_h)
        return height + 2.4

    pdf.keep(26)
    if caption:
        pdf.set_font("vazir", "B", 9.6)
        pdf.set_text_color(*BRAND_DARK)
        pdf.multi_cell(0, 6, caption, align="R", new_x=XPos.LMARGIN, new_y=YPos.NEXT)
        pdf.space(1.2)

    def draw_header():
        h = measure(headers, font_size)
        pdf.set_font("vazir", "B", font_size)
        pdf.set_fill_color(*BRAND)
        pdf.set_draw_color(*BRAND)
        pdf.set_text_color(255, 255, 255)
        x = pdf.w - pdf.r_margin
        y = pdf.get_y()
        for i, head in enumerate(headers):
            x -= widths[i]
            pdf.rect(x, y, widths[i], h, style="F")
            pdf.set_xy(x + 1.5, y + 1.1)
            pdf.multi_cell(widths[i] - 3, line_h, str(head), align="R")
        pdf.set_y(y + h)

    draw_header()
    pdf.set_draw_color(205, 214, 226)
    for r, row in enumerate(rows):
        h = measure(row, font_size)
        if pdf.get_y() + h > pdf.h - pdf.b_margin:
            pdf.add_page()
            draw_header()
        pdf.set_font("vazir", "", font_size)
        pdf.set_text_color(*TEXT)
        pdf.set_fill_color(*(CODE_BG if r % 2 == 0 else (255, 255, 255)))
        x = pdf.w - pdf.r_margin
        y = pdf.get_y()
        for i, cell in enumerate(row):
            x -= widths[i]
            pdf.rect(x, y, widths[i], h, style="DF")
            pdf.set_xy(x + 1.5, y + 1.1)
            pdf.multi_cell(widths[i] - 3, line_h, str(cell), align="R")
        pdf.set_y(y + h)
    pdf.space(3.2)


def block_note(pdf, spec):
    kind = spec.get("kind", "info")
    title = spec.get("title", "")
    text = spec["text"]
    if kind == "green":
        bg, border, fg = GREEN_BG, GREEN_BORDER, GREEN
    elif kind == "amber":
        bg, border, fg = AMBER_BG, AMBER_BORDER, AMBER
    else:
        bg, border, fg = BRAND_LIGHT, (144, 190, 240), BRAND_DARK

    pdf.keep(24)
    pdf.set_font("vazir", "", 10.8)
    lines = pdf.multi_cell(pdf.content_width - 8, 6.9, text, align="R",
                           dry_run=True, output="LINES")
    height = len(lines) * 6.9 + (8 if title else 3) + 5
    if pdf.get_y() + height > pdf.h - pdf.b_margin:
        pdf.add_page()

    y = pdf.get_y()
    pdf.set_fill_color(*bg)
    pdf.set_draw_color(*border)
    pdf.set_line_width(0.4)
    pdf.rect(pdf.l_margin, y, pdf.content_width, height, style="DF")
    pdf.set_line_width(0.2)
    pdf.set_xy(pdf.l_margin + 4, y + 2.5)
    if title:
        pdf.set_font("vazir", "B", 10.4)
        pdf.set_text_color(*fg)
        pdf.multi_cell(pdf.content_width - 8, 6.6, title, align="R")
        pdf.set_x(pdf.l_margin + 4)
    pdf.set_font("vazir", "", 10.8)
    pdf.set_text_color(*fg)
    pdf.multi_cell(pdf.content_width - 8, 6.9, text, align="R")
    pdf.set_y(y + height)
    pdf.space(3)


def block_code(pdf, spec):
    caption = spec.get("caption")
    code = spec["code"].strip("\n")
    lines = code.split("\n")
    font = "mono" if os.path.exists(MONO) else "vazir"
    size = spec.get("size", 8.0)
    lh = 4.5
    height = len(lines) * lh + 5

    pdf.keep(20)
    if caption:
        pdf.set_font("vazir", "B", 9.6)
        pdf.set_text_color(*BRAND_DARK)
        pdf.multi_cell(0, 6, caption, align="R", new_x=XPos.LMARGIN, new_y=YPos.NEXT)
        pdf.space(1)

    idx = 0
    while idx < len(lines):
        avail = pdf.h - pdf.b_margin - pdf.get_y() - 6
        fit = max(1, int(avail // lh))
        chunk = lines[idx:idx + fit]
        h = len(chunk) * lh + 4
        y = pdf.get_y()
        pdf.set_fill_color(*CODE_BG)
        pdf.set_draw_color(*CODE_BORDER)
        pdf.rect(pdf.l_margin, y, pdf.content_width, h, style="DF")
        pdf.set_font(font, "", size)
        pdf.set_text_color(*TEXT)
        yy = y + 2
        with pdf.local_context(text_shaping=False):
            for line in chunk:
                pdf.set_xy(pdf.l_margin + 3, yy)
                pdf.cell(pdf.content_width - 6, lh, line, align="L")
                yy += lh
        pdf.set_y(y + h)
        idx += fit
        if idx < len(lines):
            pdf.add_page()
    pdf.space(3)


def block_img(pdf, spec):
    path = os.path.join(ROOT, spec["path"])
    if not os.path.exists(path):
        return
    height = spec.get("height", 62)
    width = spec.get("width", pdf.content_width)
    if pdf.get_y() + height + 10 > pdf.h - pdf.b_margin:
        pdf.add_page()
    x = pdf.l_margin + (pdf.content_width - width) / 2
    pdf.image(path, x=x, y=pdf.get_y(), w=width, h=height)
    pdf.set_y(pdf.get_y() + height + 2)
    caption = spec.get("caption")
    if caption:
        pdf.set_font("vazir", "", 9)
        pdf.set_text_color(*GRAY)
        pdf.multi_cell(0, 5.5, caption, align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)
    pdf.space(3.5)


def block_fig(pdf, spec):
    fn = DIAGRAMS[spec["draw"]]
    height = spec.get("height", 60)
    if pdf.get_y() + height + 12 > pdf.h - pdf.b_margin:
        pdf.add_page()
    fn(pdf, pdf.content_width, height)
    pdf.space(2)
    caption = spec.get("caption")
    if caption:
        pdf.set_font("vazir", "", 9)
        pdf.set_text_color(*GRAY)
        pdf.multi_cell(0, 5.5, caption, align="C", new_x=XPos.LMARGIN, new_y=YPos.NEXT)
    pdf.space(3.5)


def block_quote(pdf, text):
    pdf.keep(18)
    y = pdf.get_y()
    pdf.set_font("vazir", "", 11)
    pdf.set_text_color(*BRAND_DARK)
    pdf.set_x(pdf.l_margin)
    pdf.multi_cell(pdf.content_width - 6, 7.1, text, align="R",
                   new_x=XPos.LMARGIN, new_y=YPos.NEXT)
    pdf.set_draw_color(*BRAND)
    pdf.set_line_width(1.1)
    pdf.line(pdf.w - pdf.r_margin - 0.6, y, pdf.w - pdf.r_margin - 0.6, pdf.get_y())
    pdf.set_line_width(0.2)
    pdf.space(3)


RENDERERS = {
    "h2": block_h2,
    "h3": block_h3,
    "h4": block_h4,
    "p": block_p,
    "pen": block_pen,
    "ul": block_ul,
    "ol": block_ol,
    "kv": block_kv,
    "tbl": block_table,
    "note": block_note,
    "code": block_code,
    "img": block_img,
    "fig": block_fig,
    "quote": block_quote,
}


def render_blocks(pdf, blocks):
    for kind, payload in blocks:
        if kind == "pagebreak":
            pdf.add_page()
            continue
        RENDERERS[kind](pdf, payload)


# ============================================================ ساخت مستند
def render_chapter_opener(pdf, chapter):
    pdf.current_chapter = chapter["title"]
    pdf.add_page()
    sec(pdf, chapter["title"], 0)

    y = pdf.get_y() + 6
    pdf.set_fill_color(*BRAND)
    pdf.rect(pdf.l_margin, y, pdf.content_width, 34, style="F")
    pdf.set_xy(pdf.l_margin + 6, y + 5)
    pdf.set_font("vazir", "", 11)
    pdf.set_text_color(190, 214, 245)
    pdf.cell(pdf.content_width - 12, 7, chapter["label"], align="R")
    pdf.set_xy(pdf.l_margin + 6, y + 14)
    pdf.set_font("vazir", "B", 17)
    pdf.set_text_color(255, 255, 255)
    pdf.cell(pdf.content_width - 12, 12, chapter["title"], align="R")
    pdf.set_y(y + 42)

    if chapter.get("intro"):
        block_p(pdf, chapter["intro"])
    if chapter.get("outline"):
        block_note(pdf, {
            "kind": "info",
            "title": "آنچه در این فصل می‌خوانید",
            "text": "\n".join("•  " + item for item in chapter["outline"]),
        })
    pdf.space(2)


def build(scale=1.0, extra_pad=0, quiet=True):
    pdf = InfoPdf(scale=scale)
    pdf.set_auto_page_break(auto=True, margin=18)
    register_fonts(pdf)
    pdf.set_text_shaping(use_shaping_engine=True, direction="rtl")
    pdf.set_margins(24, 21, 24)
    pdf.set_title(content.DOC_TITLE)
    pdf.set_author(content.STUDENT)
    pdf.set_subject(content.PROJECT_TITLE)
    pdf.set_creator("tools/generate_info_pdf.py")
    pdf.alias_nb_pages()

    render_cover(pdf)

    # فهرست مطالب (رزرو صفحه، مقدار واقعی در پایان درج می‌شود)
    pdf.add_page()
    pdf.current_chapter = "فهرست مطالب"
    pdf.insert_toc_placeholder(render_toc, pages=content.TOC_PAGES)

    # چکیده و بخش‌های آغازین
    for section in content.front_matter():
        pdf.current_chapter = section["title"]
        pdf.add_page()
        sec(pdf, section["title"], 0)
        pdf.set_font("vazir", "B", 17)
        pdf.set_text_color(*BRAND_DARK)
        pdf.multi_cell(0, 11, section["title"], align="R",
                       new_x=XPos.LMARGIN, new_y=YPos.NEXT)
        pdf.set_draw_color(*BRAND)
        pdf.set_line_width(0.6)
        pdf.line(pdf.l_margin, pdf.get_y() + 1, pdf.w - pdf.r_margin, pdf.get_y() + 1)
        pdf.set_line_width(0.2)
        pdf.space(6)
        render_blocks(pdf, section["blocks"])

    # فصل‌ها
    for chapter in content.chapters():
        render_chapter_opener(pdf, chapter)
        render_blocks(pdf, chapter["blocks"])

    # منابع و مآخذ
    for section in content.back_matter(extra_pad):
        pdf.current_chapter = section["title"]
        pdf.add_page()
        sec(pdf, section["title"], 0)
        pdf.set_font("vazir", "B", 17)
        pdf.set_text_color(*BRAND_DARK)
        pdf.multi_cell(0, 11, section["title"], align="R",
                       new_x=XPos.LMARGIN, new_y=YPos.NEXT)
        pdf.set_draw_color(*BRAND)
        pdf.set_line_width(0.6)
        pdf.line(pdf.l_margin, pdf.get_y() + 1, pdf.w - pdf.r_margin, pdf.get_y() + 1)
        pdf.set_line_width(0.2)
        pdf.space(6)
        render_blocks(pdf, section["blocks"])

    return pdf


def main():
    os.makedirs(OUT_DIR, exist_ok=True)

    target = TARGET_PAGES
    best = None
    # جست‌وجوی ضریب فاصلهٔ سطرها برای رسیدن دقیق به تعداد صفحهٔ هدف
    lo, hi = 0.86, 1.42
    for _ in range(18):
        mid = (lo + hi) / 2
        pdf = build(scale=mid)
        pages = pdf.pages_count
        if not best or abs(pages - target) < abs(best[1] - target):
            best = (mid, pages, pdf)
        print(f"  scale={mid:.4f} -> {pages} pages")
        if pages == target:
            best = (mid, pages, pdf)
            break
        if pages < target:
            lo = mid
        else:
            hi = mid

    scale, pages, pdf = best
    if pages != target:
        # تنظیم دقیق با افزودن/کاستن محتوای پایانی
        for pad in range(0, 14):
            probe = build(scale=scale, extra_pad=pad)
            print(f"  pad={pad} -> {probe.pages_count} pages")
            if probe.pages_count == target:
                pdf, pages = probe, probe.pages_count
                break

    pdf.output(OUT_FILE)
    print(f"\nSaved {OUT_FILE}  ({pages} pages, scale={scale:.4f})")
    if pages != target:
        print(f"WARNING: page count is {pages}, target was {target}", file=sys.stderr)


if __name__ == "__main__":
    main()

# -*- coding: utf-8 -*-
"""داده‌ها و ساختار کلی مستند فنی نرم‌افزار Dental Center."""
import json
import os
import re
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(os.path.dirname(HERE))
sys.path.insert(0, os.path.join(ROOT, "tools"))

DOC_TITLE = "مستند جامع نرم‌افزار Dental Center"
DOC_SUBTITLE = (
    "تحلیل، طراحی، پیاده‌سازی و ارزیابی یک نرم‌افزار دسکتاپ فارسی برای ارائهٔ "
    "طراحی مرکز دندان‌پزشکی با رویکرد بهره‌وری انرژی"
)
PROJECT_TITLE = "طراحی ساختمان مرکز دندانپزشکی با رویکرد بهره‌وری انرژی"
STUDENT = "فائزه دهستانی"
SUPERVISOR = "دکتر علیرضا کریمیان"
UNIVERSITY = "دانشگاه اصفهان"
FACULTY = "دانشکدهٔ فنی و مهندسی — مهندسی نرم‌افزار / معماری"
APP_VERSION = "نسخهٔ ۱٫۴ (Avalonia UI 12.1 / .NET 10)"
YEAR = "۱۴۰۵"
TOC_PAGES = 3

COVER_BLURB = (
    "این مستند در پنج فصل، تمام جنبه‌های نرم‌افزار Dental Center را شرح می‌دهد: از مفاهیم "
    "پایهٔ بهره‌وری انرژی در ساختمان‌های درمانی و مرور پیشینهٔ پژوهشی، تا روش‌های پیشنهادی "
    "طراحی و پیاده‌سازی، معرفی تفصیلی همهٔ بخش‌ها و ویژگی‌های نرم‌افزار، و در پایان "
    "جمع‌بندی نتایج و افق‌های توسعهٔ آینده."
)
COVER_FOOTER = "این مستند به‌صورت خودکار با اسکریپت tools/generate_info_pdf.py تولید شده است."


# --------------------------------------------------------------- دادهٔ مخزن
def load_topics():
    """موضوع‌های چهار بخش برنامه از Data/ContentData.cs + tools/pdf_extras.json."""
    from generate_pdfs import parse_topics  # noqa: E402

    topics = parse_topics()
    return topics


def load_extras():
    path = os.path.join(ROOT, "tools", "pdf_extras.json")
    with open(path, encoding="utf-8") as fh:
        return json.load(fh)


def source_excerpt(relative, start_marker=None, max_lines=40, skip=0):
    """چند خط از یک فایل واقعی مخزن برای درج به‌عنوان لیستینگ کد."""
    path = os.path.join(ROOT, relative)
    with open(path, encoding="utf-8") as fh:
        lines = fh.read().split("\n")
    start = 0
    if start_marker:
        for i, line in enumerate(lines):
            if start_marker in line:
                start = i
                break
    start += skip
    out = lines[start:start + max_lines]
    while out and not out[-1].strip():
        out.pop()
    return "\n".join(out)


def count_lines(patterns):
    total = 0
    for pattern in patterns:
        path = os.path.join(ROOT, pattern)
        if os.path.isfile(path):
            with open(path, encoding="utf-8", errors="ignore") as fh:
                total += len(fh.readlines())
    return total


# ------------------------------------------------ بازپیاده‌سازی موتور محاسبه
LIGHTING_W_M2 = 12
HVAC_KWH_M2 = 110
VENT_KWH_M2 = 25
UNIT_KWH_H = 0.9
COMP_KWH_H = 1.2
STERIL_KWH_DAY = 4.5
OTHER_KWH_M2 = 18
SOLAR_KWH_KW = 1600


def calculate(area=200, units=4, hours=10, days=290, tariff=1500,
              led=False, sensors=False, vrf=False, insulation=False,
              recovery=False, vsd=False, solar=False, solar_kw=10):
    """همان الگوریتم Services/EnergyCalculator.cs برای تولید جدول‌های مستند."""
    work_hours = hours * days
    lighting_base = LIGHTING_W_M2 * area / 1000.0 * work_hours
    hvac_base = HVAC_KWH_M2 * area
    vent_base = VENT_KWH_M2 * area
    units_base = UNIT_KWH_H * units * work_hours * 0.45
    comp_base = COMP_KWH_H * work_hours * 0.35
    steril_base = STERIL_KWH_DAY * days
    other_base = OTHER_KWH_M2 * area
    base_total = (lighting_base + hvac_base + vent_base + units_base
                  + comp_base + steril_base + other_base)

    lighting = lighting_base * (0.40 if led else 1) * (0.75 if sensors else 1)
    hvac = hvac_base * (0.72 if vrf else 1) * (0.80 if insulation else 1)
    vent = vent_base * (0.60 if recovery else 1)
    comp = comp_base * (0.68 if vsd else 1)
    before_solar = lighting + hvac + vent + units_base + comp + steril_base + other_base
    solar_kwh = min(before_solar, max(0, solar_kw) * SOLAR_KWH_KW) if solar else 0
    optimized = max(0, before_solar - solar_kwh)

    return {
        "base": round(base_total),
        "optimized": round(optimized),
        "solar": round(solar_kwh),
        "saving": round(base_total - optimized),
        "percent": (base_total - optimized) / base_total * 100 if base_total else 0,
        "money": round((base_total - optimized) * tariff),
        "lines": [
            ("سرمایش و گرمایش", hvac_base, hvac),
            ("روشنایی", lighting_base, lighting),
            ("یونیت‌های درمان", units_base, units_base),
            ("تهویه", vent_base, vent),
            ("کمپرسور و ساکشن", comp_base, comp),
            ("استریلیزاسیون", steril_base, steril_base),
            ("تجهیزات اداری و متفرقه", other_base, other_base),
        ],
    }


PERSIAN_DIGITS = str.maketrans("0123456789", "۰۱۲۳۴۵۶۷۸۹")


def fa(value):
    return str(value).translate(PERSIAN_DIGITS)


def num(value, digits=0):
    text = f"{value:,.{digits}f}".replace(",", "٬").replace(".", "٫")
    return fa(text)


FULL = calculate(led=True, sensors=True, vrf=True, insulation=True,
                 recovery=True, vsd=True, solar=True)
BASE = calculate()
ENERGY_CHART_ROWS = [(label, b, o) for label, b, o in FULL["lines"]]


# ------------------------------------------------------------------ ساختار
def _walk_captions():
    """گردآوری عنوان جدول‌ها و شکل‌ها به‌ترتیب ظهور در مستند."""
    tables, figures = [], []
    sources = []
    from infodoc import content_front
    sources.extend(content_front.SECTIONS)
    sources.extend(chapters())
    for item in sources:
        for kind, payload in item.get("blocks", []):
            if kind == "tbl" and isinstance(payload, dict) and payload.get("caption"):
                tables.append(payload["caption"])
            elif kind in ("fig", "img") and isinstance(payload, dict) and payload.get("caption"):
                figures.append(payload["caption"])
    return tables, figures


def figures_section():
    tables, figures = _walk_captions()
    blocks = [
        ("p",
         "برای دسترسی سریع، عنوان همهٔ جدول‌ها و شکل‌های مستند به‌ترتیب ظهور فهرست شده است. "
         "شمارهٔ هر جدول یا شکل، شمارهٔ فصل و ترتیب آن در همان فصل را نشان می‌دهد."),
        ("h2", "فهرست جدول‌ها"),
        ("ul", tables),
        ("h2", "فهرست شکل‌ها و تصاویر"),
        ("ul", figures),
    ]
    return {"title": "فهرست جدول‌ها و شکل‌ها", "blocks": blocks}


def front_matter():
    from infodoc import content_front
    return list(content_front.SECTIONS) + [figures_section()]


def chapters():
    from infodoc import (content_ch1, content_ch2, content_ch3,
                         content_ch4, content_ch5)
    return [
        content_ch1.CHAPTER,
        content_ch2.CHAPTER,
        content_ch3.CHAPTER,
        content_ch4.CHAPTER,
        content_ch5.CHAPTER,
    ]


def back_matter(extra_pad=0):
    from infodoc import content_refs
    return content_refs.sections(extra_pad)

# -*- coding: utf-8 -*-
"""
Generate one Persian (RTL) PDF per topic of the DentalCenter app.

Parses Data/ContentData.cs and writes Assets/PDF/<id>.pdf for every topic,
so the «مشاهده PDF» buttons of the app become active.

Dependencies (pip): fpdf2, uharfbuzz
Run from the repository root:  python3 tools/generate_pdfs.py
"""
import os
import re
import sys

from fpdf import FPDF

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
CONTENT = os.path.join(ROOT, "Data", "ContentData.cs")
PDF_DIR = os.path.join(ROOT, "Assets", "PDF")
REGULAR = os.path.join(ROOT, "Assets", "Fonts", "Vazirmatn-Regular.ttf")
BOLD = os.path.join(ROOT, "Assets", "Fonts", "Vazirmatn-Bold.ttf")
FALLBACK = "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf"

PROJECT_TITLE = "طراحی ساختمان مرکز دندانپزشکی با رویکرد بهره‌وری انرژی"
STUDENT = "فائزه دهستانی"
SUPERVISOR = "دکتر علیرضا کریمیان"
UNIVERSITY = "دانشگاه اصفهان"

BRAND = (13, 71, 161)       # #0D47A1
BRAND_DARK = (8, 46, 110)
GREEN = (27, 94, 32)
GREEN_BG = (232, 245, 233)
GREEN_BORDER = (165, 214, 167)
GRAY = (110, 122, 138)
LIGHT = (227, 242, 253)

SECTION_LABELS = {
    "Equipment": "بخش اول — معرفی تجهیزات دندان‌پزشکی",
    "Spaces": "بخش دوم — فضای فیزیکی مرکز",
    "Energy": "بخش سوم — بهره‌وری انرژی",
    "Children": "بخش چهارم — مرکز خدمات کودکان",
}


def _strings(text):
    """Return all C# string-literal contents inside *text*, concatenated."""
    return "".join(re.findall(r'"((?:[^"\\]|\\.)*)"', text))


def _clean(s):
    """Unescape C# escapes that appear in our strings (just backslash-escaped quotes)."""
    return s.replace('\\"', '"').strip()


def parse_topics():
    src = open(CONTENT, encoding="utf-8").read()

    sections = {}
    for key in SECTION_LABELS:
        m = re.search(
            r"public static IReadOnlyList<Topic> " + key + r" \{ get; \} = new\[\]\s*\{",
            src,
        )
        if not m:
            print("WARN: section not found:", key, file=sys.stderr)
            continue
        start = m.end()
        # We are already inside the array initializer's braces, so start at depth 1
        # and stop at the matching closing brace.
        depth = 1
        i = start
        while i < len(src):
            if src[i] == "{":
                depth += 1
            elif src[i] == "}":
                depth -= 1
                if depth == 0:
                    break
            i += 1
        sections[key] = src[start:i]

    def find_matching_paren(s, start):
        """start points just after the opening '('; return index of the matching ')'."""
        depth = 1
        i = start
        instr = False
        esc = False
        while i < len(s):
            c = s[i]
            if instr:
                if esc:
                    esc = False
                elif c == "\\":
                    esc = True
                elif c == '"':
                    instr = False
            else:
                if c == '"':
                    instr = True
                elif c == "(":
                    depth += 1
                elif c == ")":
                    depth -= 1
                    if depth == 0:
                        return i
            i += 1
        return -1

    topics = []
    for key, block in sections.items():
        pos = 0
        while True:
            m = re.search(r"new Topic\(", block[pos:])
            if not m:
                break
            start = pos + m.end()
            end = find_matching_paren(block, start)
            if end < 0:
                print("WARN: unclosed Topic in", key, file=sys.stderr)
                break
            body = block[start:end]

            def field(name):
                mm = re.search(name + r'\s*:\s*"((?:[^"\\]|\\.)*)"', body)
                return _clean(mm.group(1)) if mm else ""

            # multiline fields: text between marker and the next field marker
            def multiline(marker, next_markers):
                mm = re.search(marker + r"\s*:", body)
                if not mm:
                    return ""
                end_marker = len(body)
                for nm in next_markers:
                    nm_match = re.search(r"\n\s*" + nm + r"\s*:", body)
                    if nm_match and nm_match.start() > mm.end():
                        end_marker = min(end_marker, nm_match.start())
                return _clean(_strings(body[mm.end():end_marker]))

            topic_id = field("id")
            title = field("title")
            summary = multiline("summary", ["image", "bullets", "specs", "energyNote", "keywords"])
            image = field("image")

            # bullets: strings between "bullets: new[]" and "specs:"
            bm = re.search(r"bullets\s*:\s*new\[\]", body)
            bullets = []
            if bm:
                b_end = body.find("specs", bm.end())
                bullets = [_clean(s) for s in re.findall(r'"((?:[^"\\]|\\.)*)"', body[bm.end():b_end])]

            # specs: pairs between "specs: new[]" and "energyNote:"
            sm = re.search(r"specs\s*:\s*new\[\]", body)
            specs = []
            if sm:
                s_end = body.find("energyNote", sm.end())
                specs = [
                    (_clean(k), _clean(v))
                    for k, v in re.findall(
                        r'new Spec\(\s*"((?:[^"\\]|\\.)*)"\s*,\s*"((?:[^"\\]|\\.)*)"\s*\)',
                        body[sm.end():s_end],
                    )
                ]

            energy_note = multiline("energyNote", ["keywords"])
            keywords = field("keywords")

            topics.append(
                {
                    "id": topic_id,
                    "section": key,
                    "title": title,
                    "summary": summary,
                    "image": image,
                    "bullets": bullets,
                    "specs": specs,
                    "energy_note": energy_note,
                    "keywords": keywords,
                }
            )

            pos = end + 1

    return topics


class ThesisPdf(FPDF):
    def header(self):
        if self.page_no() == 1:
            return
        self.set_font("vazir", "", 8.5)
        self.set_text_color(*GRAY)
        self.cell(0, 6, PROJECT_TITLE, align="R")
        self.ln(3)
        self.set_draw_color(*GRAY)
        self.line(self.l_margin, self.get_y(), self.w - self.r_margin, self.get_y())

    def footer(self):
        self.set_y(-14)
        self.set_font("vazir", "", 8.5)
        self.set_text_color(*GRAY)
        self.cell(0, 8, f"صفحه {self.page_no()}", align="R")
        self.cell(0, 8, f"{STUDENT} — {SUPERVISOR}", align="L")

    def section(self, text):
        self.ln(3)
        self.set_font("vazir", "B", 13)
        self.set_text_color(*BRAND)
        self.multi_cell(0, 8, text, align="R")
        self.set_draw_color(*BRAND)
        self.set_line_width(0.5)
        self.line(self.l_margin, self.get_y(), self.w - self.r_margin, self.get_y())
        self.set_line_width(0.2)
        self.ln(3)

    def body(self, text, size=11.5, color=(40, 48, 58)):
        self.set_font("vazir", "", size)
        self.set_text_color(*color)
        self.multi_cell(0, 7.5, text, align="R")

    def bullet(self, text):
        self.set_font("vazir", "", 11)
        self.set_text_color(40, 48, 58)
        # draw a small bullet then the text
        x = self.get_x()
        y = self.get_y()
        self.set_fill_color(*BRAND)
        self.ellipse(self.w - self.r_margin - 2.6, y + 3.6, 1.6, 1.6, style="F")
        self.set_x(x)
        self.multi_cell(self.w - self.l_margin - self.r_margin - 10, 7.2, text, align="R")
        self.ln(1)


def build_pdf(topic, out_path):
    pdf = ThesisPdf(format="A4")
    pdf.set_auto_page_break(auto=True, margin=16)
    pdf.add_font("vazir", "", REGULAR)
    pdf.add_font("vazir", "B", BOLD)
    if os.path.exists(FALLBACK):
        pdf.add_font("fallback", "", FALLBACK)
        pdf.set_fallback_fonts(["fallback"])
    pdf.set_text_shaping(use_shaping_engine=True, direction="rtl")
    pdf.set_margins(18, 18, 18)

    pdf.add_page()

    # Header band
    pdf.set_fill_color(*BRAND)
    pdf.rect(0, 0, pdf.w, 30, style="F")
    pdf.set_fill_color(*BRAND_DARK)
    pdf.rect(0, 30, pdf.w, 2, style="F")
    pdf.set_xy(18, 6)
    pdf.set_font("vazir", "B", 13.5)
    pdf.set_text_color(255, 255, 255)
    pdf.cell(0, 8, PROJECT_TITLE, align="R")
    pdf.ln(9)
    pdf.set_font("vazir", "", 9.5)
    pdf.set_text_color(200, 220, 245)
    pdf.cell(0, 6, SECTION_LABELS.get(topic["section"], ""), align="R")
    pdf.ln(6)
    pdf.set_y(40)

    # Title
    pdf.set_font("vazir", "B", 18)
    pdf.set_text_color(*BRAND_DARK)
    pdf.multi_cell(0, 10, topic["title"], align="R")
    pdf.ln(2)

    # Summary
    if topic["summary"]:
        pdf.body(topic["summary"], 11.5)

    # Bullets
    if topic["bullets"]:
        pdf.section("نکات کلیدی طراحی")
        for b in topic["bullets"]:
            pdf.bullet(b)

    # Specs
    if topic["specs"]:
        pdf.section("مشخصات فنی")
        pdf.set_fill_color(*LIGHT)
        pdf.set_text_color(*BRAND_DARK)
        pdf.set_font("vazir", "B", 10.5)
        col = (pdf.w - pdf.l_margin - pdf.r_margin) / 2
        pdf.cell(col, 9, "شاخص", border=1, fill=True, align="R")
        pdf.cell(col, 9, "مقدار", border=1, fill=True, align="R")
        pdf.ln()
        pdf.set_font("vazir", "", 10.5)
        pdf.set_text_color(40, 48, 58)
        for k, v in topic["specs"]:
            pdf.cell(col, 9, k, border=1, align="R")
            pdf.cell(col, 9, v, border=1, align="R")
            pdf.ln()
        pdf.ln(2)

    # Keywords
    if topic["keywords"]:
        pdf.section("واژگان کلیدی")
        pdf.set_font("vazir", "", 11)
        pdf.set_text_color(*BRAND_DARK)
        pdf.multi_cell(0, 7.5, topic["keywords"], align="R")

    # Energy note
    if topic["energy_note"]:
        pdf.ln(2)
        y0 = pdf.get_y()
        pdf.set_fill_color(*GREEN_BG)
        pdf.set_draw_color(*GREEN_BORDER)
        pdf.set_line_width(0.4)
        box = pdf.multi_cell(
            0, 7.5, "نکتهٔ بهره‌وری انرژی:\n" + topic["energy_note"],
            align="R", fill=True, border=1,
        )
        pdf.set_line_width(0.2)

    pdf.output(out_path)


def main():
    topics = parse_topics()
    if not topics:
        print("No topics parsed — aborting.", file=sys.stderr)
        sys.exit(1)

    os.makedirs(PDF_DIR, exist_ok=True)
    for t in topics:
        out = os.path.join(PDF_DIR, t["id"] + ".pdf")
        build_pdf(t, out)
        print(f"  {t['id']:16s} -> {os.path.basename(out)}")

    print(f"\nGenerated {len(topics)} PDF files in {PDF_DIR}")


if __name__ == "__main__":
    main()

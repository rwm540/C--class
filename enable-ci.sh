#!/usr/bin/env bash
# فعال‌سازی ورک‌فلوی ساخت exe روی GitHub Actions.
# چون توکن ایجنت اجازهٔ نوشتن در .github/workflows را ندارد، این اسکریپت را
# یک بار روی سیستم خودتان اجرا کنید (با حساب گیت‌هاب خودتان).
#
#   bash enable-ci.sh
#
set -euo pipefail

cd "$(dirname "$0")"

BRANCH="arena/01a010af-c-class"

mkdir -p .github/workflows
cp ci/build-exe.yml .github/workflows/build-exe.yml

git add .github/workflows/build-exe.yml
git commit -m "Enable Windows exe build workflow" || echo "چیزی برای کامیت نبود"
git push origin "HEAD:$BRANCH"

echo
echo "✅ ورک‌فلو push شد."
echo "   اجرای بیلد را اینجا ببینید: https://github.com/rwm540/C--class/actions"
echo "   بعد از اتمام، فایل exe در بخش Artifacts (DentalCenter-win-x64) و در Releases قابل دانلود است."

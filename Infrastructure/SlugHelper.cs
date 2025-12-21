using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Horizon.Infrastructure
{
    public static class SlugHelper
    {
        public static string ToSlug(this string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return string.Empty;

            // 1. Chuyển về chữ thường
            title = title.ToLowerInvariant();

            // 2. Thay thế các ký tự tiếng Việt đặc thù
            title = title.Replace("đ", "d").Replace("đ", "d");

            // 3. Chuẩn hóa và loại bỏ dấu (Unicode)
            title = title.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();
            foreach (var c in title)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }
            title = builder.ToString().Normalize(NormalizationForm.FormC);

            // 4. Loại bỏ ký tự đặc biệt, chỉ giữ chữ cái, số và khoảng trắng
            title = Regex.Replace(title, @"[^a-z0-9\s-]", "");

            // 5. Thay khoảng trắng thành gạch ngang và xóa gạch ngang thừa
            title = Regex.Replace(title, @"\s+", "-").Trim();
            title = Regex.Replace(title, @"-+", "-");

            return title;
        }
    }
}
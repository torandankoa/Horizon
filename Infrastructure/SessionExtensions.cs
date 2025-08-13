using System.Text.Json; // Cần thiết để làm việc với JSON

namespace Horizon.Infrastructure // Namespace phải khớp với vị trí file
{
    public static class SessionExtensions
    {
        // Phương thức để LƯU một đối tượng vào Session
        public static void Set<T>(this ISession session, string key, T value)
        {
            // Chuyển đối tượng thành chuỗi JSON và lưu vào Session
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Phương thức để ĐỌC một đối tượng từ Session
        public static T? Get<T>(this ISession session, string key)
        {
            // Lấy chuỗi JSON từ Session
            var value = session.GetString(key);

            // Nếu không có gì, trả về giá trị mặc định (null)
            // Nếu có, chuyển chuỗi JSON ngược lại thành đối tượng
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
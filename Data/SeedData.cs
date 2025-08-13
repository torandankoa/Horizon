using Microsoft.AspNetCore.Identity;

namespace Horizon.Data // Đảm bảo namespace này là Horizon.Data
{
    public static class SeedData
    {
        // async Task vì chúng ta sẽ làm việc với CSDL một cách bất đồng bộ
        public static async Task InitializeAsync(IServiceProvider services)
        {
            // Lấy các service cần thiết từ hệ thống DI (Dependency Injection)
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            // DANH SÁCH CÁC VAI TRÒ CẦN TẠO
            string[] roleNames = { "Admin", "Customer" };

            // Lặp qua danh sách và tạo vai trò nếu nó chưa tồn tồn tại
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // TẠO TÀI KHOẢN ADMIN MẶC ĐỊNH
            string adminEmail = "admin@horizon.com"; // <<< BẠN CÓ THỂ THAY BẰNG EMAIL CỦA BẠN

            // Tìm xem user admin đã tồn tại chưa
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                // Nếu chưa, tạo một user mới
                IdentityUser user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true // Xác thực email luôn cho tài khoản admin
                };

                // Tạo user với mật khẩu đã cho
                // LƯU Ý: Mật khẩu này cần đủ mạnh để vượt qua yêu cầu mặc định của Identity
                IdentityResult result = await userManager.CreateAsync(user, "Admin@123");

                // Nếu tạo user thành công, gán vai trò "Admin" cho user đó
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
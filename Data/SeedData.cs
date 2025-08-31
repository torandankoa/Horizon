using Horizon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Horizon.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<MyDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            // --- 1. SEED ROLES AND ADMIN USER (Giữ nguyên) ---
            string[] roleNames = { "Admin", "Customer" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string adminEmail = "admin@horizon.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Dừng lại nếu đã có đơn hàng (để tránh tạo lại mỗi lần khởi động)
            if (await context.Orders.AnyAsync())
            {
                return;
            }

            // --- 2. SEED 20 CUSTOMER ACCOUNTS ---
            var customerUsers = new List<IdentityUser>();
            for (int i = 1; i <= 20; i++)
            {
                string customerEmail = $"customer{i}@test.com";
                if (await userManager.FindByEmailAsync(customerEmail) == null)
                {
                    var customerUser = new IdentityUser { UserName = customerEmail, Email = customerEmail, EmailConfirmed = true };
                    await userManager.CreateAsync(customerUser, "Customer@123");
                    await userManager.AddToRoleAsync(customerUser, "Customer");
                    customerUsers.Add(customerUser);
                }
                else
                {
                    // Nếu user đã tồn tại, lấy về để sử dụng cho việc tạo đơn hàng
                    customerUsers.Add(await userManager.FindByEmailAsync(customerEmail));
                }
            }

            // --- 3. SEED PRODUCTS & CATEGORIES (Nếu cần) ---
            // Tạm thời giả định bạn đã có sản phẩm. Nếu chưa, bạn cần thêm code seed sản phẩm ở đây.
            var products = await context.Products.ToListAsync();
            if (!products.Any()) return; // Cần có sản phẩm để tạo đơn hàng

            // --- 4. SEED 100 ORDERS VÀ ORDER DETAILS ---
            var random = new Random();
            var orderList = new List<Order>();
            var orderDetailList = new List<OrderDetail>();

            for (int i = 0; i < 100; i++)
            {
                var orderDate = DateTime.UtcNow.AddDays(-random.Next(1, 365)); // Ngày ngẫu nhiên trong 1 năm qua
                var user = customerUsers[random.Next(customerUsers.Count)]; // Chọn một user ngẫu nhiên từ 20 user đã tạo

                var order = new Order
                {
                    UserId = user.Id,
                    OrderDate = orderDate,
                    Status = GetRandomOrderStatus(random),
                    CustomerName = $"Customer {user.UserName.Split('@')[0]}",
                    ShippingAddress = $"{random.Next(1, 1000)} Random St, City {random.Next(1, 50)}",
                    PhoneNumber = "0123456789"
                };

                // Thêm 1-5 sản phẩm ngẫu nhiên vào mỗi đơn hàng
                decimal totalAmount = 0;
                int itemsInOrder = random.Next(1, 6);
                var productCache = new HashSet<int>(); // Đảm bảo không thêm trùng sản phẩm trong 1 đơn hàng

                for (int j = 0; j < itemsInOrder; j++)
                {
                    var product = products[random.Next(products.Count)];
                    if (productCache.Contains(product.Id)) continue; // Bỏ qua nếu đã thêm

                    var quantity = random.Next(1, 4);
                    var orderDetail = new OrderDetail
                    {
                        ProductId = product.Id,
                        Quantity = quantity,
                        Price = product.SalePrice ?? product.Price,
                        Order = order // Liên kết với đơn hàng
                    };
                    totalAmount += orderDetail.Subtotal;
                    order.OrderDetails.Add(orderDetail);
                    productCache.Add(product.Id);
                }

                if (order.OrderDetails.Any())
                {
                    order.TotalAmount = totalAmount;
                    orderList.Add(order);
                }
            }

            await context.Orders.AddRangeAsync(orderList);
            await context.SaveChangesAsync();
        }

        // Hàm trợ giúp để tạo trạng thái đơn hàng ngẫu nhiên
        private static string GetRandomOrderStatus(Random random)
        {
            string[] statuses = { "Processing", "Shipped", "Completed", "Cancelled" };
            return statuses[random.Next(statuses.Length)];
        }
    }
}
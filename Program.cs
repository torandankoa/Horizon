// Thêm tất cả các using cần thiết ở đầu file
using Horizon.Data;
using Horizon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ========== 1: ĐĂNG KÝ CÁC SERVICE ==========

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChuoiKetNoi"));
});

// Cấu hình Identity với hỗ trợ Roles
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // <<< BẬT CHỨC NĂNG QUẢN LÝ VAI TRÒ
    .AddEntityFrameworkStores<MyDbContext>();

// Đăng ký dịch vụ cho Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ========== 2: XÂY DỰNG ỨNG DỤNG ==========
var app = builder.Build();

// ========== 3: CẤU HÌNH HTTP REQUEST PIPELINE ==========

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // Session phải được dùng trước Authorization

app.UseAuthorization(); // Identity yêu cầu dòng này

// Cấu hình route cho Area MVC
app.MapControllerRoute(
   name: "default",
   pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

// Map các trang Razor Pages của Identity <<< THÊM DÒNG NÀY
app.MapRazorPages();

// ... (các đoạn code phía trên giữ nguyên) ...

// Khối code Tự động tạo bảng và đổ dữ liệu SeedData
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MyDbContext>();

        // >>> DÒNG QUAN TRỌNG NHẤT: Tự động tạo bảng nếu chưa có trên server <<<
        await context.Database.MigrateAsync();

        // Sau đó mới chạy SeedData để đổ 100 đơn hàng mẫu
        await SeedData.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        // Ghi log lỗi nếu có sự cố xảy ra để mình biết đường sửa
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// ========== PHẦN 4: CHẠY ỨNG DỤNG ==========
app.Run();

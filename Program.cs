// Thêm tất cả các using cần thiết ở đầu file
using Horizon.Data;
using Horizon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ========== PHẦN 1: ĐĂNG KÝ CÁC SERVICE ==========

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChuoiKetNoi"));
});

// Cấu hình Identity với hỗ trợ Roles
// Thay thế dòng AddDefaultIdentity cũ của bạn bằng dòng này
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


// ========== PHẦN 2: XÂY DỰNG ỨNG DỤNG ==========
var app = builder.Build();

// ========== PHẦN 3: CẤU HÌNH HTTP REQUEST PIPELINE ==========

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


// Khối code để chạy SeedData (đã đúng)
using (var scope = app.Services.CreateScope())
{
    // Không cần 'using Horizon.Data;' ở đây nữa
    await SeedData.InitializeAsync(scope.ServiceProvider);
}


// ========== PHẦN 4: CHẠY ỨNG DỤNG ==========
app.Run();
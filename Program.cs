// Thêm các using cần thiết ở đầu file (nếu chưa có)
using Horizon.Models; // Namespace cho MyDbContext
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ========== PHẦN 1: ĐĂNG KÝ CÁC SERVICE ==========

// Đăng ký service cho Controllers và Views (CHỈ 1 LẦN)
builder.Services.AddControllersWithViews();

// Đăng ký Entity Framework Core với chuỗi kết nối (CHỈ 1 LẦN)
builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChuoiKetNoi"));
});

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// SỬ DỤNG SESSION - PHẢI NẰM SAU UseRouting và TRƯỚC UseAuthorization/Map...
app.UseSession();

app.UseAuthorization();

// Cấu hình route cho Area
app.MapControllerRoute(
   name: "default",
   pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

// ========== PHẦN 4: CHẠY ỨNG DỤNG ==========

app.Run();
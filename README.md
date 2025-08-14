# Đồ án Cuối kỳ: Website E-commerce Horizon

**Horizon** là một dự án website thương mại điện tử được xây dựng trong 10 ngày, lấy cảm hứng từ vũ trụ và thiết kế của trò chơi "Girl's Frontline". Trang web cho phép người dùng xem, tìm kiếm, mua sắm các sản phẩm (hư cấu) và cung cấp một trang quản trị mạnh mẽ để quản lý toàn bộ hệ thống.

## 🚀 Demo & Hình ảnh

*(Mẹo: Hãy chụp vài bức ảnh đẹp nhất của trang web - trang chủ, trang chi tiết sản phẩm, trang giỏ hàng, trang dashboard admin - và chèn vào đây để README thêm phần sống động)*

![Trang chủ Horizon](link_den_anh_trang_chu.png)
*Giao diện trang chủ với phong cách dark-mode và các sản phẩm mới nhất.*

![Dashboard Admin](link_den_anh_dashboard.png)
*Trang Dashboard của Admin với các thống kê và biểu đồ trực quan.*

## ✨ Các Chức năng Nổi bật

Dự án được phân chia thành 2 khu vực chính với các chức năng riêng biệt cho Khách hàng và Quản trị viên.

### I. Khu vực Khách hàng (Customer Area)

- **Giao diện Chủ đề:** Toàn bộ giao diện được "lột xác" theo phong cách sci-fi military, sử dụng tông màu tối và màu vàng hổ phách làm điểm nhấn, mang lại cảm giác công nghệ cao.
- **Trang chủ Động:**
    - Hiển thị danh sách các sản phẩm mới nhất.
    - **Chibi Animation:** Một "sân khấu" nhỏ với các nhân vật chibi di chuyển qua lại, khi click vào sẽ hiển thị lời thoại đặc trưng, tạo sự sống động và thú vị.
- **Xem Sản phẩm:**
    - Trang `/Shop` hiển thị tất cả sản phẩm dưới dạng card.
    - Trang chi tiết sản phẩm với bố cục 2 cột (hình ảnh/video và thông tin chi tiết).
- **Tìm kiếm & Lọc:** Cho phép tìm kiếm sản phẩm theo tên và lọc theo danh mục.
- **Giỏ hàng (Shopping Cart):**
    - Sử dụng `Session` để lưu trữ giỏ hàng cho người dùng.
    - Các chức năng: Thêm vào giỏ, Cập nhật số lượng, Xóa sản phẩm.
- **Luồng Đặt hàng:**
    - Yêu cầu đăng nhập để tiến hành thanh toán.
    - Form điền thông tin giao hàng.
    - Tự động trừ kho và lưu lịch sử đơn hàng vào cơ sở dữ liệu.
- **Xác thực Người dùng:**
    - Đăng ký và Đăng nhập tài khoản.
    - Giao diện đăng nhập/đăng ký được tùy chỉnh với video nền ấn tượng.
- **Tương tác:** Tích hợp cửa sổ chat live Tawk.to.

### II. Khu vực Quản trị (Admin Area)

- **Bảo mật & Phân quyền:** Toàn bộ khu vực Admin được bảo vệ, chỉ những tài khoản có vai trò "Admin" mới có thể truy cập.
- **Dashboard Thống kê:**
    - Hiển thị các số liệu tổng quan: Tổng doanh thu, Tổng đơn hàng, Tổng sản phẩm, Tổng số khách hàng.
    - **Biểu đồ trực quan:** Sử dụng Chart.js để vẽ biểu đồ tròn (phân loại sản phẩm) và biểu đồ cột (doanh thu theo ngày).
- **Quản lý Sản phẩm (CRUD):**
    - Chức năng Thêm, Sửa, Xóa, Xem danh sách sản phẩm.
    - Hỗ trợ upload ảnh từ máy tính hoặc dùng URL từ bên ngoài.
    - Tự động xóa file ảnh trên server khi sản phẩm bị xóa.
- **Quản lý Danh mục (CRUD):** Chức năng Thêm, Sửa, Xóa danh mục sản phẩm.

## 🛠️ Công nghệ Sử dụng

### Backend
- **Framework:** ASP.NET Core 8 MVC
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core 8
- **Authentication:** ASP.NET Core Identity (Hỗ trợ Roles: Admin, Customer)

### Frontend
- **Styling:** HTML5, CSS3, Bootstrap 5
- **JavaScript:**
    - Vanilla JS
    - **Chart.js** cho việc vẽ biểu đồ

### Công cụ & Quy trình
- **IDE:** Visual Studio 2022
- **Version Control:** Git & GitHub (sử dụng quy trình Feature Branch)
- **Project Management:** ClickUp (theo dõi tiến độ cá nhân)

## 🔧 Hướng dẫn Cài đặt và Chạy thử

1.  **Clone repository:**
    ```bash
    git clone https://github.com/your-username/Horizon.git
    ```
2.  **Mở project** bằng Visual Studio 2022.
3.  **Cấu hình Chuỗi kết nối:**
    - Mở file `appsettings.json`.
    - Thay đổi chuỗi kết nối trong `ConnectionStrings` cho phù hợp với môi trường SQL Server của bạn.
4.  **Tạo Cơ sở dữ liệu:**
    - Mở `Package Manager Console`.
    - Chạy lệnh: `Update-Database`.
5.  **Chạy ứng dụng:**
    - Nhấn `F5` hoặc nút "Start Debugging".
    - Ứng dụng sẽ tự động chạy `SeedData` để tạo vai trò và tài khoản Admin mặc định.
    - **Tài khoản Admin:**
        - **Email:** `admin@horizon.com`
        - **Password:** `Admin@123`

---
Cảm ơn đã xem qua đồ án của tôi!
#  Horizon 🚀 - E-commerce Website for Gamers

![Horizon Showcase]([link_den_anh_banner_dep_nhat_cua_ban])

**Horizon** is a final-term project, a fully functional e-commerce web application built in 10 days using ASP.NET Core 8. The project is designed for selling gaming consoles, game discs, and accessories, with a unique UI/UX inspired by the high-tech, dark-mode aesthetic of the Binance trading platform.

---

## ✨ Key Features

The application is architected with a clean separation between the customer-facing storefront and the powerful admin panel, utilizing ASP.NET Core's **Areas**.

### 👤 Customer Area

- **Themed UI/UX:** A complete visual overhaul inspired by Binance, featuring a dark-mode interface with a black and gold accent color palette for a premium, modern feel.
- **Interactive Homepage:**
    - Displays curated sections for **Featured Collections**, **On Sale Items**, and **Newest Arrivals**.
    - A unique **Chibi Animation Stage** where animated characters patrol. Clicking on a character reveals a dialogue box.
    - **Background Music Player** with a toggle button to enhance the immersive experience.
- **Seamless Shopping Flow:**
    - **Product Browsing:** A dedicated Shop page to view all products.
    - **Related Products:** The product details page suggests other items from the same category to encourage further exploration.
    - **Shopping Cart:** Fully functional cart using `Session` storage, allowing users to add, update quantities, and remove items.
    - **Order Placement:** A secure checkout process that requires authentication, records the order in the database, and updates product stock in real-time.
- **User Authentication:**
    - Secure user Registration and Login system powered by ASP.NET Core Identity.
    - Custom-designed, visually appealing Login and Register pages with animated video backgrounds.
- **Live Interaction:** Integrated **Tawk.to** live chat widget for instant customer support.

### 🔐 Admin Area

- **Secure & Role-Based Access:** The entire admin panel is protected. Only users with the "Admin" role can access its functionalities.
- **Comprehensive Dashboard:**
    - **At-a-glance Statistics:** Displays key metrics like Total Revenue, Total Orders, Total Products, and Total Customers.
    - **Data Visualization:** Utilizes **Chart.js** to render interactive charts: a doughnut chart for product distribution by category and a bar chart for daily revenue over the last 7 days.
- **Content Management (CRUD):**
    - **Product Management:** Full CRUD capabilities for products, including a flexible image uploader (local file or external URL).
    - **Category Management:** Full CRUD for product categories.
    - **Order Management:** A dedicated interface for admins to view all customer orders, check details, and update their status (e.g., from "Processing" to "Shipped").
- **Advanced Filtering:** Admins can filter the order list by month and year to easily track past transactions.

---

## 🛠️ Technology Stack

| Category | Technology / Tool |
| :--- | :--- |
| **Backend** | .NET 8, ASP.NET Core 8 MVC, ASP.NET Core Identity, Entity Framework Core 8 |
| **Frontend** | HTML5, CSS3, JavaScript, Bootstrap 5, Chart.js, jQuery |
| **Database** | Microsoft SQL Server |
| **Dev Tools** | Visual Studio 2022, Git & GitHub, IIS Express |
| **Third-party**| Tawk.to Live Chat |

---

## ⚙️ Setup and Installation

To run this project locally, follow these steps:

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/your-username/Horizon.git
    ```
2.  **Open the project** in Visual Studio 2022.
3.  **Configure the Connection String:**
    - Open the `appsettings.json` file.
    - Modify the `DefaultConnection` string under `ConnectionStrings` to match your local SQL Server instance.
4.  **Create and Seed the Database:**
    - Open the **Package Manager Console**.
    - Run the command: `Update-Database`. This will create the database and all necessary tables.
5.  **Run the application:**
    - Press `F5` or the "Start Debugging" button.
    - The application will automatically seed the database with "Admin" and "Customer" roles, and a default admin account will be created.

---

### 🔑 Default Admin Account

-   **Email:** `admin@horizon.com`
-   **Password:** `Admin@123`

---

## 📸 Screenshots

*(This is the perfect place to showcase your hard work!)*

**Customer Homepage**
![Customer Homepage]([link_den_anh_trang_chu])

**Product Details Page**
![Product Details Page]([link_den_anh_trang_chi_tiet])

**Admin Dashboard**
![Admin Dashboard]([link_den_anh_dashboard])

**Shopping Cart**
![Shopping Cart]([link_den_anh_gio_hang])
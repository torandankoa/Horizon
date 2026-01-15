# HORIZON - Premium Tactical E-commerce Platform

[![Live Demo](https://img.shields.io/badge/demo-os--horizon.site-blue?style=for-the-badge&logo=google-chrome&logoColor=white)](https://os-horizon.site/)
[![Platform](https://img.shields.io/badge/.NET-8.0-512bd4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Database](https://img.shields.io/badge/SQL_Server-2022-cc2927?style=for-the-badge&logo=microsoft-sql-server)](https://www.microsoft.com/en-us/sql-server/)

## Introduction
**Horizon** is a comprehensive Business-to-Consumer (B2C) e-commerce system focusing on the niche market of Gaming gear and Tactical-style accessories. The project was built with the objective of not only creating a unique shopping interface but also integrating real-world operational strategies: from secure payments and SEO optimization to data-driven logistics management.

> **Slogan:** *"Walk Past The Horizon."*

---

## Strategic Core Features

### 1. Operations & Payment (E-commerce Core)
*   **VnPay Protocol Integration:** Integrated with the national payment gateway, automating verification processes, real-time inventory deduction, and transaction logging (`Transactions`).
*   **Logistics & Inventory Management:** A `Product Receipts` system records cost prices and procurement history, enabling precise net profit calculations.
*   **Cart System (Cargo Manifest):** Utilizes Session JSON Serialization to optimize performance and store complex data objects.

### 2. Digital Marketing & SEO Optimization
*   **URL Friendly (Slug):** SEO-Friendly URLs (Slugs): Automated generation of keyword-rich slugs from product names, optimized for Google search results.
*   **Social Marketing:** Implementation of **Open Graph (OG Tags)** for professional display of images, prices, and descriptions when shared on Facebook/Zalo.
*   **Email Marketing:** Integrated Mailchimp for Lead Generation and executing "Intel Briefing" newsletter campaigns.

### 3. User Experience & CRM
*   **QRF Support (Livechat):** Embedded **Tawk.to** system to support customers 24/7 in real-time.
*   **Behavioral Analytics:** Utilizes **Google Analytics 4 (GA4)** to monitor traffic flow and optimize the sales funnel.
*   **Modern Light Interface:**  Minimalist design focused on the product, ensuring high-speed responsiveness (latency < 2s).

---

## Tech Stack

| Component | Deployed Technology |
| :--- | :--- |
| **Backend** | C# / ASP.NET Core 8.0 (MVC) |
| **ORM** | Entity Framework Core 8 (Code-First) |
| **Identity** | ASP.NET Core Identity (Role-based Authorization) |
| **Frontend** | Bootstrap 5, CSS3 Variables, JavaScript, Chart.js |
| **3rd Party Services** | VnPay Gateway, Mailchimp, Tawk.to, GA4 |
| **Cloud Infrastructure** | SmarterASP Hosting, TenTen DNS, SSL (Let's Encrypt) |

---

## System Architecture
The project adopts the **Areas** structure to decouple Administrative and Customer business logic:
- **`Area/Admin`**: Command Center (Dashboard, Revenue Statistics, Inventory Management, Order Status Updates).
- **`Area/Customer`**: Online Storefront (Search, Sidebar Filters, Secure Checkout Workflow).
- **`Infrastructure`**: Contains advanced helper modules (`SlugHelper`, `VnPayLibrary`, `SessionExtensions`).

---

## Key Learnings
- Mastered the (**End-to-End Development**) lifecycle from conceptualization to Cloud deployment.
- Developed a mindset for designing robust Relational Databases for financial transaction systems.
- Gained expertise in integrating and handling APIs/SDKs from professional third-party providers.
- Optimized application performance and web security following OWASP standards.

---

## Contact Information
- **Full Name:** [Trần Đăng Khoa]
- **Email:** [khoatran04.it@gmail.com]
- **Project Domain:** [https://os-horizon.site/](https://os-horizon.site/)
- **LinkedIn:** []

---
*© 2025 Horizon Project - Built with passion and code.*

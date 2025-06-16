# School Meal Coupon Booking System
A modern ASP.NET Core MVC platform for managing school meal coupon bookings, designed to streamline communication between parents, kitchen staff, and administrators.

## Key Features

### Role-Based Access Control
- **Three Distinct Roles**:
  - **Admin**: Manages users, monitors meal preparation stats, and oversees system operations
  - **Cook**: Views daily meal requirements and manages kitchen workflow
  - **Parent**: Books meal coupons and manages payment methods

### Core Functionality
- Secure authentication via **Google's API**
- User-friendly interface with responsive design
- SQL Server database integration for reliable data storage

### Upcoming Features (Planned Enhancements)
- 🗓 **Interactive Booking Calendar**
  - Visual date selection interface
  - Meal availability indicators
  - Conflict prevention system
- 💳 **Parent Financial Portal**
  - Secure payment card registration
  - Transaction history tracking
  - Account balance monitoring
- 📊 **Administration Dashboard**
  - Real-time meal demand analytics
  - PDF export capabilities
  - User management interface
- 🏦 **Integrated Banking System**
  - Virtual wallet functionality
  - Automated payment processing
  - Low-balance notifications

## Technology Stack

| Component                | Technology                          |
|--------------------------|-------------------------------------|
| **Framework**            | ASP.NET Core 9.0 (MVC)              |
| **Authentication**       | Google's API                        |
| **Database**             | MS SQL Server 2022                  |
| **ORM**                  | Entity Framework Core 9             |
| **Frontend**             | Razor Views                         |

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server 2022
- Google Cloud account (for OAuth configuration)

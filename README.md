# Hostel2.0 - Modern Hostel Management System

A comprehensive hostel management system built with ASP.NET Core MVC, designed to streamline hostel operations and enhance the experience for both managers and students.

## Features

- Multi-role system (Admin, Hostel Manager, Student)
- Secure authentication and authorization
- Profile management
- Room allocation and management
- Student registration and management
- Notice board system
- Maintenance request and tracking
- Payment management (multiple methods, receipts, history)
- Modern, responsive UI/UX

## Screenshots

Below are some screenshots showcasing the modern UI and features of Hostel2.0:

| Dashboard | Student Profile | Maintenance Request | Payment | Manager Dashboard |
|-----------|----------------|--------------------|---------|-------------------|
| ![Dashboard](screenshort/Screenshot%202025-05-24%20043104.png) | ![Profile](screenshort/Screenshot%202025-05-24%20043031.png) | ![Maintenance](screenshort/Screenshot%202025-05-24%20043019.png) | ![Payment](screenshort/Screenshot%202025-05-24%20042947.png) | ![Manager](screenshort/Screenshot%202025-05-24%20042924.png) |

## Technology Stack

- **Backend**: ASP.NET Core 8.0
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap 5
- **Database**: SQL Server
- **Authentication**: ASP.NET Core Identity
- **Payment Integration**: Stripe
- **Email Service**: SendGrid
- **SMS Service**: Twilio

## Prerequisites

- .NET 8.0 SDK
- SQL Server 2019 or later
- Visual Studio 2022 or VS Code
- Git

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/CodeByMoheb/Hostel2.0.git
   ```

2. Navigate to the project directory:
   ```bash
   cd Hostel2.0
   ```

3. Update the connection string in `appsettings.json` with your SQL Server details.

4. Run the database migrations:
   ```bash
   dotnet ef database update
   ```

5. Run the application:
   ```bash
   dotnet run
   ```

## Configuration

Update the following settings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_Connection_String"
  },
  "SendGrid": {
    "ApiKey": "Your_SendGrid_API_Key",
    "FromEmail": "your-email@domain.com",
    "FromName": "Hostel2.0"
  },
  "Twilio": {
    "AccountSid": "Your_Twilio_Account_SID",
    "AuthToken": "Your_Twilio_Auth_Token",
    "FromNumber": "Your_Twilio_Phone_Number"
  },
  "Stripe": {
    "SecretKey": "Your_Stripe_Secret_Key",
    "PublishableKey": "Your_Stripe_Publishable_Key"
  }
}
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

Moheb - @CodeByMoheb

Project Link: [https://github.com/CodeByMoheb/Hostel2.0](https://github.com/CodeByMoheb/Hostel2.0) 
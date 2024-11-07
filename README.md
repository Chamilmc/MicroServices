Microservices Project with .NET 8
Project Overview
This project demonstrates a scalable microservices architecture for an e-commerce platform, built with .NET 8 using a SQL Server code-first approach. The architecture includes several independently managed services, each responsible for a different core functionality, from handling discounts to managing authentication, product data, shopping cart, and notifications.

Microservices Included:
Coupon API: Manages discount coupons.
Auth API: For managing user registration, authentication, and authorization.
Product API: Manages products.
Shopping Cart API: Handles shopping cart operations.
Service Bus: Facilitates communication between services.
Email API: Manages transactional email notifications.

Architecture
Each microservice in this solution is independently deployable, following best practices for a distributed system. Azure Service Bus is used to manage communication and event-driven interactions between services, ensuring scalability and reliability.

Technologies Used
.NET 8 - Modern and robust platform for building microservices.
Entity Framework Core - ORM for database access using Code First migrations.
Azure Service Bus - For event-driven communication between services.

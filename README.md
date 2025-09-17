# 💼 Freelancer Marketplace

A full-stack freelancer marketplace web application built with ASP.NET 8 Web API and Angular 18. Users can register, login, and participate with three distinct roles: Administrator, Client, and Freelancer. Freelancers can create profiles, manage portfolios, browse and apply for projects, and chat with clients in real-time. Clients can post projects, manage proposals, and collaborate with freelancers. Admins can manage users and enforce platform rules.

## 🚀 Features
### ✅ User Registration & Login
- Secure authentication using JWT tokens
- Role-based authorization (Admin / Client / Freelancer)

### ✅ Freelancer Features
- Profile management with editable details
- Create, edit, delete portfolio items (title, description, photo) with pagination
- Browse projects and filter by required skills (5 distinct skills available)
- Submit proposals for projects
- View submitted proposals (filter by Approved / Rejected / Pending)
- Approved proposals automatically generate active projects and enable real-time chat with clients
- Messaging system with unread counters, last message preview, timestamps, and delete message option (local delete only, with warning)

### ✅ Client Features
- Profile management with editable details
- Create, edit, delete projects with pagination
- View submitted proposals from freelancers (accept/reject)
- Manage active projects (approved freelancer collaborations)
- Messaging system identical to freelancer’s

### ✅ Admin Tools
- View all users in admin panel with profile info + live presence indicator (SignalR)
- Enable or disable user accounts
- Disabled users see a modal on login with instructions to contact the admin

### ✅ User Experience

- Toaster notifications for actions and updates
- Confirmation modals for critical actions
- Unsaved changes guard on profile and portfolio/project forms
- Presence indicator showing who is online
- Autoscroll chat windows and navigation modals to prevent data loss

## 🛠 Tech Stack
ASP.NET 8 Web API Angular 18 SignalR SQLite JWT tokens Cloudinary (photos)

## 🗂 Project Structure
### API (/FreelancerMarketplace/API)
- Controllers/ → REST API controllers: AccountController, UsersController, PortfolioItemController, ProjectController, ProposalsController, ProjectConversationController, Admin actions, etc.
- Data/ → EF Core DbContext (DataContext.cs), Repositories, Seed data
- Entities/ → Domain models: AppUser, AppRole, Project, Proposal, PortfolioItem, Message, Skill, Photo
- DTOs/ → Data transfer objects: MemberDTO, ProjectDTO, ProposalDTO, PortfolioItemDTO, MessageDTO, etc.
- SignalR/ → Real-time hubs: MessageHub, PresenceHub
- Services/ → TokenService, PhotoService
- Helpers/ → Pagination, AutoMapper profiles, parameter classes
- Middleware/ → ExceptionMiddleware

### Client (/FreelancerMarketplace/app)
#### Components
- nav/ → Separate navbars for roles: Admin, Client, Freelancer, Public
- profile/, profile-edit-client/, profile-edit-freelancer → Shared profile view/edit components for users
- portfolio-item/, portfolio-list/, portfolio-item-create → Freelancer portfolio management
- profile-project-item/, profile-project-list /, project-create → Client project management
- browse-projects/ → Client project creation + editing, Freelancer project browsing
- create-proposals/ → Submitted proposals (freelancer) + proposal inbox (client)
- active-projects/ → Active projects page
- messages/, chat/ → Messaging and conversation components
- admin-panel/ → User management, account enable/disable (admin only)
- register/, home/, /date-picker, /text-input → Public entry components and auxiliary components for them
- errors/ → Not found, server error, test error components
- modal/, confirm-dialog/ → Reusable dialogs and notifications
  
#### Guards, Services, Interceptors, Models, DTOs, Pipes and Forms
- _guards/ → Prevent disabled users, prevent unsaved changes
- _services/ → Angular services for account, project, proposals, portfolio items, messages, presence
- _interceptors/ interceptors/ → JWT, error, loading interceptors
- _models/ _DTOs → Interfaces for User, Project, Proposal, PortfolioItem, Message, Photo, etc...
- pipes/ → Custom pipes (time-ago, etc.)
- forms/ → Shared form components (text-input, date-picker)

## ⚙ Getting Started
Prerequisites
- .NET 8 SDK
- Node.js (18+) + NPM
- Angular CLI
- SQLite

### Clone & Setup
```bash
git clone <your-repo-url>
cd <your-repo-folder>
```

### API Setup
```bash
cd FreelancerMarketplace/API
dotnet restore
dotnet ef database update
dotnet run
```

API runs at: https://localhost:5001/

Angular Client Setup

```bash
cd FreelancerMarketplace/client
npm install
ng serve --open
```

Client runs at: http://localhost:4200/

## ⚡ Usage
- Register/Login: Choose role (Client/Freelancer) during registration.
- Freelancers: Build portfolio, browse projects, submit proposals, chat with clients.
- Clients: Create/manage projects, review proposals, accept/reject freelancers, manage active projects.
- Messaging: Real-time chat between accepted freelancer-client pairs, notifications + unread counters.
- Admin: Monitor user presence, enable/disable accounts.
- UX: Notifications, modals, unsaved-change guards, online indicators.

## 🔐 Security
- JWT Bearer tokens for API calls
- Role-based access control (Admin / Client / Freelancer)
- SignalR with token authentication
- Client-side guards for disabled accounts & protected routes
- Server-side validation for uploaded images

## 💡 Development Notes
- API uses EF Core Code First migrations
- Angular uses standalone components (non-nested for simplicity)
- Cloudinary is integrated for image storage


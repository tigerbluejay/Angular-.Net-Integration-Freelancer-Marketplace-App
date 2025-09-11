import { Routes } from '@angular/router';

// Components
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ProfileComponent } from './profile/profile.component';
import { ProfileEditFreelancerComponent } from './profile-edit-freelancer/profile-edit-freelancer.component';
import { ProfileEditClientComponent } from './profile-edit-client/profile-edit-client.component';
import { PortfolioItemCreateComponent } from './portfolio-item-create/portfolio-item-create.component';
import { ProjectCreateComponent } from './project-create/project-create.component';
import { BrowseProjectsComponent } from './browse-projects/browse-projects.component';
import { ProposalCreateComponent } from './create-proposal/create-proposal.component';
import { SubmittedProposalsComponent } from './submitted-proposals/submitted-proposals.component';
import { ProposalInboxComponent } from './proposal-inbox/proposal-inbox.component';
import { ActiveProjectsComponent } from './active-projects/active-projects.component';
import { MessagesComponent } from './messages/messages.component';
import { ChatComponent } from './chat/chat.component';
import { AdminPanelComponent } from './admin-panel/admin-panel.component';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';

// Guards
import { authGuard } from './auth.guard';
import { accountGuard } from './_guards/prevent-disabled-users-to-use-the-platform.guard';
import { preventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';

export const routes: Routes = [
  // Public routes
  { path: '', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'errors', component: TestErrorsComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: 'server-error', component: ServerErrorComponent },

  // Authenticated routes (wrapper)
  {
    path: '',
    canActivate: [authGuard], // ensure user is logged in
    canActivateChild: [accountGuard], // ensure account is active on every child route
    children: [
      { path: 'profile/:username', component: ProfileComponent },

      // Profile edit
      { path: 'profile-edit-freelancer', component: ProfileEditFreelancerComponent, canDeactivate: [preventUnsavedChangesGuard] },
      { path: 'profile-edit-client', component: ProfileEditClientComponent, canDeactivate: [preventUnsavedChangesGuard] },

      // Projects
      { path: 'portfolio-item/create', component: PortfolioItemCreateComponent },
      { path: 'portfolio-item/edit/:id', component: PortfolioItemCreateComponent, canDeactivate: [preventUnsavedChangesGuard] },
      { path: 'project/create', component: ProjectCreateComponent },
      { path: 'project/edit/:id', component: ProjectCreateComponent, canDeactivate: [preventUnsavedChangesGuard]  },
      { path: 'browse-projects', component: BrowseProjectsComponent },

      // Proposals
      { path: 'create-proposal/:id', component: ProposalCreateComponent },
      { path: 'submitted-proposals', component: SubmittedProposalsComponent },
      { path: 'proposal-inbox', component: ProposalInboxComponent },

      // Projects & messaging
      { path: 'active-projects', component: ActiveProjectsComponent },
      { path: 'chat/:projectId', component: ChatComponent },
      { path: 'messages', component: MessagesComponent },

      // Admin
      { path: 'admin-panel', component: AdminPanelComponent },
    ],
  },

  // Fallback
  { path: '**', component: HomeComponent, pathMatch: 'full' },
];
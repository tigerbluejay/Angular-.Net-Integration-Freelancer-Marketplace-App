import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ProfileComponent } from './profile/profile.component';
import { authGuard } from './auth.guard';  // You may not have this yet
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { ProfileEditFreelancerComponent } from './profile-edit-freelancer/profile-edit-freelancer.component';
import { preventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { ProfileEditClientComponent } from './profile-edit-client/profile-edit-client.component';
import { PortfolioItemCreateComponent } from './portfolio-item-create/portfolio-item-create.component';
import { ProjectCreateComponent } from './project-create/project-create.component';
import { BrowseProjectsComponent } from './browse-projects/browse-projects.component';
import { ProposalCreateComponent } from './create-proposal/create-proposal.component';
import { SubmittedProposalsComponent } from './submitted-proposals/submitted-proposals.component';
import { ProposalInboxComponent } from './proposal-inbox/proposal-inbox.component';
import { ActiveProjectsComponent } from './active-projects/active-projects.component';


export const routes: Routes = [
  // Public routes
  { path: '', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'errors', component: TestErrorsComponent},
  { path: 'not-found', component: NotFoundComponent},
  { path: 'server-error', component: ServerErrorComponent},
  { path: 'profile-edit-freelancer', component: ProfileEditFreelancerComponent, 
    canDeactivate: [preventUnsavedChangesGuard]},
  { path: 'profile-edit-client', component: ProfileEditClientComponent, 
    canDeactivate: [preventUnsavedChangesGuard]},
  { path: 'portfolio-item/create', component: PortfolioItemCreateComponent },
  { path: 'portfolio-item/edit/:id', component: PortfolioItemCreateComponent },
  { path: 'project/create', component: ProjectCreateComponent },
  { path: 'project/edit/:id', component: ProjectCreateComponent },
  { path: 'create-proposal/:id', component: ProposalCreateComponent },
  { path: 'submitted-proposals', component: SubmittedProposalsComponent },
  { path: 'proposal-inbox', component: ProposalInboxComponent },
  { path: 'active-projects', component: ActiveProjectsComponent, canActivate: [authGuard] },

  // ✅ New freelancer route
  { path: 'browse-projects', component: BrowseProjectsComponent, canActivate: [authGuard] },

  // Protected route
  { path: 'profile/:username', component: ProfileComponent, canActivate: [authGuard]  },

  // fallback
  { path: '**', component: HomeComponent, pathMatch: 'full'},
];
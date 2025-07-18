import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ProfileComponent } from './profile/profile.component';
import { authGuard } from './auth.guard';  // You may not have this yet

export const routes: Routes = [
  // Public routes
  { path: '', component: HomeComponent },
  { path: 'register', component: RegisterComponent },

  // Protected route
  { path: 'profile', component: ProfileComponent, canActivate: [authGuard] },

  // Wildcard fallback
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
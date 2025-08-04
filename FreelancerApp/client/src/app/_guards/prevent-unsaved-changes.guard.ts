
import { CanDeactivateFn } from '@angular/router';
import { ProfileEditFreelancerComponent } from '../profile-edit-freelancer/profile-edit-freelancer.component';

export const preventUnsavedChangesGuard: CanDeactivateFn<ProfileEditFreelancerComponent> = (component) => {
  if (component.editForm?.dirty) {
  return confirm ('Are you sure you want to continue? Any unsaved changes will be lost.')
  }
  return true;
};
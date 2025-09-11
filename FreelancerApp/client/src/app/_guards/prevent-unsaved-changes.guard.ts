import { inject } from "@angular/core";
import { ConfirmService } from "../_services/confirm.service";
import { CanDeactivateFn } from "@angular/router";
import { HasEditForm } from "../_interfaces/has-edit-form.interface";

export const preventUnsavedChangesGuard: CanDeactivateFn<HasEditForm> = (component) => {
  console.log('Guard triggered for component:', component);

  const confirmService = inject(ConfirmService);

  if (component.editForm?.dirty) {
    console.log('Form is dirty, calling confirm modal');
    const result$ = confirmService.confirm(
      'Unsaved changes',
      'You have unsaved changes. Do you really want to leave this page?',
      'Leave',
      'Stay'
    );

    console.log('ConfirmService returned observable:', result$);
    return result$;
  }

  console.log('Form not dirty, navigation allowed');
  return true;
};
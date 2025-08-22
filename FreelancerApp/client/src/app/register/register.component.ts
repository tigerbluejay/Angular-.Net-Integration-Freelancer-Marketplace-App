import { AfterViewInit, ChangeDetectorRef, Component, inject, input, OnInit, output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { JsonPipe, NgIf } from '@angular/common';
import { TextInputComponent } from '../text-input/text-input.component';
import { DatePickerComponent } from '../date-picker/date-picker.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, TextInputComponent, DatePickerComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {

  private accountService = inject(AccountService);
  private fb = inject(FormBuilder);
  usersFromHomeComponent = input.required<any>() // receives input from parent Home Component
  cancelRegister = output<boolean>(); // child to parent communication
  // 1. create the cancelRegister variable of type output in the child component
  // 2. create the cancel method in the child component to emit a value in the child component
  // 3. register the output value in the child component tag in the parent component
  // 4. create the function referenced in 3 in the parent (Home component)
  private router = inject(Router);
  registerForm: FormGroup = new FormGroup({});
  maxDate = new Date();
  validationErrors: string[] | undefined;

  ngOnInit(): void {
  this.initializeForm();
  this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);

  // Diagnostic logging
  Object.keys(this.registerForm.controls).forEach(key => {
    const ctrl = this.registerForm.get(key);
    console.log(`${key}: pristine=${ctrl?.pristine}, touched=${ctrl?.touched}, value=`, ctrl?.value);
    ctrl?.valueChanges.subscribe(val => {
      console.log(`Value changed for ${key}:`, val);
    });
  });
}

  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],
      role: ['Freelancer', Validators.required] // <-- default to freelancer
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }


  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : { isMatching: true }
    }
  }

register() {
  const dobControl = this.registerForm.get('dateOfBirth')?.value;

  let dob: string | null = null;
  if (dobControl) {
    // Ensure it's a string in 'YYYY-MM-DD' format
    const date = dobControl instanceof Date ? dobControl : new Date(dobControl);
    dob = date.toISOString().slice(0, 10);
  }

  const registerPayload = {
    ...this.registerForm.value,
    dateOfBirth: dob
  };

  this.accountService.register(registerPayload).subscribe({
    next: (user) => {
      this.accountService.setCurrentUser(user);
      this.router.navigate(['/profile', user.username]);
    },
    error: (error) => this.validationErrors = error
  });
}
  cancel() {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined) {
    if (!dob) return;
    return new Date(dob).toISOString().slice(0, 10)
  }

}

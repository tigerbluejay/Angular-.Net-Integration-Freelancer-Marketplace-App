import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  private accountService = inject(AccountService);
  usersFromHomeComponent = input.required<any>() // receives input from parent Home Component
  cancelRegister = output<boolean>(); // child to parent communication
  // 1. create the cancelRegister variable of type output in the child component
  // 2. create the cancel method in the child component to emit a value in the child component
  // 3. register the output value in the child component tag in the parent component
  // 4. create the function referenced in 3 in the parent (Home component)
  private toastr = inject(ToastrService);

  model: any = {}

  register() {
    this.accountService.register(this.model).subscribe({
      next: response => {
        console.log(response);
        this.cancel();
      },
      error: error => this.toastr.error(error.error)
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
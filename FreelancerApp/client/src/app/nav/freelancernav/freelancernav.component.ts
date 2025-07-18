import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-freelancernav',
  standalone: true,
  imports: [FormsModule, NgIf, BsDropdownModule],
  templateUrl: './freelancernav.component.html',
  styleUrl: './freelancernav.component.css'
})
export class FreelancernavComponent {
accountService = inject(AccountService);
model:any = {};

  login() {
    this.accountService.login(this.model).subscribe({
      next: response => { console.log(response);},
      error: error => console.log(error)
    })
  }

  logout() {
    this.accountService.logout();
  }

}

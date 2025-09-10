// modal.component.ts
import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ModalService } from '../_services/modal.service';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';  // <-- import Router

@Component({
  standalone: true,
  selector: 'app-modal',
  imports: [CommonModule],
  template: `
    <div class="modal fade" tabindex="-1" id="disabledAccountModal">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content shadow-lg">
          <div class="modal-header bg-danger text-white">
            <h5 class="modal-title">Account Disabled</h5>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <p>Please contact the admin at admin&#64;example.com to re-enable your account.</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-primary btn-ok" (click)="handleOk()">OK</button>
          </div>
        </div>
      </div>
    </div>
  `,
})
export class ModalComponent {
  @Input() id!: string;
  @Input() title: string = 'Notice';
  @Input() message: string = '';

  isOpen = false;

  constructor(
    private modalService: ModalService,
    private accountService: AccountService,
    private router: Router  // <-- inject Router here
  ) { }

  ngOnInit() {
  }

  open() {
    this.isOpen = true;
  }

  close() {
    this.isOpen = false;
  }

  // <-- This is the handleOk() method you mentioned
  handleOk() {
    this.close();
    this.accountService.logout(); // log out the user
    // navigate after modal closes
    setTimeout(() => {
      this.router.navigate(['/']);
    }, 200);  // 200ms is enough for modal fade-out
  }
}
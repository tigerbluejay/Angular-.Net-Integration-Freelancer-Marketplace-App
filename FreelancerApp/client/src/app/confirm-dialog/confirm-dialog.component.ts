import { Component, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html'
})
export class ConfirmDialogComponent {
  title!: string;
  message!: string;
  btnOkText!: string;
  btnCancelText!: string;

  confirmResult = new EventEmitter<boolean>();

  constructor(public bsModalRef: BsModalRef) {}

  confirm() {
    this.confirmResult.emit(true); // Leave
    console.log('User clicked OK');
    this.bsModalRef.hide();
  }

  decline() {
    this.confirmResult.emit(false); // Stay
    console.log('User clicked Cancel');
    this.bsModalRef.hide();
  }
}
import { Injectable } from '@angular/core';
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';
import { Observable, of } from 'rxjs';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';

@Injectable({ providedIn: 'root' })
export class ConfirmService {
  bsModalRef: BsModalRef<ConfirmDialogComponent> | undefined;
  constructor(private modalService: BsModalService) { }

  confirm(
    title = 'Confirmation',
    message = 'Are you sure you want to do this?',
    btnOkText = 'Ok',
    btnCancelText = 'Cancel'
  ): Observable<boolean> {
    console.log('ConfirmService.confirm called with:', { title, message, btnOkText, btnCancelText });

    const config: ModalOptions = { initialState: { title, message, btnOkText, btnCancelText } };
    this.bsModalRef = this.modalService.show(ConfirmDialogComponent, config);
    console.log('ModalService.show called, bsModalRef:', this.bsModalRef);

    if (this.bsModalRef?.content?.confirmResult) {
      console.log('Returning confirmResult observable');
      return this.bsModalRef.content.confirmResult.asObservable();
    }

    console.log('No confirmResult found, returning false observable');
    return of(false);
  }
}
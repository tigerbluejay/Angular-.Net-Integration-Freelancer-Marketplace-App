import { Injectable } from '@angular/core';
import { AccountService } from './account.service';

declare var bootstrap: any;

@Injectable({
  providedIn: 'root',
})
export class ModalService {
  private okCallbacks: { [id: string]: () => void } = {};

  constructor(private accountService: AccountService) { }

  open(id: string, title?: string, message?: string, onOk?: () => void) {
    // Save the callback if provided
    if (onOk) {
      this.okCallbacks[id] = onOk;
    } else {
      // Default OK behavior: log out
      this.okCallbacks[id] = () => this.accountService.logout();
    }

    const modalEl = document.getElementById(id);
    if (modalEl) {
      if (title) modalEl.querySelector('.modal-title')!.textContent = title;
      if (message) modalEl.querySelector('.modal-body p')!.textContent = message;

      // Attach OK button handler
      // Attach OK button handler
      const okBtn = modalEl.querySelector('.btn-ok') as HTMLButtonElement | null;
      if (okBtn) {
        okBtn.onclick = () => {
          this.close(id);
          this.okCallbacks[id]?.();
          delete this.okCallbacks[id]; // cleanup
        };
      }

      const modal = new bootstrap.Modal(modalEl);
      modal.show();
    }
  }

  close(id: string) {
    const modalEl = document.getElementById(id);
    if (modalEl) {
      const modal = bootstrap.Modal.getInstance(modalEl);
      modal?.hide();
    }
  }
}
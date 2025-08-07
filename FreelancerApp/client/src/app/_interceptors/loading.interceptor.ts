import { HttpInterceptorFn } from '@angular/common/http';
import { BusyService } from '../_services/busy.service';
import { delay, finalize } from 'rxjs';
import { inject } from '@angular/core';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  // Check if the request has the 'skip-spinner' header
  const skipSpinner = req.headers.has('skip-spinner');

  // If skipping spinner, remove the header before sending to server
  const request = skipSpinner ? req.clone({
    headers: req.headers.delete('skip-spinner')
  }) : req;

  // Only call busyService.busy() if NOT skipping spinner
  if (!skipSpinner) {
    busyService.busy();
  }

  return next(request).pipe(
    delay(1000),
    finalize(() => {
      if (!skipSpinner) {
        busyService.idle();
      }
    })
  );
};
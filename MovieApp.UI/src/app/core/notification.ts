import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';

export interface NotificationMessage {
  message: string;
  type: 'success' | 'error';
}

@Injectable({
  providedIn: 'root'
})

export class Notification {
  private notificationSubject = new Subject<NotificationMessage | null>();

  get notification$(): Observable<NotificationMessage | null> {
    return this.notificationSubject.asObservable();
  }

  showSuccess(message: string): void {
    this.notificationSubject.next({ message, type: 'success' });
    this.clearAfterDelay();
  }

  showError(message: string): void {
    this.notificationSubject.next({ message, type: 'error' });
    this.clearAfterDelay();
  }

  private clearAfterDelay(): void {
    console.log("cleared");
    setTimeout(() => {
      this.notificationSubject.next(null);
    }, 4000);
  }
}
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { Notification, NotificationMessage } from '../../core/notificationService';

@Component({
  selector: 'app-notification',
  imports: [CommonModule],
  templateUrl: './notification.html',
  styleUrl: './notification.css',
})
export class NotificationComponent  {

  notification: NotificationMessage | null = null;

  constructor(private notificationService: Notification ,private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.notificationService.notification$.subscribe(notification => {
      this.notification = notification;
       this.cdr.markForCheck();
    });
  }

}

import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { RouteConfigLoadEnd, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { NotificationComponent } from './notification/notification/notification';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet , RouterLinkActive ,RouterLink ,NotificationComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('MovieApp.UI');
}

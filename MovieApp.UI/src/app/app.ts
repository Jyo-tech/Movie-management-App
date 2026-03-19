import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { RouteConfigLoadEnd, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet , RouterLinkActive ,RouterLink ,CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('MovieApp.UI');
}

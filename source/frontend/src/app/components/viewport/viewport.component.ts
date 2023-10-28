import {Component, isDevMode} from '@angular/core';
import {CommonModule} from '@angular/common';

@Component({
  selector: 'app-viewport',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './viewport.component.html',
  styleUrl: './viewport.component.scss'
})
export class ViewportComponent {
  public isDevelopment: boolean;

  constructor() {
    this.isDevelopment = isDevMode();
  }
}

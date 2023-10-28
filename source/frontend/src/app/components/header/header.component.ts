import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {ApiService} from "../../services/api.service";

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {

  protected readonly ApiService = ApiService;
}

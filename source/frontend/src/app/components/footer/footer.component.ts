import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {Observable} from "rxjs";
import {ApiService} from "../../services/api.service";

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})
export class FooterComponent {
  public productName$: Observable<string>;
  public test?: string;

  constructor(
    apiService: ApiService,
  ) {
    this.productName$ = apiService.getProductName();
  }
}

import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { PortfolioItemComponent } from '../portfolio-item/portfolio-item.component';
import { PortfolioItem } from '../_models/portfolio-item';

@Component({
  selector: 'app-portfolio-list',
  standalone: true,
  imports: [CommonModule, PortfolioItemComponent],
  templateUrl: './portfolio-list.component.html',
  styleUrl: './portfolio-list.component.css'
})
export class PortfolioListComponent {
  @Input() items: PortfolioItem[] = [];
}

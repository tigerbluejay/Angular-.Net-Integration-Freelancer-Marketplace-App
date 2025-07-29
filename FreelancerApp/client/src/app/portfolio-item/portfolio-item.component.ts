import { Component, Input } from '@angular/core';
import { PortfolioItem } from '../_models/portfolio-item';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-portfolio-item',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './portfolio-item.component.html',
  styleUrl: './portfolio-item.component.css'
})
export class PortfolioItemComponent {
  @Input() item!: PortfolioItem;
}

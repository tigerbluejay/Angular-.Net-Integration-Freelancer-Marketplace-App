import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PortfolioItemComponent } from '../portfolio-item/portfolio-item.component';
import { PortfolioItem } from '../_models/portfolio-item';
import { PortfolioItemDTO } from '../_DTOs/portfolioItemDTO';

@Component({
  selector: 'app-portfolio-list',
  standalone: true,
  imports: [CommonModule, PortfolioItemComponent],
  templateUrl: './portfolio-list.component.html',
  styleUrl: './portfolio-list.component.css'
})
export class PortfolioListComponent {
  @Input() items: PortfolioItemDTO[] = [];
  @Output() delete = new EventEmitter<number>(); // item ID

  onDelete(itemId: number) {
    this.delete.emit(itemId);
  }
}

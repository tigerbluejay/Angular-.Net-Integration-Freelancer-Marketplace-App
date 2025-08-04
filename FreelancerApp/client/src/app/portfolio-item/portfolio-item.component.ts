import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { PortfolioItem } from '../_models/portfolio-item';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-portfolio-item',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './portfolio-item.component.html',
  styleUrl: './portfolio-item.component.css'
})
export class PortfolioItemComponent {
  @Input() item!: PortfolioItem;
  @Output() delete = new EventEmitter<number>();
  private router = inject(Router);  // <-- inject Router here

  onDeleteClick() {
    this.delete.emit(this.item.id);
  }

}

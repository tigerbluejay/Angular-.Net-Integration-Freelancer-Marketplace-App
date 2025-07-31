import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Project } from '../_models/project';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile-project-item',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile-project-item.component.html',
  styleUrl: './profile-project-item.component.css'
})
export class ProfileProjectItemComponent {
   @Input() project!: Project;
   @Output() delete = new EventEmitter<number>();
   
     onDeleteClick() {
       this.delete.emit(this.project.id);
     }
}

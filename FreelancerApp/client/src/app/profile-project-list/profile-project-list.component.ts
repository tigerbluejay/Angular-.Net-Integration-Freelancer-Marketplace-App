import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Project } from '../_models/project';
import { CommonModule } from '@angular/common';
import { ProfileProjectItemComponent } from '../profile-project-item/profile-project-item.component';
import { ProjectDTO } from '../_DTOs/projectDTO';

@Component({
  selector: 'app-profile-project-list',
  standalone: true,
  imports: [CommonModule, ProfileProjectItemComponent],
  templateUrl: './profile-project-list.component.html',
  styleUrl: './profile-project-list.component.css'
})
export class ProfileProjectListComponent {
  @Input() projects: ProjectDTO[] = [];
  @Output() delete = new EventEmitter<number>(); // item ID
  
    onDelete(itemId: number) {
      this.delete.emit(itemId);
    }
}

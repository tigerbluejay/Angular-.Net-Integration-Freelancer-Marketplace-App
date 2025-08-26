import { Component, OnInit } from "@angular/core";
import { ProjectBrowseDTO } from "../_DTOs/projectBrowseDTO";
import { ProjectParams } from "../_models/projectParams";
import { ProjectService } from "../_services/project.service";
import { PaginatedResult } from "../_models/pagination";
import { CommonModule } from "@angular/common";

@Component({
  selector: 'app-browse-projects',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './browse-projects.component.html',
  styleUrls: ['./browse-projects.component.css']
})
export class BrowseProjectsComponent implements OnInit {
  projects: ProjectBrowseDTO[] = [];
  projectParams: ProjectParams = {
    pageNumber: 1,
    pageSize: 8,
    skillNames: [],
    includeAssigned: false,
    matchAllSkills: true
  };
  pagination: any;

  // available filter buttons
  availableSkills: string[] = ['C#', 'ASP.NET Core', 'Angular', 'SQL', 'JavaScript'];

  constructor(private projectService: ProjectService) {}

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects() {
    this.projectService.getProjects(this.projectParams).subscribe({
      next: (res: PaginatedResult<ProjectBrowseDTO[]>) => {
        this.projects = res.result;
        this.pagination = res.pagination;
      },
      error: (err) => console.error(err)
    });
  }

  toggleSkill(skill: string) {
    const idx = this.projectParams.skillNames?.indexOf(skill);
    if (idx === -1 || idx === undefined) {
      this.projectParams.skillNames?.push(skill);
    } else {
      this.projectParams.skillNames?.splice(idx, 1);
    }
    this.projectParams.pageNumber = 1;
    this.loadProjects();
  }

  pageChanged(page: number) {
    if (this.projectParams.pageNumber === page) return;
    this.projectParams.pageNumber = page;
    this.loadProjects();
  }

  isSkillSelected(skill: string) {
    return this.projectParams.skillNames?.includes(skill);
  }
}
export interface ProjectDTO {
  id: number;
  title: string;
  description?: string;
  photoUrl?: string;
  isAssigned: boolean;
  skills: string[];
}
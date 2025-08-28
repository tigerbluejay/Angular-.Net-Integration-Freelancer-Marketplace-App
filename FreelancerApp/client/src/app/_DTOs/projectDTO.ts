export interface ProjectDTO {
  id: number;
  title: string;
  description?: string;
  clientUserId: number;
  photoUrl?: string;
  isAssigned: boolean;
  skills: string[];
  
}
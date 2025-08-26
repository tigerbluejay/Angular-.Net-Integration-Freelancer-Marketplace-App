export interface ProjectBrowseDTO {
  id: number;
  title: string;
  description: string;
  isAssigned: boolean;
  skillNames: string[];
  clientUserId: number;
  clientUserName: string;
  photoUrl?: string | null;
  clientPhotoUrl: string;
}
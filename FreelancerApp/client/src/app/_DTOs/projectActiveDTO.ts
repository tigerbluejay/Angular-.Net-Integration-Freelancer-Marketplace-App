export interface ProjectActiveDTO {
  id: number;
  title?: string;
  description: string;
  isAssigned: boolean;
  skillNames: string[];
  clientUserId: number;
  clientKnownAs?: string;
  clientPhotoUrl?: string;
  freelancerUserId?: number;
  freelancerKnownAs?: string;
  freelancerPhotoUrl?: string;
  photoUrl?: string;
}
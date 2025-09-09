export interface UserAdminDTO {
  id: number;
  firstName: string;
  lastName: string;
  dateOfBirth?: string; // DateOnly from backend comes as ISO string
  knownAs: string;
  gender?: string;
  country?: string;
  city?: string;
  isAvailable: boolean;
  created: string;
  lastActive: string;
  photoUrl?: string;
  skills: string[];
  roles: string[];
  isAccountDisabled: boolean;
}
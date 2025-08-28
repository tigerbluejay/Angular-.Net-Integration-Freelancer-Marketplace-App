export interface Proposal {
  id: number;
  title: string;
  description?: string;
  bid: number; // NEW FIELD
  projectId: number;
  freelancerUserId: number;
  clientUserId: number;
  freelancerUsername?: string;
  clientUsername?: string;
  projectTitle?: string;
  photoUrl?: string;
  isAccepted?: boolean;
}

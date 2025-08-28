export interface ProposalCreateDTO {
  title: string;
  description?: string;
  bid: number; // NEW FIELD
  projectId: number;
  freelancerUserId: number;
  clientUserId: number;
  photoFile?: File; // optional
}
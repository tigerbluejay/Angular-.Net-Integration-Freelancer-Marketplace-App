import { ProjectAssignDTO } from "./projectAssignDTO";
import { ProposalAssignDTO } from "./proposalAssignDTO";

export interface ProposalWithProjectAssignCombinedDTO {
  proposal: ProposalAssignDTO;
  project: ProjectAssignDTO;
}
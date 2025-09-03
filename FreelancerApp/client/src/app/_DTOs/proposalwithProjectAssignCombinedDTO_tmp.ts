import { ProjectAssignDTO } from "./projectAssignDTO";
import { ProposalAssignDTO } from "./proposalAssignDTO";

export interface ProposalwithProjectAssignCombinedDTO {
  proposal: ProposalAssignDTO;
  project: ProjectAssignDTO;
}
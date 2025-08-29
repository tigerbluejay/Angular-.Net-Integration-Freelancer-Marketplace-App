import { Proposal } from "../_models/proposal";
import { ProjectBrowseDTO } from "./projectBrowseDTO";

export interface ProposalWithProjectCombinedDTO {
  proposal: Proposal;
  project: ProjectBrowseDTO;
}
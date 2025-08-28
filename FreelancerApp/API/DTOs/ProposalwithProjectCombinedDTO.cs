namespace API.DTOs
{
    public class ProposalWithProjectCombinedDTO
    {
        public ProposalDTO Proposal { get; set; }
        public ProjectBrowseDTO Project { get; set; }
    }
}
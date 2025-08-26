export interface ProjectParams {
  pageNumber?: number;      // default 1
  pageSize?: number;        // default 6 (max 12 on server)
  skillNames?: string[];    // default []
  includeAssigned?: boolean; // default false
  matchAllSkills?: boolean;  // default false
}
import { PortfolioItem } from "./portfolio-item";
import { Project } from "./project";

export interface Member {
  id: number;
  userName: string;

  firstName: string;
  lastName: string;
  dateOfBirth?: string; // ISO format (e.g., '1990-01-01')

  gender?: string;
  country?: string;
  city?: string;

  bio?: string;
  lookingFor?: string;
  website?: string;
  linkedIn?: string;
  gitHub?: string;

  isAvailable: boolean;

  created: string;    // ISO datetime (e.g., '2025-07-22T13:45:00Z')
  lastActive: string;

  photoUrl?: string;

  skills: string[];
  roles: string[];

  portfolioItems: PortfolioItem[];
  clientProjects: Project[];
  freelancerProjects: Project[];
}
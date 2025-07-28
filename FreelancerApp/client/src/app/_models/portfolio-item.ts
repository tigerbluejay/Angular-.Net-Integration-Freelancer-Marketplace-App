export interface PortfolioItem {
  id: number;
  photoUrl: string;
  title?: string;
  description?: string;
  created: string; // ISO string (DateTime from .NET)
}
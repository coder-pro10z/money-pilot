/**
 * Dashboard summary returned from backend
 * Maps to /api/dashboard or summary endpoints
 */
export interface DashboardSummary {
  totalExpenses: number;
  totalBudget: number;
  remainingBudget: number;
  monthlyTrend: MonthlyTrend[];
  categoryBreakdown: CategoryBreakdown[];
}

export interface MonthlyTrend {
  month: string;
  amount: number;
}

export interface CategoryBreakdown {
  category: string;
  amount: number;
}
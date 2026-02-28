/**
 * Expense entity model
 */
export interface Expense {
  id: number;
  description: string;
  amount: number;
  date: string;
  categoryId: number;
  categoryName: string; // add this
}
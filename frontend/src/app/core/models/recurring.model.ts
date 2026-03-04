export interface RecurringTransaction {
  id: number;
  description: string;
  amount: number;
  categoryId: number;
  categoryName?: string;
  recurrenceType: number;
  startDate: string;
  endDate?: string;
  isActive: boolean;
}
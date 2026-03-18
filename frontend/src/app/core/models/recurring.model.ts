export interface RecurringTransaction {
  id: number;
  description: string;
  amount: number;
  categoryId: number;
  categoryName?: string;
  recurrenceType: string | number;
  interval?: number;
  dayOfWeek?: string;
  dayOfMonth?: number;
  startDate: string;
  endDate?: string;
  nextOccurrence?: string;
  isActive: boolean;
  generatedExpensesCount?: number;
}

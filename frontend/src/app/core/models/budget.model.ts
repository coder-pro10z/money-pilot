export interface Budget {
  id: number;
  categoryId: number;
  categoryName?: string;
  monthlyLimit: number;
  month: string;
}
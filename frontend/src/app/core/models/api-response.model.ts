/**
 * Standard API response wrapper from backend.
 * All backend responses are wrapped in this structure.
 */
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}
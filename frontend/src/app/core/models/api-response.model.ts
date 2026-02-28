/**
 * Standard API response wrapper from backend.
 * All backend responses are wrapped in this structure.
 */
/**
 * Generic API response wrapper
 * Matches backend ApiResponse<T>
 */
export interface ApiResponse<T> {
  success: boolean;
  message: string | null;
  data: T;
}
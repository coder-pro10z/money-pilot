# Phase 2 Frontend Integration Notes

This document summarizes the backend behavior the Angular app depends on today, plus the backend/API changes still needed to make Phase 2 cleaner and safer to extend.

## Scope

Phase 2 in this repo effectively covers:

- Auth flow
- Categories CRUD
- Expenses CRUD
- Budgets CRUD
- Recurring transactions CRUD
- Dashboard summary integration

The notes below are based on the current code in:

- `backend/src/MoneyPilot.API/Controllers`
- `backend/src/MoneyPilot.Application/DTOs`
- `frontend/src/app/core/services`
- `frontend/src/app/features`

## Current API Contract Used by the App

### Response wrapper pattern

Most read endpoints use:

```json
{
  "success": true,
  "message": "optional",
  "data": {}
}
```

The Angular `ApiService` assumes that shape and unwraps `response.data`.

### Endpoints currently consumed by frontend

#### Auth

- `POST /api/auth/login`
- `POST /api/auth/register`

Current behavior:

- `register` returns wrapped `ApiResponse<string>`
- `login` returns raw `{ token }`

Frontend implication:

- `AuthService.login()` bypasses `ApiService` and uses `HttpClient` directly because login does not follow the wrapper used by the rest of the app.

#### Expenses

- `GET /api/expense`
- `GET /api/expense/{id}`
- `POST /api/expense`
- `PUT /api/expense/{id}`
- `DELETE /api/expense/{id}`

Current behavior:

- `GET` list returns `ApiResponse<PagedResponse<ExpenseResponseDto>>`
- `GET` by id returns `ApiResponse<ExpenseResponseDto>`
- `POST` returns raw created DTO via `CreatedAtAction`
- `PUT` returns `204 No Content`
- `DELETE` returns `204 No Content`

#### Budgets

- `GET /api/budget`
- `GET /api/budget/{id}`
- `POST /api/budget`
- `PUT /api/budget/{id}`
- `DELETE /api/budget/{id}`

Current behavior:

- `GET` list returns `ApiResponse<PagedResponse<BudgetResponseDto>>`
- `GET` by id returns `ApiResponse<BudgetResponseDto>`
- `POST` returns raw created DTO via `CreatedAtAction`
- `PUT` returns `204 No Content`
- `DELETE` returns wrapped success message

#### Categories

- `GET /api/categories`
- `POST /api/categories`
- `PUT /api/categories/{id}`
- `DELETE /api/categories/{id}`

Current behavior:

- `GET` all returns `ApiResponse<IEnumerable<CategoryDto>>`
- `POST`, `PUT`, `DELETE` return wrapped responses
- No `GET /api/categories/{id}` exists

Frontend implication:

- `CategoryService` is split between `ApiService` and raw `HttpClient`
- `getById()` exists in Angular service, but the backend controller does not implement that route

#### Recurring Transactions

- `GET /api/RecurringTransactions`
- `GET /api/RecurringTransactions/{id}`
- `POST /api/RecurringTransactions`
- `PUT /api/RecurringTransactions/{id}`
- `DELETE /api/RecurringTransactions/{id}`
- `GET /api/RecurringTransactions/due`
- `POST /api/RecurringTransactions/process-due`

Current behavior:

- `GET` list returns `ApiResponse<PagedResponse<RecurringTransactionDto>>`
- `GET` by id returns raw `RecurringTransactionDto`
- `POST` returns raw created DTO
- `PUT` returns raw updated DTO
- `DELETE` returns `204 No Content`
- `GET due` returns raw list
- `POST process-due` returns raw anonymous object

Important gap:

- Controller currently has `[Authorize]` commented out, but still reads `User` claims

#### Dashboard

- `GET /api/dashboard/summary`

Current behavior:

- returns `ApiResponse<DashboardSummaryDto>`

Frontend note:

- `DashboardService.charts()` points to `dashboard/charts`, but that endpoint does not exist

## DTO Shape Confirmed in Backend

### `PagedResponse<T>`

Backend shape:

```json
{
  "items": [],
  "totalCount": 0,
  "page": 1,
  "pageSize": 20
}
```

Frontend mismatch:

- `frontend/src/app/core/models/paged-response.model.ts` defines `pageNumber`
- backend uses `page`

This works only because current components mainly read `items`.

### Expense DTO

The app currently depends on:

- `id`
- `description`
- `amount`
- `date`
- `categoryId`
- `categoryName`

### Budget DTO

The app currently depends on:

- `id`
- `monthlyLimit`
- `month`
- `categoryId`
- `categoryName`

### Recurring DTO

Backend returns more fields than the current Angular model captures:

- `interval`
- `dayOfWeek`
- `dayOfMonth`
- `nextOccurrence`
- `createdAt`
- `generatedExpensesCount`

Frontend currently uses only the basic subset.

## Backend Changes the App Still Needs

These are the highest-value changes for Phase 2 stability.

### 1. Standardize all controller responses

Target:

- All success responses should either consistently use `ApiResponse<T>` or the frontend should stop assuming that wrapper globally.

Recommended backend change:

- Wrap `auth/login`, recurring `getById`, recurring `create`, recurring `update`, recurring `due`, recurring `process-due`, and all create/update/delete responses that are still raw or mixed.

Why this matters:

- It removes special-case Angular code
- It lets all frontend services use `ApiService`
- It reduces silent runtime contract drift

### 2. Re-enable authorization on recurring endpoints

Current issue:

- `RecurringTransactionsController` has `[Authorize]` commented out
- `GetUserId()` still expects a JWT claim

Required backend change:

- Restore `[Authorize]` on the controller or specific actions

Impact on app:

- Prevents unauthenticated calls from reaching claim access logic and failing unpredictably

### 3. Add `GET /api/categories/{id}`

Current issue:

- Angular service exposes `getById()`
- backend controller does not support it

Required backend change:

- Add a category details endpoint, or remove the unused frontend method

Why it matters:

- Keeps category edit/detail flows predictable
- Avoids dead service methods and confusion during future refactors

### 4. Make paging contracts consistent

Current issue:

- backend returns `page`
- frontend model uses `pageNumber`

Required change:

- Prefer keeping backend as-is and updating Angular model to:

```ts
export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}
```

Optional backend improvement:

- Add page and pageSize query support to budgets and recurring endpoints, not just expenses

### 5. Normalize route naming

Current issue:

- app uses singular routes for `expense` and `budget`
- categories use plural
- recurring uses controller-derived `RecurringTransactions`

Required backend change:

- Pick one naming convention and keep it consistent, preferably lowercase plural REST routes:
  - `/api/expenses`
  - `/api/budgets`
  - `/api/categories`
  - `/api/recurring-transactions`

Impact:

- Cleaner frontend services
- fewer hard-to-spot route mismatches

### 6. Expose chart-specific dashboard endpoints only if needed

Current issue:

- frontend has `dashboard/charts` method
- backend only exposes `dashboard/summary`

Decision needed:

- Either remove the unused Angular method
- or add a dedicated chart endpoint if summary payload becomes too large

### 7. Add stronger validation feedback contracts

Current issue:

- some actions return raw `BadRequest`, some return wrapped failures, some return plain `NotFound`

Recommended backend change:

- Standardize validation and error responses so Angular can show reliable user-facing messages

Suggested minimum:

- `400` returns wrapper with message and validation details
- `404` returns wrapper with message
- `401/403` remain standard auth responses

## Frontend Changes That Should Happen Alongside Backend Cleanup

These are not backend tasks, but they are tightly coupled to the changes above.

- Refactor `AuthService` to use `ApiService` once login is wrapped
- Refactor `CategoryService` to stop mixing wrapped and raw `HttpClient` usage
- Update `PagedResponse` model to use `page`
- Extend `RecurringTransaction` model if the UI needs `nextOccurrence`, `interval`, or generated count
- Remove or implement `DashboardService.charts()`
- Replace native `confirm()` in budgets, categories, and recurring with the shared confirmation dialog already used by expenses

## Recommended Phase 2 Backend Priority Order

1. Re-enable recurring authorization.
2. Standardize response wrappers across all controllers.
3. Add missing category-by-id endpoint.
4. Normalize paging contract and route naming.
5. Standardize validation/error payloads.

## Practical Definition of Done for Phase 2

Phase 2 should be considered stable when:

- every frontend HTTP call can go through `ApiService`
- all protected controllers consistently require auth
- all list endpoints return the same paged shape when they are lists
- all CRUD modules expose predictable read/create/update/delete contracts
- the dashboard contract is explicit and no frontend service points to a nonexistent endpoint

## Notes

The current app is functional, but the main remaining risk is not missing features. It is API inconsistency. Cleaning that up will make the next phase faster than adding more one-off fixes in Angular.

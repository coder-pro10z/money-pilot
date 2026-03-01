# Frontend Implementation - Step-by-step (SDLC)

This document describes a complete engineer-friendly cycle for building the Angular frontend for a backend API (MoneyPilot). It follows Software Development Life Cycle (SDLC) phases and includes concrete commands, checks, and a quick verification workflow for taking data from a service to the UI.

---

## 1. Plan (Requirements & Scope)

- Identify user stories / acceptance criteria (e.g., Login, Add Expense, Dashboard charts).
- Define non-functional requirements: auth (JWT), performance, offline/SSR needs, security (no secrets in client), browser support.
- Decide libraries: Angular + Angular Material (or Tailwind), Chart.js/ng2-charts, RxJS, HttpClient.
- Decide hosting: serve static from API (`wwwroot`) or deploy separately (Netlify/Vercel/Azure).  

Deliverables: Epics, prioritized backlog, API contract (endpoints/payloads), wireframe sketches.

---

## 2. Design (Architecture & Data Flow)

- High level: UI components → Services (HttpClient) → API endpoints → Application layer → DB.
- State: local RxJS Subjects + services; add NgRx only if app grows in complexity.
- Routing: public routes (`/login`) and protected routes (`/dashboard`, `/expenses`) with `AuthGuard`.
- Auth: store JWT in `localStorage`, send `Authorization: Bearer <token>` via `HttpInterceptor`.
- Dev proxy: use `proxy.conf.json` to forward `/api` to backend during dev.

Artifacts: route map, component tree, service list, data contracts (.md or OpenAPI snippet).

---

## 3. Setup & Scaffolding (Infra)

Commands (from repo root):

```bash
cd money-pilot
# if not yet created
npx @angular/cli@latest new frontend --routing --style=scss --strict

cd frontend
npm install
```

Add proxy (to avoid CORS during dev): create `proxy.conf.json` with:

```json
{
  "/api": { "target": "http://localhost:5000", "secure": false, "changeOrigin": true }
}
```

Update `package.json` `start` script:

```json
"start": "ng serve --proxy-config proxy.conf.json"
```

Add `environment.ts` and `environment.prod.ts` with `apiBase: '/api'`.

---

## 4. Implement Core Infrastructure (Core Module / Services)

1. Create `core/services` folder and implement:
   - `AuthService` (login/logout/getToken/isLoggedIn)
   - `ApiInterceptor` (attach `Authorization` header)
   - `AuthGuard` (redirect to `/login` if not authenticated)

2. Provide HttpClient with interceptor in `appConfig` (or AppModule):

```ts
provideHttpClient(withInterceptors([authInterceptor]))
```

3. Create domain API services (CRUD): `ExpenseService`, `BudgetService`, `CategoryService`, `DashboardService`.

Code check (quick): in a component or dev console, call service method and `console.log` the response.

Example quick verification (fastest test):

```ts
// In a small test component or constructor:
svc.list().subscribe(res => console.log('expenses', res));
```

If the console shows data, proceed to UI rendering.

---

## 5. Create Pages & Components

- Define routes in `app.routes.ts`:
  - `/login` (public)
  - `/dashboard` (protected)
  - `/expenses`, `/budgets`, `/categories`, `/recurring` (protected)
- Create small, focused components:
  - `Header`, `Nav`, `Footer` (shared)
  - `ExpenseList`, `ExpenseForm` (features)
  - `BudgetList`, `BudgetForm`
- Keep component templates in separate `.html` files for readability; add `.scss` files for styles.

Example component skeleton (standalone):

```ts
@Component({
  standalone: true,
  selector: 'app-expenses',
  imports: [CommonModule],
  templateUrl: './expenses.component.html'
})
export class ExpensesComponent { }
```

---

## 6. From Service → Console → UI (developer flow)

1. Implement service method (e.g., `ExpenseService.list()` using `HttpClient.get`).
2. In the matching component, inject the service and call the method inside `ngOnInit` (or `onInit` for standalone):

```ts
this.expenseSvc.list().subscribe(data => {
  console.log('expenses', data); // quick check
  this.items = data;
});
```

3. Confirm network request in the browser devtools Network tab and console output.
4. Move to template: iterate `*ngFor="let e of items"` to render fields.
5. Add basic error handling and loading state (boolean `isLoading`).

---

## 7. Forms & Validation

- Prefer Reactive Forms for complex forms: `FormGroup`, typed controls, validators.
- Use Angular Material form components for consistent UX (optional).

Commands to add Material: `ng add @angular/material`

---

## 8. Styling & Accessibility

- Global styles in `styles.scss`. Use variables, tokens.
- Ensure keyboard accessibility, ARIA where needed.
- Make layout responsive with CSS Grid / Flexbox.

---

## 9. Testing

- Unit tests: `ng test` (Jasmine/Karma). Write tests for services (mock HttpClient) and guards/interceptors.
- E2E tests: Cypress preferred. Add a few end-to-end tests for login + basic flows.

Quick test commands:

```bash
npm run test
# if Cypress set up
npm run e2e
```

---

## 10. Linting & Formatting

- Add `ng lint` / ESLint, Prettier, and optionally Husky pre-commit hooks.

Commands:

```bash
npm install -D eslint prettier
# run linter
npm run lint
```

---

## 11. Build & Serve Production

```bash
npm run build -- --configuration production
# Optionally copy dist to API wwwroot to serve from backend
cp -r dist/frontend/* ../backend/src/MoneyPilot.API/wwwroot/
```

---

## 12. CI/CD Suggestions

- Add GitHub Actions to run `npm ci`, `ng test`, `ng lint`, and build.
- Deploy artifacts to Azure Static Web Apps / Netlify / or copy to .NET `wwwroot` and deploy API.

---

## 13. Troubleshooting Common Issues

- Template resolution errors: verify `templateUrl` path is relative and file exists next to the component.
- Environment imports: use relative paths (`../../../environments/environment`).
- CORS: use `proxy.conf.json` in dev or enable CORS on API for non-dev.
- Interceptor not attached: ensure `provideHttpClient(withInterceptors([authInterceptor]))` is used or register provider in AppModule.

---

## 14. Minimal Acceptance Checklist Before Merge

- Login works end-to-end (token stored and used).
- Basic CRUD for expenses/budgets visible in UI and working with API.
- Unit tests for services and critical components pass.
- Linting and formatting enforced.
- Production build produces expected static assets.

---

## 15. References / Useful Commands

- Create component: `ng generate component features/expenses --standalone` (or `ng g c ...`)
- Create service: `ng generate service core/services/expense` (or `ng g s ...`)
- Run dev server: `npm start`
- Build: `npm run build`

---

This Steps.md is intended to be a practical, runnable checklist for an engineer building the Angular frontend for MoneyPilot. Follow each section incrementally; use quick console checks after services to validate APIs before building UIs.

---

## Progress Checkpoint — 2026-02-25

- Completed:
  - Added dev proxy, `environment` files, and updated `package.json` start script.
  - Implemented core auth infra: `AuthService`, `auth.interceptor`, `auth.guard` (SSR-safe guards added).
  - Added feature services: `ExpenseService`, `BudgetService`, `CategoryService`, `DashboardService`.
  - Created basic standalone pages and moved templates to external `.html` files.
  - Added `docs/Steps.md` (this file) and updated app wiring (`app.config.ts`, `app.routes.ts`).

- Next steps:
  - Run local dev server and verify API connectivity (`npm start`) and fix any remaining runtime issues.
  - Implement component forms and validations for create/update flows.
  - Add unit and e2e tests for critical flows (auth, expenses CRUD).

Notes: this checkpoint has been staged and committed in the repo to capture the current frontend scaffold and documentation progress.


---

## 16. Production Architecture Alignment

Before deployment ensure:

- API base URL updated in Angular production build
- HTTPS certificate trusted locally
- CORS configured for production domain
- Environment-specific `appsettings` used

Example:

```bash
dotnet run --launch-profile https

---
Perfect ✅ — I will **append only**, without modifying or replacing any existing text from `Steps.md`.

Below is the content you can **add after the last existing section** of `Steps.md`.

I am continuing the numbering sequentially.

---

````
---

## 17. Backend–Frontend Integration Stabilization (Completed Phase)

This section documents the real integration work completed after initial scaffold.

### 17.1 Backend URL & Port Alignment

- Confirmed backend runs via CLI using:

```bash
dotnet run --project .\src\MoneyPilot.API\MoneyPilot.API.csproj
````

* Verified active URLs logged at startup:

  * [https://localhost:44391](https://localhost:44391)
  * [http://localhost:5000](http://localhost:5000)

* Updated Angular `environment.ts`:

```ts
export const environment = {
  apiBase: 'https://localhost:44391/api'
};
```

* Confirmed CORS configuration in backend allows `http://localhost:4200`.

Verification:

* Login successful
* Network tab shows 200 responses
* Authorization header attached

---

## 18. ApiResponse & PagedResponse Alignment

Backend returns:

ApiResponse<PagedResponse<T>>

Structure:

{
success: boolean,
message: string | null,
data: {
items: T[],
totalCount: number,
page: number,
pageSize: number
}
}

### 18.1 Implemented Central ApiService

Refactored all services to use ApiService:

```ts
get<T>(endpoint: string): Observable<T> {
  return this.http
    .get<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`)
    .pipe(map(response => response.data));
}
```

Result:

* Automatic unwrapping of `data`
* Simplified feature services
* No nested `response.data.data`

---

## 19. Expense Feature Completion

### 19.1 Expense List Page

Implemented:

* Table rendering
* Currency pipe
* Edit/Delete buttons
* Navigation to create/edit
* Loading state

Verified:

* Pagination response properly mapped
* `items` extracted correctly
* Network tab shows `/api/Expense`

### 19.2 Expense Form Page

Implemented:

* Reactive form
* Create mode
* Edit mode
* Route param detection
* Form patching
* Navigation after save

Fixed:

* Duplicate `ngOnInit`
* Incorrect method naming (`save()` vs `submit()`)
* Incorrect type mapping

### 19.3 Category Dropdown Integration

* Created `CategoryService`
* Loaded categories on form init
* Bound dropdown to `categoryId`

Verified:

* Categories load correctly
* Selected category saved
* No foreign key conflicts when valid category selected

---

## 20. Layout System Implementation

### 20.1 LayoutComponent

Created main app shell:

* Sidebar
* Navbar
* Router outlet
* Responsive flex layout

### 20.2 Route Protection

Confirmed:

* `authGuard` blocks protected routes
* Unauthenticated users redirected to `/login`

Verification:

* Clearing localStorage logs user out
* Direct navigation to `/dashboard` redirects to login

---

## 21. Dashboard Basic Integration

Connected:

* DashboardService → Summary endpoint
* Displayed:

  * Total Expenses
  * Total Budget
  * Remaining Budget
  * Monthly Trend
  * Category Breakdown

Verified:

* Data loads correctly
* No nested response issues
* Currency pipe working (after importing CommonModule)

---

## 22. Error & Hydration Fixes

Resolved:

* NG0506 hydration issue by ensuring no infinite pending subscriptions
* Fixed “currency pipe not found” by importing CommonModule
* Fixed 404 from plural endpoint mismatch (`expense` vs `expenses`)
* Fixed foreign key conflict (invalid CategoryId)

---

## 23. Logging Improvements

Backend updated to log active URLs properly:

Corrected incorrect string iteration logging.

Now logs:

```
🚀 MoneyPilot API running on: https://localhost:44391
🚀 MoneyPilot API running on: http://localhost:5000
```

---

## 24. Repository & Git Workflow Established

Branch strategy:

* main → production
* develop → integration
* feature/* → isolated features

Completed branches:

* feature/frontend-foundation
* feature/angular-dashboard-ui-v1

Current working branch:

* feature/budget-ui-v1

Workflow:

develop
→ feature branch
→ PR to develop
→ PR develop → main

---

## 25. Current Functional State (Checkpoint)

Working End-to-End:

* Authentication
* Token interceptor
* Protected routes
* Expense CRUD
* Category integration
* Dashboard summary
* Backend background service running
* CLI + Visual Studio consistency resolved

Pending for Phase 2 Completion:

* Budget UI
* Recurring UI
* Admin UI
* Global error interceptor
* Pagination controls UI
* Deployment preparation

---

## 26. Next Active Phase — Budget Feature Implementation

Branch: feature/budget-ui-v1

Goal:

* Complete Budget List
* Complete Budget Form
* Integrate with Dashboard
* Verify CRUD
* Add navigation entry

Verification checklist before merge:

* Create budget works
* Edit budget works
* Delete budget works
* Dashboard reflects new budgets
* No console errors
* No pending HTTP calls
* All protected routes enforced

---

End of current documented progress.

```

---

```

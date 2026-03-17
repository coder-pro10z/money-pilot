import { Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
// Public routes
    {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login.component')
        .then(m => m.LoginComponent)
     },
     {path: 'register',
      loadComponent: () =>
        import('./features/auth/register.component')
          .then(m => m.RegisterComponent)
     },
     // Protected routes
     {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
          // Lazy loaded feature modules
          {
            path: 'dashboard',
            loadComponent: () =>
              import('./features/dashboard/dashboard.component')
                .then(m => m.DashboardComponent)
          },
            //expense routes
            {
            path: 'expense',
            loadComponent: () =>
              import('./features/expenses/expenses.component')
                .then(m => m.ExpensesComponent)
          },
          {
            path: 'expense/create',
            loadComponent: () =>
              import('./features/expenses/expense-form.component')
                .then(m => m.ExpenseFormComponent)
          },
          {
            path: 'expense/edit/:id',
            loadComponent: () =>
              import('./features/expenses/expense-form.component')
                .then(m => m.ExpenseFormComponent)
          },
          //Budget routes
          {
            path: 'budget',
            loadComponent: () =>
              import('./features/budgets/budgets.component')
                .then(m => m.BudgetsComponent)
          },
          {
            path: 'budget/create',
            loadComponent: () =>
              import('./features/budgets/budget-form.component')
                .then(m => m.BudgetFormComponent)
          },
          {
            path: 'budget/edit/:id',
            loadComponent: () =>
              import('./features/budgets/budget-form.component')
                .then(m => m.BudgetFormComponent)
          },

          //recurring transactions
          {
            path: 'recurring',
            loadComponent: () =>
              import('./features/recurring/recurring.component')
                .then(m => m.RecurringComponent)
          },
          {
            path: 'recurring/create',
            loadComponent: () =>
              import('./features/recurring/recurring-form.component')
                .then(m => m.RecurringFormComponent)
          },
          {
            path: 'recurring/edit/:id',
            loadComponent: () =>
              import('./features/recurring/recurring-form.component')
                .then(m => m.RecurringFormComponent)
          },
            //category routes
          {
            path: 'category',
            loadComponent: () =>
              import('./features/categories/categories.component')
                .then(m => m.CategoriesComponent)
          },
          {
            path: 'category/create',
            loadComponent: () =>
              import('./features/categories/category-form.component')
                .then(m => m.CategoryFormComponent)
          },
          {
            path: 'category/edit/:id',
            loadComponent: () =>
              import('./features/categories/category-form.component')
                .then(m => m.CategoryFormComponent)
          },
    // Default route
          {
            path: '',
            redirectTo: 'dashboard',
            pathMatch: 'full'
          }
    ]
  },


];

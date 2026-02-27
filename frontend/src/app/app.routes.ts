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
     // Protected routes
     {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
// Lazy loaded feature modules
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./features/dashboard/dashboard.module')
            .then(m => m.DashboardModule)
      },

      {
        path: 'expenses',
        loadComponent: () =>
          import('./features/expenses/expenses.component')
            .then(m => m.ExpensesComponent)
      },

      {
        path: 'recurring',
        loadComponent: () =>
          import('./features/recurring/recurring.component')
            .then(m => m.RecurringComponent)
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
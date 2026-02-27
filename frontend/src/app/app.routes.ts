import { Routes } from '@angular/router';

import { LoginComponent } from './features/auth/login.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ExpensesComponent } from './features/expenses/expenses.component';
import { BudgetsComponent } from './features/budgets/budgets.component';
import { CategoriesComponent } from './features/categories/categories.component';
import { RecurringComponent } from './features/recurring/recurring.component';
import { authGuard } from './core/auth.guard';
import { LayoutComponent } from './layout/layout.component';

export const routes: Routes = [
	{ path: '', redirectTo: 'dashboard', pathMatch: 'full' },
	{ path: 'login', component: LoginComponent },
	// { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
	{ path: 'expenses', component: ExpensesComponent, canActivate: [authGuard] },
	{ path: 'budgets', component: BudgetsComponent, canActivate: [authGuard] },
	{ path: 'categories', component: CategoriesComponent, canActivate: [authGuard] },
	{ path: 'recurring', component: RecurringComponent, canActivate: [authGuard] },
	{ path: '**', redirectTo: 'dashboard' },
    {
    path: 'dashboard',
    loadChildren: () =>
        import('./features/dashboard/dashboard.module')
        .then(m => m.DashboardModule)
    },
    {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
        {
        path: 'dashboard',
        loadChildren: () =>
            import('./features/dashboard/dashboard.module')
            .then(m => m.DashboardModule)
        },
        {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
        }
    ]
    }
];

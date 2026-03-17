import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors, withFetch } from '@angular/common/http';

import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { importProvidersFrom } from '@angular/core'; // <-- Add this
import { MatDialogModule } from '@angular/material/dialog'; // <-- Add this
// import { importProvidersFrom } from '@angular/core';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSnackBarModule } from '@angular/material/snack-bar';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideClientHydration(),
    provideAnimationsAsync('noop'),
    // provideHttpClient(withFetch(), withInterceptors([authInterceptor]))
    provideHttpClient(withFetch(), withInterceptors([authInterceptor,errorInterceptor])),
    // Add MatDialogModule to providers
     importProvidersFrom(MatDialogModule), // <-- Makes MatDialog available everywhere
     importProvidersFrom(
      MatSidenavModule,
      MatToolbarModule,
      MatIconModule,
      MatListModule,
      MatSnackBarModule
    )
    ]
};

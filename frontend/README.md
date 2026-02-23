# MoneyPilot Frontend (Angular)

This folder holds the Angular frontend for MoneyPilot. Use Angular CLI to scaffold and run the app.

Quick start:

```bash
cd money-pilot/frontend
npx @angular/cli@latest 
npm install
npm start
```

Development notes:
- Use `proxy.conf.json` for local API proxying to the backend (avoid CORS).
- Implement `AuthService`, `HttpInterceptor` and `AuthGuard` to handle JWT-based auth.
- See `../docs/Steps` for a full implementation checklist and guidance.

If you want I can scaffold example `AuthService`, interceptor, and a sample page next.

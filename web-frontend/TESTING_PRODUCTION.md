# Testing Production Build Locally

## Why test the production build?

The development server (`npm run dev`) serves unbundled ES modules directly, while production serves minified and bundled code. Some issues like circular dependencies only appear in the production build.

## Option 1: Quick Preview (Recommended for quick tests)

Vite includes a built-in preview server:

```bash
# Build the production bundle
npm run build

# Preview the production build
npm run preview
```

The preview server will start at `http://localhost:4173` (or another port if 4173 is busy).

**Note:** This preview server is for local testing only and doesn't exactly replicate your production nginx configuration.

## Option 2: Docker Compose (Production-like environment)

For a more accurate simulation of the production environment using nginx:

### Prerequisites
- Docker and Docker Compose installed
- Production build created

### Steps

1. Build the production bundle:
```bash
npm run build
```

2. Start the Docker Compose environment:
```bash
docker compose up
```

3. Access the application at `http://localhost:8080`

4. Stop the environment:
```bash
docker compose down
```

### Configuration Notes

- The `compose.yaml` uses the same `nginx.conf` as production
- Static files are served from the `dist` directory
- API proxy is configured (adjust the `api` service configuration to match your backend)

## Troubleshooting Production Issues

### Common production-only issues:

1. **Circular dependency errors** (e.g., "Cannot access uninitialized variable")
   - Caused by module bundling breaking circular imports
   - Solution: Remove manual chunking or adjust `vite.config.ts`

2. **Module not found errors**
   - Check if dynamic imports are properly configured
   - Verify all dependencies are in `dependencies` (not `devDependencies`)

3. **CORS errors**
   - Check nginx proxy configuration
   - Verify API base URLs match your backend

4. **Environment variables not working**
   - Ensure variables are prefixed with `GIBZAPP_`
   - Check if `.env` files are properly configured

### Debugging Steps

1. Build and preview locally first:
```bash
npm run build
npm run preview
```

2. If issues persist, test with Docker Compose to match production nginx setup

3. Check browser console and network tab for specific errors

4. Compare working dev build with production build to identify differences

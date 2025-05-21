import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './index.css';
import App from './App.tsx';
import {
  createBrowserRouter,
  Navigate,
  RouterProvider,
} from 'react-router-dom';
import { Currencies } from './components/pages/Currencies.tsx';
import { CreditPrograms } from './components/pages/CreditPrograms.tsx';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthStorekeeper } from './components/pages/AuthStorekeeper.tsx';
import { Storekeepers } from './components/pages/Storekeepers.tsx';
import { Periods } from './components/pages/Periods.tsx';
import { Toaster } from './components/ui/sonner.tsx';
import { Profile } from './components/pages/Profile.tsx';
import { Reports } from './components/pages/Reports.tsx';

const routes = createBrowserRouter([
  {
    path: '/',
    element: <App />,
    children: [
      {
        path: '/currencies',
        element: <Currencies />,
      },
      {
        path: '/credit-programs',
        element: <CreditPrograms />,
      },
      {
        path: '/storekeepers',
        element: <Storekeepers />,
      },
      {
        path: '/periods',
        element: <Periods />,
      },
      {
        path: '/profile',
        element: <Profile />,
      },
      {
        path: '/reports',
        element: <Reports />,
      },
    ],
    errorElement: <p>бля пизда рулям</p>,
  },
  {
    path: '/auth',
    element: <AuthStorekeeper />,
  },
  {
    path: '*',
    element: <Navigate to="/" replace />,
  },
]);

const queryClient = new QueryClient();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={routes} />
      <Toaster />
    </QueryClientProvider>
  </StrictMode>,
);

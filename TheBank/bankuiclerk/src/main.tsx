import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './index.css';
import App from './App.tsx';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Toaster } from 'sonner';
import {
  createBrowserRouter,
  Navigate,
  RouterProvider,
} from 'react-router-dom';
import { Profile } from './components/pages/Profile.tsx';
import { AuthClerks } from './components/pages/AuthClerks.tsx';
import { Clerks } from './components/pages/Clerks.tsx';
import { Clients } from './components/pages/Clients.tsx';
import { Deposits } from './components/pages/Deposits.tsx';
import { Replenishments } from './components/pages/Replenishments.tsx';

const routes = createBrowserRouter([
  {
    path: '/',
    element: <App />,
    children: [
      {
        path: '/profile',
        element: <Profile />,
      },
      {
        path: '/clerks',
        element: <Clerks />,
      },
      {
        path: '/clients',
        element: <Clients />,
      },
      {
        path: '/deposits',
        element: <Deposits />,
      },
      {
        path: '/replenishments',
        element: <Replenishments />,
      },
    ],
    errorElement: <p>бля пизда рулям</p>,
  },
  {
    path: '/auth',
    element: <AuthClerks />,
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

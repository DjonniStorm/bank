import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './index.css';
import App from './App.tsx';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { Currencies } from './components/pages/Currencies.tsx';
import { CreditPrograms } from './components/pages/CreditPrograms.tsx';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

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
    ],
    errorElement: <p>бля пизда рулям</p>,
  },
]);

const queryClient = new QueryClient();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={routes} />
    </QueryClientProvider>
  </StrictMode>,
);

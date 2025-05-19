import { useAuthCheck } from '@/hooks/useAuthCheck';
import { useAuthStore } from '@/store/workerStore';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { Header } from '@/components/layout/Header';
import { Footer } from '@/components/layout/Footer';
import { Suspense } from 'react';

function App() {
  const user = useAuthStore((store) => store.user);
  const { isLoading } = useAuthCheck();
  const location = useLocation();

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (!user) {
    const redirect = encodeURIComponent(location.pathname + location.search);
    return <Navigate to={`/auth?redirect=${redirect}`} replace />;
  }

  return (
    <>
      <Header />
      <Suspense fallback={<p>Loading...</p>}>
        <Outlet />
      </Suspense>
      <Footer />
    </>
  );
}

export default App;

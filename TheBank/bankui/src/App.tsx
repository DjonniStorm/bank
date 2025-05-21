import { useAuthCheck } from '@/hooks/useAuthCheck';
import { useAuthStore } from '@/store/workerStore';
import { Link, Navigate, Outlet, useLocation } from 'react-router-dom';
import { Header } from '@/components/layout/Header';
import { Footer } from '@/components/layout/Footer';
import { Suspense } from 'react';
import { Button } from './components/ui/button';

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
      {location.pathname === '/' && (
        <main className="flex justify-center items-center">
          <div className="flex-1 flex justify-center items-center">
            <img className="block" src="/Shrek.png" alt="кладовщик" />
          </div>
          <div className="flex-1">
            <div>Удобный сервис для кладовщиков</div>
            <Link to="/storekeepers">
              <Button>За работу</Button>
            </Link>
          </div>
        </main>
      )}
      {location.pathname !== '/' && (
        <>
          <Header />
          <Suspense fallback={<p>Loading...</p>}>
            <Outlet />
          </Suspense>
        </>
      )}
      <Footer />
    </>
  );
}

export default App;

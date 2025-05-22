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
      <Header />
      <Suspense fallback={<p>Loading...</p>}>
        {location.pathname === '/' && (
          <main>
            <div>Удобный сервис для работы клерков</div>
            <div className="w-full h-full flex">
              <div className="">
                <img
                  className="max-w-[65%]"
                  src="/clerk.jpg"
                  alt="Клерк улыбается"
                />
              </div>
              <Link className="block my-auto" to="/clerks">
                <Button>Работать!</Button>
              </Link>
            </div>
          </main>
        )}
        {location.pathname !== '/' && <Outlet />}
      </Suspense>
      <Footer />
    </>
  );
}

export default App;

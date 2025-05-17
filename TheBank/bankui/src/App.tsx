import { Suspense } from 'react';
import { Footer } from './components/layout/Footer';
import { Header } from './components/layout/Header';
import { Outlet } from 'react-router-dom';

function App() {
  return (
    <>
      <Header />
      <Suspense fallback={<p>loading</p>}>
        <Outlet />
      </Suspense>
      <Footer />
    </>
  );
}

export default App;

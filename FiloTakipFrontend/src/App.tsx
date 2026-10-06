import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './context/AuthContext';
import { Layout } from './components/Layout';
import { Login } from './pages/Login';
import { Dashboard } from './pages/Dashboard';
import { Araclar } from './pages/Araclar';
import AracDetay from './pages/AracDetay';
import { Soforler } from './pages/Soforler';
import { Seferler } from './pages/Seferler';
import Yakit from './pages/Yakit';
import Bakim from './pages/Bakim';
import Belgeler from './pages/Belgeler';
import Raporlar from './pages/Raporlar';
import Ayarlar from './pages/Ayarlar';
import type { ReactNode } from 'react';

function PrivateRoute({ children }: { children: ReactNode }) {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? children : <Navigate to="/login" replace />;
}

function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route
        path="/"
        element={
          <PrivateRoute>
            <Layout />
          </PrivateRoute>
        }
      >
        <Route index element={<Dashboard />} />
        <Route path="araclar" element={<Araclar />} />
        <Route path="araclar/:id" element={<AracDetay />} />
        <Route path="soforler" element={<Soforler />} />
        <Route path="seferler" element={<Seferler />} />
        <Route path="yakit" element={<Yakit />} />
        <Route path="bakim" element={<Bakim />} />
        <Route path="belgeler" element={<Belgeler />} />
        <Route path="raporlar" element={<Raporlar />} />
        <Route path="ayarlar" element={<Ayarlar />} />
      </Route>
    </Routes>
  );
}

export default App;

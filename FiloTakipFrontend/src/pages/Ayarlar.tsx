import { useState, useEffect } from 'react';
import { Settings, Users, Building, Bell, Plus, Shield, MapPin, Phone, X } from 'lucide-react';
import api from '../lib/api';

const Ayarlar = () => {
  const [activeTab, setActiveTab] = useState<'kullanicilar' | 'subeler' | 'bildirimler'>('kullanicilar');
  
  // Note: These would typically come from specific endpoints in AyarlarController / KimlikController
  // For demonstration, we'll implement a basic structure that can be expanded
  const [kullanicilar, setKullanicilar] = useState<any[]>([]);

  const [isUserModalOpen, setIsUserModalOpen] = useState(false);
  const [newUser, setNewUser] = useState({ adSoyad: '', eposta: '', sifre: '', rol: 1, subeId: '' });

  useEffect(() => {
    if (activeTab === 'kullanicilar') {
      fetchKullanicilar();
    }
  }, [activeTab]);

  const fetchKullanicilar = async () => {
    try {
      const response = await api.get('/Kimlik/kullanicilar');
      setKullanicilar(response.data || []);
    } catch (error) {
      console.error('Kullanıcılar alınamadı:', error);
    }
  };

  const handleKullaniciEkle = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await api.post('/Kimlik/kullanici-olustur', {
        ...newUser,
        subeId: newUser.subeId ? parseInt(newUser.subeId) : null
      });
      setIsUserModalOpen(false);
      setNewUser({ adSoyad: '', eposta: '', sifre: '', rol: 1, subeId: '' });
      fetchKullanicilar();
    } catch (error) {
      console.error('Kullanıcı eklenemedi:', error);
      alert('Kullanıcı eklenirken bir hata oluştu.');
    }
  };

  const [subeler, setSubeler] = useState<any[]>([]);
  const [loadingSubeler, setLoadingSubeler] = useState(false);

  useEffect(() => {
    if (activeTab === 'subeler') {
      fetchSubeler();
    }
  }, [activeTab]);

  const fetchSubeler = async () => {
    setLoadingSubeler(true);
    try {
      const response = await api.get('/Subeler');
      setSubeler(response.data || []);
    } catch (error) {
      console.error('Şubeler alınamadı:', error);
    } finally {
      setLoadingSubeler(false);
    }
  };

  return (
    <div className="p-6 max-w-7xl mx-auto">
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-3">
            <Settings className="h-8 w-8 text-blue-600" />
            Sistem Ayarları
          </h1>
          <p className="text-gray-500 mt-1">Kullanıcı, rol, şube ve genel sistem konfigürasyonları</p>
        </div>
      </div>

      <div className="flex flex-col md:flex-row gap-8">
        {/* Sidebar Navigation */}
        <div className="w-full md:w-64 flex-shrink-0">
          <nav className="flex flex-col gap-2">
            <button
              onClick={() => setActiveTab('kullanicilar')}
              className={`flex items-center gap-3 px-4 py-3 rounded-xl font-medium transition-colors ${
                activeTab === 'kullanicilar' ? 'bg-blue-50 text-blue-700' : 'text-gray-600 hover:bg-gray-50'
              }`}
            >
              <Users className="w-5 h-5" /> Kullanıcı Yönetimi
            </button>
            <button
              onClick={() => setActiveTab('subeler')}
              className={`flex items-center gap-3 px-4 py-3 rounded-xl font-medium transition-colors ${
                activeTab === 'subeler' ? 'bg-blue-50 text-blue-700' : 'text-gray-600 hover:bg-gray-50'
              }`}
            >
              <Building className="w-5 h-5" /> Şube & Lokasyonlar
            </button>
            <button
              onClick={() => setActiveTab('bildirimler')}
              className={`flex items-center gap-3 px-4 py-3 rounded-xl font-medium transition-colors ${
                activeTab === 'bildirimler' ? 'bg-blue-50 text-blue-700' : 'text-gray-600 hover:bg-gray-50'
              }`}
            >
              <Bell className="w-5 h-5" /> Bildirim Şablonları
            </button>
          </nav>
        </div>

        {/* Content Area */}
        <div className="flex-1 bg-white rounded-2xl shadow-sm border border-gray-100 p-6 min-h-[500px]">
          {activeTab === 'kullanicilar' && (
            <div>
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-xl font-bold text-gray-900">Sistem Kullanıcıları</h2>
                <button 
                  onClick={() => setIsUserModalOpen(true)}
                  className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 flex items-center gap-2">
                  <Plus className="w-4 h-4" /> Yeni Kullanıcı
                </button>
              </div>

              {isUserModalOpen && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
                  <div className="bg-white rounded-xl shadow-lg w-full max-w-md p-6">
                    <div className="flex justify-between items-center mb-4">
                      <h3 className="text-lg font-bold text-gray-900">Yeni Kullanıcı Ekle</h3>
                      <button onClick={() => setIsUserModalOpen(false)} className="text-gray-400 hover:text-gray-600"><X className="w-5 h-5" /></button>
                    </div>
                    <form onSubmit={handleKullaniciEkle} className="space-y-4">
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Ad Soyad</label>
                        <input type="text" required value={newUser.adSoyad} onChange={e => setNewUser({...newUser, adSoyad: e.target.value})} className="w-full border border-gray-300 rounded-lg p-2" />
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">E-posta</label>
                        <input type="email" required value={newUser.eposta} onChange={e => setNewUser({...newUser, eposta: e.target.value})} className="w-full border border-gray-300 rounded-lg p-2" />
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Şifre</label>
                        <input type="password" required value={newUser.sifre} onChange={e => setNewUser({...newUser, sifre: e.target.value})} className="w-full border border-gray-300 rounded-lg p-2" />
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Rol</label>
                        <select value={newUser.rol} onChange={e => setNewUser({...newUser, rol: parseInt(e.target.value)})} className="w-full border border-gray-300 rounded-lg p-2">
                          <option value={1}>Admin</option>
                          <option value={2}>Yonetici</option>
                          <option value={3}>Kullanici</option>
                        </select>
                      </div>
                      <div className="pt-4 flex justify-end gap-2">
                        <button type="button" onClick={() => setIsUserModalOpen(false)} className="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50">İptal</button>
                        <button type="submit" className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">Kaydet</button>
                      </div>
                    </form>
                  </div>
                </div>
              )}
              <div className="overflow-x-auto">
                <table className="w-full text-left border-collapse">
                  <thead>
                    <tr className="bg-gray-50 border-b border-gray-100">
                      <th className="p-3 text-sm font-semibold text-gray-600">Ad Soyad</th>
                      <th className="p-3 text-sm font-semibold text-gray-600">E-posta</th>
                      <th className="p-3 text-sm font-semibold text-gray-600">Rol</th>
                      <th className="p-3 text-sm font-semibold text-gray-600">İşlem</th>
                    </tr>
                  </thead>
                  <tbody>
                    {kullanicilar.map((user) => (
                      <tr key={user.id} className="border-b border-gray-50 hover:bg-gray-50">
                        <td className="p-3 font-medium text-gray-900">{user.adSoyad}</td>
                        <td className="p-3 text-sm text-gray-600">{user.eposta}</td>
                        <td className="p-3 text-sm">
                          <span className={`inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-medium ${
                            user.rol === 1 || user.rol === 'Admin' ? 'bg-purple-50 text-purple-700 border border-purple-200' : 'bg-gray-50 text-gray-700 border border-gray-200'
                          }`}>
                            {(user.rol === 1 || user.rol === 'Admin') && <Shield className="w-3 h-3" />}
                            {user.rol === 1 ? 'Admin' : user.rol === 2 ? 'Yönetici' : user.rol === 3 ? 'Kullanıcı' : user.rol}
                          </span>
                        </td>
                        <td className="p-3 text-sm">
                          <button className="text-blue-600 hover:underline">Düzenle</button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}

          {activeTab === 'subeler' && (
            <div>
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-xl font-bold text-gray-900">Şube ve Lokasyonlar</h2>
                <button className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 flex items-center gap-2">
                  <Plus className="w-4 h-4" /> Yeni Şube Ekle
                </button>
              </div>
              
              {loadingSubeler ? (
                <div className="text-center py-10">Yükleniyor...</div>
              ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {subeler.length === 0 ? (
                    <div className="col-span-2 text-center py-10 text-gray-500">
                      Henüz şube bulunmuyor.
                    </div>
                  ) : (
                    subeler.map(sube => (
                      <div key={sube.id} className="border border-gray-200 rounded-xl p-5 hover:shadow-sm transition-shadow">
                        <div className="flex justify-between items-start mb-2">
                          <h3 className="font-bold text-gray-900 text-lg">{sube.ad}</h3>
                          <span className="bg-green-100 text-green-700 text-xs px-2 py-1 rounded font-medium">Aktif</span>
                        </div>
                        <div className="space-y-2 mt-4">
                          <div className="flex items-start gap-2 text-sm text-gray-600">
                            <MapPin className="w-4 h-4 mt-0.5 text-gray-400" />
                            <span>{sube.adres || '-'}, {sube.sehir || '-'}</span>
                          </div>
                          <div className="flex items-center gap-2 text-sm text-gray-600">
                            <Phone className="w-4 h-4 text-gray-400" />
                            <span>{sube.telefon || '-'}</span>
                          </div>
                        </div>
                        <div className="mt-4 pt-4 border-t border-gray-100 flex justify-end">
                          <button className="text-blue-600 hover:text-blue-800 text-sm font-medium">Düzenle</button>
                        </div>
                      </div>
                    ))
                  )}
                </div>
              )}
            </div>
          )}

          {activeTab === 'bildirimler' && (
            <div className="text-center py-20">
              <Bell className="h-16 w-16 text-gray-300 mx-auto mb-4" />
              <h3 className="text-lg font-medium text-gray-900 mb-1">Bildirim Şablonları</h3>
              <p className="text-gray-500">Muayene, sigorta ve bakım hatırlatıcıları için SMS/E-posta şablonları.</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Ayarlar;

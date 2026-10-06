import React, { useState, useEffect } from 'react';
import api from '../lib/api';
import { Wrench, Plus, AlertCircle, CheckCircle } from 'lucide-react';

interface Arac {
  id: number;
  plaka: string;
  marka: string;
  model: string;
}

interface BakimKaydi {
  id: number;
  aracId: number;
  islemTuru: string;
  aciklama: string;
  maliyet: number;
  bakimTarihi: string;
  tamamlandiMi: boolean;
  sonrakiBakimTarihi: string | null;
  sonrakiBakimKm: number | null;
}

const Bakim = () => {
  const [araclar, setAraclar] = useState<Arac[]>([]);
  const [seciliAracId, setSeciliAracId] = useState<number | ''>('');
  const [bakimKayitlari, setBakimKayitlari] = useState<BakimKaydi[]>([]);
  const [yaklasanBakimlar, setYaklasanBakimlar] = useState<BakimKaydi[]>([]);
  const [loading, setLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  
  const [formData, setFormData] = useState({
    aracId: 0,
    islemTuru: '',
    aciklama: '',
    maliyet: 0,
    bakimTarihi: new Date().toISOString().split('T')[0],
    sonrakiBakimTarihi: '',
    sonrakiBakimKm: 0
  });

  useEffect(() => {
    fetchAraclar();
    fetchYaklasanBakimlar();
  }, []);

  useEffect(() => {
    if (seciliAracId) {
      fetchBakimKayitlari(Number(seciliAracId));
    } else {
      setBakimKayitlari([]);
    }
  }, [seciliAracId]);

  const fetchAraclar = async () => {
    try {
      const response = await api.get('/Araclar?sayfa=1&boyut=1000');
      if (response.data && response.data.veriler) {
        setAraclar(response.data.veriler);
      }
    } catch (error) {
      console.error('Araçlar yüklenemedi:', error);
    }
  };

  const fetchYaklasanBakimlar = async () => {
    try {
      const response = await api.get('/Bakim/yaklasan-bakimlar');
      setYaklasanBakimlar(response.data || []);
    } catch (error) {
      console.error('Yaklaşan bakımlar alınamadı:', error);
    }
  };

  const fetchBakimKayitlari = async (aracId: number) => {
    try {
      setLoading(true);
      const response = await api.get(`/Bakim/arac/${aracId}`);
      setBakimKayitlari(response.data || []);
    } catch (error) {
      console.error('Bakım kayıtları alınamadı:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const payload = {
        ...formData,
        aracId: seciliAracId || formData.aracId,
        sonrakiBakimTarihi: formData.sonrakiBakimTarihi ? formData.sonrakiBakimTarihi : null,
        sonrakiBakimKm: formData.sonrakiBakimKm > 0 ? formData.sonrakiBakimKm : null
      };
      await api.post('/Bakim', payload);
      setIsModalOpen(false);
      setFormData({
        aracId: 0, islemTuru: '', aciklama: '', maliyet: 0,
        bakimTarihi: new Date().toISOString().split('T')[0],
        sonrakiBakimTarihi: '', sonrakiBakimKm: 0
      });
      if (seciliAracId) {
        fetchBakimKayitlari(Number(seciliAracId));
      }
      fetchYaklasanBakimlar();
    } catch (error) {
      alert('Bakım kaydı eklenemedi.');
    }
  };

  const handleTamamla = async (id: number) => {
    if (window.confirm('Bu bakımı tamamlandı olarak işaretlemek istiyor musunuz?')) {
      try {
        await api.patch(`/Bakim/${id}/tamamla`, `"${new Date().toISOString()}"`, {
          headers: { 'Content-Type': 'application/json' }
        });
        if (seciliAracId) {
          fetchBakimKayitlari(Number(seciliAracId));
        }
        fetchYaklasanBakimlar();
      } catch (error) {
        alert('Bakım güncellenemedi.');
      }
    }
  };

  return (
    <div className="p-6 max-w-7xl mx-auto">
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-3">
            <Wrench className="h-8 w-8 text-blue-600" />
            Bakım & Arıza Yönetimi
          </h1>
          <p className="text-gray-500 mt-1">Araç servis, onarım ve periyodik bakım takibi</p>
        </div>
        <button
          onClick={() => setIsModalOpen(true)}
          className="bg-blue-600 text-white px-5 py-2.5 rounded-xl hover:bg-blue-700 transition-colors flex items-center gap-2 font-medium shadow-sm hover:shadow-md"
        >
          <Plus className="h-5 w-5" />
          Yeni Bakım / Arıza Kaydı
        </button>
      </div>

      {yaklasanBakimlar.length > 0 && (
        <div className="mb-8 p-4 bg-orange-50 border border-orange-200 rounded-2xl">
          <h2 className="text-lg font-bold text-orange-800 flex items-center gap-2 mb-3">
            <AlertCircle className="h-5 w-5" />
            Yaklaşan veya Geciken Bakımlar ({yaklasanBakimlar.length})
          </h2>
          <div className="flex gap-4 overflow-x-auto pb-2">
            {yaklasanBakimlar.map(bakim => {
              const arac = araclar.find(a => a.id === bakim.aracId);
              return (
                <div key={bakim.id} className="min-w-[250px] bg-white p-4 rounded-xl shadow-sm border border-orange-100 flex-shrink-0">
                  <div className="font-bold text-gray-900">{arac?.plaka || 'Bilinmeyen Araç'}</div>
                  <div className="text-sm text-gray-600 mt-1 line-clamp-1">{bakim.islemTuru}</div>
                  <div className="text-sm font-medium text-orange-600 mt-2">
                    Son Tarih: {bakim.sonrakiBakimTarihi ? new Date(bakim.sonrakiBakimTarihi).toLocaleDateString('tr-TR') : '-'}
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      )}

      <div className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100 mb-8">
        <label className="block text-sm font-medium text-gray-700 mb-2">Görüntülenecek Araç Seçin</label>
        <select
          value={seciliAracId}
          onChange={(e) => setSeciliAracId(e.target.value === '' ? '' : Number(e.target.value))}
          className="w-full max-w-md p-3 rounded-xl border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 transition-all outline-none bg-gray-50 hover:bg-white"
        >
          <option value="">-- Tüm Araçlar (Seçim Yapın) --</option>
          {araclar.map((arac) => (
            <option key={arac.id} value={arac.id}>
              {arac.plaka} - {arac.marka} {arac.model}
            </option>
          ))}
        </select>
      </div>

      {!seciliAracId ? (
        <div className="text-center py-20 bg-gray-50 rounded-2xl border-2 border-dashed border-gray-200">
          <Wrench className="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <p className="text-gray-500 text-lg">Servis geçmişini görmek için yukarıdan bir araç seçin.</p>
        </div>
      ) : loading ? (
        <div className="text-center py-10">Yükleniyor...</div>
      ) : (
        <div className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="bg-gray-50 border-b border-gray-100">
                  <th className="p-4 text-sm font-semibold text-gray-600">Durum</th>
                  <th className="p-4 text-sm font-semibold text-gray-600">Bakım Tarihi</th>
                  <th className="p-4 text-sm font-semibold text-gray-600">İşlem Türü</th>
                  <th className="p-4 text-sm font-semibold text-gray-600">Açıklama</th>
                  <th className="p-4 text-sm font-semibold text-gray-600">Maliyet</th>
                  <th className="p-4 text-sm font-semibold text-gray-600">Sonraki Bakım</th>
                  <th className="p-4 text-sm font-semibold text-gray-600">İşlem</th>
                </tr>
              </thead>
              <tbody>
                {bakimKayitlari.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="p-8 text-center text-gray-500">
                      Bu araca ait bakım kaydı bulunamadı.
                    </td>
                  </tr>
                ) : (
                  bakimKayitlari.map((kayit) => (
                    <tr key={kayit.id} className="border-b border-gray-50 hover:bg-gray-50 transition-colors">
                      <td className="p-4">
                        {kayit.tamamlandiMi ? (
                          <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium bg-green-50 text-green-700 border border-green-200">
                            <CheckCircle className="w-3.5 h-3.5" /> Tamamlandı
                          </span>
                        ) : (
                          <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium bg-orange-50 text-orange-700 border border-orange-200">
                            <AlertCircle className="w-3.5 h-3.5" /> Bekliyor
                          </span>
                        )}
                      </td>
                      <td className="p-4 text-sm text-gray-900 font-medium">
                        {new Date(kayit.bakimTarihi).toLocaleDateString('tr-TR')}
                      </td>
                      <td className="p-4 text-sm text-gray-900">{kayit.islemTuru}</td>
                      <td className="p-4 text-sm text-gray-500 max-w-xs truncate" title={kayit.aciklama}>{kayit.aciklama}</td>
                      <td className="p-4 text-sm font-bold text-gray-900">{(kayit.maliyet || 0).toLocaleString('tr-TR')} ₺</td>
                      <td className="p-4 text-sm text-gray-600">
                        {kayit.sonrakiBakimTarihi ? new Date(kayit.sonrakiBakimTarihi).toLocaleDateString('tr-TR') : '-'}
                        {kayit.sonrakiBakimKm ? ` veya ${kayit.sonrakiBakimKm.toLocaleString('tr-TR')} km` : ''}
                      </td>
                      <td className="p-4 text-sm">
                        {!kayit.tamamlandiMi && (
                          <button
                            onClick={() => handleTamamla(kayit.id)}
                            className="text-blue-600 hover:text-blue-800 font-medium text-xs border border-blue-200 px-2.5 py-1 rounded-lg hover:bg-blue-50 transition-colors"
                          >
                            Tamamla
                          </button>
                        )}
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {isModalOpen && (
        <div className="fixed inset-0 bg-black/40 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-2xl w-full max-w-md shadow-xl overflow-hidden">
            <div className="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50/50">
              <h2 className="text-xl font-bold text-gray-900">Yeni Bakım Kaydı</h2>
              <button onClick={() => setIsModalOpen(false)} className="text-gray-400 hover:text-gray-600">✕</button>
            </div>
            
            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Araç Seçin</label>
                <select
                  required
                  value={formData.aracId || seciliAracId}
                  onChange={(e) => setFormData({ ...formData, aracId: Number(e.target.value) })}
                  className="w-full p-2.5 rounded-xl border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
                >
                  <option value="">Araç Seçin...</option>
                  {araclar.map(a => (
                    <option key={a.id} value={a.id}>{a.plaka}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">İşlem Türü (Örn: Periyodik Bakım, Yağ Değişimi)</label>
                <input
                  type="text"
                  required
                  value={formData.islemTuru}
                  onChange={(e) => setFormData({ ...formData, islemTuru: e.target.value })}
                  className="w-full p-2.5 rounded-xl border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Açıklama / Değişen Parçalar</label>
                <textarea
                  value={formData.aciklama}
                  onChange={(e) => setFormData({ ...formData, aciklama: e.target.value })}
                  className="w-full p-2.5 rounded-xl border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
                  rows={2}
                ></textarea>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Bakım Tarihi</label>
                  <input
                    type="date"
                    required
                    value={formData.bakimTarihi}
                    onChange={(e) => setFormData({ ...formData, bakimTarihi: e.target.value })}
                    className="w-full p-2.5 rounded-xl border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Maliyet (₺)</label>
                  <input
                    type="number"
                    step="0.01"
                    required
                    value={formData.maliyet}
                    onChange={(e) => setFormData({ ...formData, maliyet: Number(e.target.value) })}
                    className="w-full p-2.5 rounded-xl border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
                  />
                </div>
              </div>

              <div className="border-t border-gray-100 pt-4 mt-2">
                <h3 className="text-sm font-semibold text-gray-900 mb-3">Sonraki Bakım Planlaması (Opsiyonel)</h3>
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Sonraki Tarih</label>
                    <input
                      type="date"
                      value={formData.sonrakiBakimTarihi}
                      onChange={(e) => setFormData({ ...formData, sonrakiBakimTarihi: e.target.value })}
                      className="w-full p-2.5 rounded-xl border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none text-sm"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-gray-600 mb-1">Sonraki Km</label>
                    <input
                      type="number"
                      value={formData.sonrakiBakimKm}
                      onChange={(e) => setFormData({ ...formData, sonrakiBakimKm: Number(e.target.value) })}
                      className="w-full p-2.5 rounded-xl border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none text-sm"
                    />
                  </div>
                </div>
              </div>

              <div className="pt-4 flex gap-3">
                <button
                  type="button"
                  onClick={() => setIsModalOpen(false)}
                  className="flex-1 px-4 py-2.5 rounded-xl border border-gray-200 text-gray-700 font-medium hover:bg-gray-50 transition-colors"
                >
                  İptal
                </button>
                <button
                  type="submit"
                  className="flex-1 px-4 py-2.5 rounded-xl bg-blue-600 text-white font-medium hover:bg-blue-700 transition-colors shadow-sm"
                >
                  Kaydet
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default Bakim;

import React, { useState, useEffect } from 'react';
import api from '../lib/api';
import { Fuel, Plus } from 'lucide-react';

interface Arac {
  id: number;
  plaka: string;
  marka: string;
  model: string;
}

interface YakitKaydi {
  id: number;
  aracId: number;
  alimTarihi: string;
  litre: number;
  birimFiyat: number;
  toplamTutar: number;
  istasyonAdi: string;
  faturaNo: string;
  kilometre: number;
}

const Yakit = () => {
  const [araclar, setAraclar] = useState<Arac[]>([]);
  const [seciliAracId, setSeciliAracId] = useState<number | ''>('');
  const [yakitKayitlari, setYakitKayitlari] = useState<YakitKaydi[]>([]);
  const [loading, setLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  
  const [formData, setFormData] = useState({
    aracId: 0,
    alimTarihi: new Date().toISOString().split('T')[0],
    litre: 0,
    birimFiyat: 0,
    istasyonAdi: '',
    faturaNo: '',
    kilometre: 0
  });

  useEffect(() => {
    fetchAraclar();
  }, []);

  useEffect(() => {
    if (seciliAracId) {
      fetchYakitKayitlari(Number(seciliAracId));
    } else {
      setYakitKayitlari([]);
    }
  }, [seciliAracId]);

  const fetchAraclar = async () => {
    try {
      const response = await api.get('/Araclar?sayfa=1&boyut=1000');
      if (response.data && response.data.veriler) {
        setAraclar(response.data.veriler);
      }
    } catch (error) {
      console.error('Araçlar yüklenirken hata:', error);
    }
  };

  const fetchYakitKayitlari = async (aracId: number) => {
    try {
      setLoading(true);
      const response = await api.get(`/Yakit/arac/${aracId}`);
      setYakitKayitlari(response.data || []);
    } catch (error) {
      console.error('Yakıt kayıtları alınamadı:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await api.post('/Yakit', { ...formData, aracId: seciliAracId || formData.aracId });
      setIsModalOpen(false);
      if (seciliAracId) {
        fetchYakitKayitlari(Number(seciliAracId));
      }
    } catch (error) {
      alert('Yakıt kaydı eklenemedi.');
    }
  };

  return (
    <div className="p-6 max-w-7xl mx-auto">
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-3">
            <Fuel className="h-8 w-8 text-blue-600" />
            Yakıt Takibi
          </h1>
          <p className="text-gray-500 mt-1">Araç bazlı yakıt tüketimi ve maliyet takibi</p>
        </div>
        <button
          onClick={() => setIsModalOpen(true)}
          className="bg-blue-600 text-white px-5 py-2.5 rounded-xl hover:bg-blue-700 transition-colors flex items-center gap-2 font-medium shadow-sm hover:shadow-md"
        >
          <Plus className="h-5 w-5" />
          Yeni Yakıt Fişi
        </button>
      </div>

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
          <Fuel className="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <p className="text-gray-500 text-lg">Yakıt geçmişini görmek için yukarıdan bir araç seçin.</p>
        </div>
      ) : loading ? (
        <div className="text-center py-10">Yükleniyor...</div>
      ) : (
        <div className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="bg-gray-50 border-b border-gray-100">
                  <th className="p-4 text-sm font-semibold text-gray-600">Tarih</th>
                  <th className="p-4 text-sm font-semibold text-gray-600">İstasyon</th>
                  <th className="p-4 text-sm font-semibold text-gray-600">Litre</th>
                  <th className="p-4 text-sm font-semibold text-gray-600">Birim Fiyat</th>
                  <th className="p-4 text-sm font-semibold text-gray-600">Toplam Tutar</th>
                  <th className="p-4 text-sm font-semibold text-gray-600">Fatura No</th>
                  <th className="p-4 text-sm font-semibold text-gray-600">Araç Km</th>
                </tr>
              </thead>
              <tbody>
                {yakitKayitlari.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="p-8 text-center text-gray-500">
                      Bu araca ait yakıt kaydı bulunamadı.
                    </td>
                  </tr>
                ) : (
                  yakitKayitlari.map((kayit) => (
                    <tr key={kayit.id} className="border-b border-gray-50 hover:bg-gray-50 transition-colors">
                      <td className="p-4 text-sm text-gray-900 font-medium">
                        {new Date(kayit.alimTarihi).toLocaleDateString('tr-TR')}
                      </td>
                      <td className="p-4 text-sm text-gray-600">{kayit.istasyonAdi}</td>
                      <td className="p-4 text-sm text-gray-900">{kayit.litre} L</td>
                      <td className="p-4 text-sm text-gray-600">{kayit.birimFiyat} ₺</td>
                      <td className="p-4 text-sm font-bold text-gray-900">{kayit.toplamTutar} ₺</td>
                      <td className="p-4 text-sm text-gray-500">{kayit.faturaNo}</td>
                      <td className="p-4 text-sm text-gray-600">{kayit.kilometre.toLocaleString('tr-TR')} km</td>
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
              <h2 className="text-xl font-bold text-gray-900">Yeni Yakıt Fişi</h2>
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

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Tarih</label>
                  <input
                    type="date"
                    required
                    value={formData.alimTarihi}
                    onChange={(e) => setFormData({ ...formData, alimTarihi: e.target.value })}
                    className="w-full p-2.5 rounded-xl border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Araç Km</label>
                  <input
                    type="number"
                    required
                    value={formData.kilometre}
                    onChange={(e) => setFormData({ ...formData, kilometre: Number(e.target.value) })}
                    className="w-full p-2.5 rounded-xl border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Litre</label>
                  <input
                    type="number"
                    step="0.01"
                    required
                    value={formData.litre}
                    onChange={(e) => setFormData({ ...formData, litre: Number(e.target.value) })}
                    className="w-full p-2.5 rounded-xl border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Birim Fiyat (₺)</label>
                  <input
                    type="number"
                    step="0.01"
                    required
                    value={formData.birimFiyat}
                    onChange={(e) => setFormData({ ...formData, birimFiyat: Number(e.target.value) })}
                    className="w-full p-2.5 rounded-xl border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">İstasyon</label>
                <input
                  type="text"
                  required
                  value={formData.istasyonAdi}
                  onChange={(e) => setFormData({ ...formData, istasyonAdi: e.target.value })}
                  className="w-full p-2.5 rounded-xl border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
                  placeholder="Örn: Shell"
                />
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

export default Yakit;

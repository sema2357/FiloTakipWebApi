import { useState, useEffect } from 'react';
import api from '../lib/api';
import { Plus, Search, Edit2, Trash2, Car, AlertCircle, X, Loader2, ChevronRight } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

interface Arac {
  id: number;
  plaka: string;
  marka: string;
  model: string;
  yil: number;
  sasiNo: string;
  yakitTipi: number;
  aracDurumu: number;
  guncelKm: number;
  subeAd?: string;
}

export function Araclar() {
  const navigate = useNavigate();
  const [araclar, setAraclar] = useState<Arac[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formData, setFormData] = useState({
    plaka: '',
    marka: '',
    model: '',
    yil: new Date().getFullYear(),
    sasiNo: '',
    yakitTipi: 0,
    subeId: 1 // Default to 1 for now
  });

  const fetchAraclar = async () => {
    try {
      setLoading(true);
      // Backend expects pagination: sayfa, boyut
      const response = await api.get('/Araclar?sayfa=1&boyut=100');
      if (response.data && response.data.veriler) {
        setAraclar(response.data.veriler);
      }
    } catch (error) {
      console.error('Araçlar yüklenirken hata oluştu:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAraclar();
  }, []);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    try {
      await api.post('/Araclar', formData);
      setIsModalOpen(false);
      setFormData({ plaka: '', marka: '', model: '', yil: new Date().getFullYear(), sasiNo: '', yakitTipi: 0, subeId: 1 });
      fetchAraclar();
    } catch (error) {
      console.error('Araç oluşturulurken hata:', error);
      alert('Araç eklenemedi. Lütfen bilgileri kontrol edin.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Bu aracı silmek istediğinize emin misiniz?')) {
      try {
        await api.delete(`/Araclar/${id}`);
        fetchAraclar();
      } catch (error) {
        console.error('Silme hatası:', error);
        alert('Araç silinemedi.');
      }
    }
  };

  const filteredAraclar = araclar.filter(a => 
    a.plaka.toLowerCase().includes(searchTerm.toLowerCase()) || 
    a.marka.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const getDurumBadge = (durum: number) => {
    switch (durum) {
      case 0: return <span className="px-2.5 py-1 rounded-full text-xs font-medium bg-emerald-500/10 text-emerald-400 border border-emerald-500/20">Aktif</span>;
      case 1: return <span className="px-2.5 py-1 rounded-full text-xs font-medium bg-amber-500/10 text-amber-400 border border-amber-500/20">Bakımda</span>;
      case 2: return <span className="px-2.5 py-1 rounded-full text-xs font-medium bg-blue-500/10 text-blue-400 border border-blue-500/20">Seferde</span>;
      case 3: return <span className="px-2.5 py-1 rounded-full text-xs font-medium bg-rose-500/10 text-rose-400 border border-rose-500/20">Pasif</span>;
      default: return <span className="px-2.5 py-1 rounded-full text-xs font-medium bg-slate-500/10 text-slate-400 border border-slate-500/20">Bilinmiyor</span>;
    }
  };

  return (
    <div className="space-y-6">
      {/* Header & Actions */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-white flex items-center gap-2">
            <Car className="w-6 h-6 text-cyan-400" />
            Araç Yönetimi
          </h1>
          <p className="text-sm text-slate-400 mt-1">Filonuzdaki tüm araçları görüntüleyin ve yönetin.</p>
        </div>
        
        <div className="flex items-center gap-3 w-full sm:w-auto">
          <div className="relative group flex-1 sm:flex-none">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-500 group-focus-within:text-cyan-400 transition-colors" />
            <input 
              type="text" 
              placeholder="Plaka veya Marka Ara..." 
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full sm:w-64 bg-slate-900/50 border border-slate-700 rounded-lg py-2 pl-9 pr-4 text-sm text-white placeholder:text-slate-500 focus:outline-none focus:border-cyan-500/50 focus:ring-1 focus:ring-cyan-500/50 transition-all"
            />
          </div>
          <button 
            onClick={() => setIsModalOpen(true)}
            className="flex items-center gap-2 bg-cyan-500 hover:bg-cyan-400 text-slate-950 font-semibold py-2 px-4 rounded-lg transition-colors"
          >
            <Plus className="w-4 h-4" />
            <span className="hidden sm:inline">Yeni Araç</span>
          </button>
        </div>
      </div>

      {/* Table Content */}
      <div className="bg-slate-900/50 backdrop-blur-xl border border-white/5 rounded-2xl overflow-hidden shadow-xl shadow-black/20">
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-slate-800/50 border-b border-white/5">
                <th className="py-4 px-6 text-xs font-semibold text-slate-400 uppercase tracking-wider">Plaka / Araç</th>
                <th className="py-4 px-6 text-xs font-semibold text-slate-400 uppercase tracking-wider">Model / Yıl</th>
                <th className="py-4 px-6 text-xs font-semibold text-slate-400 uppercase tracking-wider">Durum</th>
                <th className="py-4 px-6 text-xs font-semibold text-slate-400 uppercase tracking-wider">Güncel KM</th>
                <th className="py-4 px-6 text-xs font-semibold text-slate-400 uppercase tracking-wider text-right">İşlemler</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-white/5">
              {loading ? (
                <tr>
                  <td colSpan={5} className="py-12 text-center">
                    <Loader2 className="w-8 h-8 animate-spin text-cyan-400 mx-auto mb-4" />
                    <p className="text-slate-400">Araçlar yükleniyor...</p>
                  </td>
                </tr>
              ) : filteredAraclar.length === 0 ? (
                <tr>
                  <td colSpan={5} className="py-12 text-center">
                    <AlertCircle className="w-8 h-8 text-slate-500 mx-auto mb-4" />
                    <p className="text-slate-400">Kayıtlı araç bulunamadı.</p>
                  </td>
                </tr>
              ) : (
                filteredAraclar.map((arac) => (
                  <tr key={arac.id} className="hover:bg-white/[0.02] transition-colors group">
                    <td className="py-4 px-6">
                      <div className="flex flex-col">
                        <span className="text-white font-bold tracking-wide">{arac.plaka}</span>
                        <span className="text-sm text-slate-400">{arac.marka}</span>
                      </div>
                    </td>
                    <td className="py-4 px-6">
                      <div className="flex flex-col">
                        <span className="text-slate-200">{arac.model}</span>
                        <span className="text-xs text-slate-500">{arac.yil}</span>
                      </div>
                    </td>
                    <td className="py-4 px-6">
                      {getDurumBadge(arac.aracDurumu)}
                    </td>
                    <td className="py-4 px-6">
                      <span className="text-slate-300 font-medium">
                        {(arac.guncelKm || 0).toLocaleString('tr-TR')} <span className="text-xs text-slate-500">km</span>
                      </span>
                    </td>
                    <td className="py-4 px-6">
                      <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                        <button 
                          onClick={() => navigate(`/araclar/${arac.id}`)}
                          className="p-2 rounded-lg text-slate-400 hover:text-blue-400 hover:bg-blue-400/10 transition-colors"
                          title="Detay Görüntüle"
                        >
                          <ChevronRight className="w-4 h-4" />
                        </button>
                        <button className="p-2 rounded-lg text-slate-400 hover:text-cyan-400 hover:bg-cyan-400/10 transition-colors">
                          <Edit2 className="w-4 h-4" />
                        </button>
                        <button 
                          onClick={() => handleDelete(arac.id)}
                          className="p-2 rounded-lg text-slate-400 hover:text-rose-400 hover:bg-rose-400/10 transition-colors"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Create Modal */}
      {isModalOpen && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
          <div className="bg-slate-900 border border-slate-700 rounded-2xl w-full max-w-lg shadow-2xl overflow-hidden animate-in fade-in zoom-in-95 duration-200">
            <div className="flex justify-between items-center p-6 border-b border-white/5">
              <h2 className="text-xl font-bold text-white flex items-center gap-2">
                <Car className="w-5 h-5 text-cyan-400" />
                Yeni Araç Ekle
              </h2>
              <button 
                onClick={() => setIsModalOpen(false)}
                className="text-slate-400 hover:text-white transition-colors p-1"
              >
                <X className="w-5 h-5" />
              </button>
            </div>
            
            <form onSubmit={handleCreate} className="p-6 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-1">
                  <label className="text-xs font-medium text-slate-400 uppercase">Plaka</label>
                  <input 
                    type="text" required
                    value={formData.plaka} onChange={e => setFormData({...formData, plaka: e.target.value})}
                    className="w-full bg-slate-950 border border-slate-800 rounded-lg py-2 px-3 text-white focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 outline-none uppercase"
                    placeholder="34 ABC 123"
                  />
                </div>
                <div className="space-y-1">
                  <label className="text-xs font-medium text-slate-400 uppercase">Marka</label>
                  <input 
                    type="text" required
                    value={formData.marka} onChange={e => setFormData({...formData, marka: e.target.value})}
                    className="w-full bg-slate-950 border border-slate-800 rounded-lg py-2 px-3 text-white focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 outline-none"
                    placeholder="Ford"
                  />
                </div>
                <div className="space-y-1">
                  <label className="text-xs font-medium text-slate-400 uppercase">Model</label>
                  <input 
                    type="text" required
                    value={formData.model} onChange={e => setFormData({...formData, model: e.target.value})}
                    className="w-full bg-slate-950 border border-slate-800 rounded-lg py-2 px-3 text-white focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 outline-none"
                    placeholder="Transit"
                  />
                </div>
                <div className="space-y-1">
                  <label className="text-xs font-medium text-slate-400 uppercase">Yıl</label>
                  <input 
                    type="number" required min="1990" max={new Date().getFullYear() + 1}
                    value={formData.yil} onChange={e => setFormData({...formData, yil: parseInt(e.target.value)})}
                    className="w-full bg-slate-950 border border-slate-800 rounded-lg py-2 px-3 text-white focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 outline-none"
                  />
                </div>
                <div className="space-y-1 col-span-2">
                  <label className="text-xs font-medium text-slate-400 uppercase">Şasi Numarası</label>
                  <input 
                    type="text" required
                    value={formData.sasiNo} onChange={e => setFormData({...formData, sasiNo: e.target.value})}
                    className="w-full bg-slate-950 border border-slate-800 rounded-lg py-2 px-3 text-white focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 outline-none uppercase"
                  />
                </div>
              </div>
              
              <div className="pt-4 flex justify-end gap-3">
                <button 
                  type="button" 
                  onClick={() => setIsModalOpen(false)}
                  className="px-4 py-2 text-sm font-medium text-slate-300 hover:text-white transition-colors"
                >
                  İptal
                </button>
                <button 
                  type="submit"
                  disabled={isSubmitting}
                  className="px-4 py-2 text-sm font-medium bg-cyan-500 hover:bg-cyan-400 text-slate-950 rounded-lg transition-colors flex items-center gap-2 disabled:opacity-50"
                >
                  {isSubmitting ? <Loader2 className="w-4 h-4 animate-spin" /> : <Plus className="w-4 h-4" />}
                  Kaydet
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

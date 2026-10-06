import { useState, useEffect } from 'react';
import api from '../lib/api';
import { Plus, Search, CheckCircle, XCircle, Route, MapPin, Calendar, Loader2, Users } from 'lucide-react';
import { format } from 'date-fns';
import { tr } from 'date-fns/locale';

interface Sefer {
  id: number;
  aracPlaka: string;
  soforAdSoyad: string;
  baslangicNoktasi: string;
  varisNoktasi: string;
  planlananBaslangic: string;
  planlananBitis: string;
  durumu: number;
  onaylandiMi: boolean;
  planlananKm: number;
}

interface Arac { id: number; plaka: string; }
interface Sofor { id: number; ad: string; soyad: string; }

export function Seferler() {
  const [seferler, setSeferler] = useState<Sefer[]>([]);
  const [araclar, setAraclar] = useState<Arac[]>([]);
  const [soforler, setSoforler] = useState<Sofor[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [formData, setFormData] = useState({
    aracId: 0,
    soforId: 0,
    baslangicNoktasi: '',
    varisNoktasi: '',
    planlananBaslangic: '',
    planlananBitis: '',
    planlananKm: 0,
    irsaliyeNo: ''
  });

  const fetchData = async () => {
    try {
      setLoading(true);
      const [seferRes, aracRes, soforRes] = await Promise.all([
        api.get('/Seferler'),
        api.get('/Araclar?sayfa=1&boyut=1000'), // Tüm araçları al
        api.get('/Soforler')
      ]);
      setSeferler(seferRes.data || []);
      setAraclar(aracRes.data?.veriler || []);
      setSoforler(soforRes.data || []);
    } catch (error) {
      console.error('Veriler yüklenirken hata oluştu:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    try {
      await api.post('/Seferler', formData);
      setIsModalOpen(false);
      setFormData({
        aracId: 0, soforId: 0, baslangicNoktasi: '', varisNoktasi: '',
        planlananBaslangic: '', planlananBitis: '', planlananKm: 0, irsaliyeNo: ''
      });
      fetchData();
    } catch (error: any) {
      alert(error.response?.data?.message || 'Sefer oluşturulamadı. Araç/Şoför müsaitliğini kontrol edin.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleOnayla = async (id: number) => {
    if (window.confirm('Bu seferi onaylamak istediğinize emin misiniz?')) {
      try {
        await api.post('/Seferler/onay', { seferId: id, onayDurumu: true, onaylayanId: 1 });
        fetchData();
      } catch (error) {
        alert('Sefer onaylanamadı.');
      }
    }
  };

  const getDurumBadge = (durum: number) => {
    switch (durum) {
      case 0: return <span className="px-2.5 py-1 rounded-full text-xs font-medium bg-slate-500/10 text-slate-400 border border-slate-500/20">Planlandı</span>;
      case 1: return <span className="px-2.5 py-1 rounded-full text-xs font-medium bg-blue-500/10 text-blue-400 border border-blue-500/20">Devam Ediyor</span>;
      case 2: return <span className="px-2.5 py-1 rounded-full text-xs font-medium bg-emerald-500/10 text-emerald-400 border border-emerald-500/20">Tamamlandı</span>;
      case 3: return <span className="px-2.5 py-1 rounded-full text-xs font-medium bg-rose-500/10 text-rose-400 border border-rose-500/20">İptal Edildi</span>;
      default: return <span className="px-2.5 py-1 rounded-full text-xs font-medium bg-slate-500/10 text-slate-400 border border-slate-500/20">Bilinmiyor</span>;
    }
  };

  const formatDate = (dateStr: string) => {
    if (!dateStr) return '-';
    return format(new Date(dateStr), "d MMM yyyy HH:mm", { locale: tr });
  };

  const filtered = seferler.filter(s => 
    s.aracPlaka?.toLowerCase().includes(searchTerm.toLowerCase()) || 
    s.soforAdSoyad?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    s.varisNoktasi?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-white flex items-center gap-2">
            <Route className="w-6 h-6 text-purple-400" />
            Sefer Yönetimi
          </h1>
          <p className="text-sm text-slate-400 mt-1">Rota planlaması yapın, güncel seferleri ve varışları takip edin.</p>
        </div>
        
        <div className="flex items-center gap-3 w-full sm:w-auto">
          <div className="relative group flex-1 sm:flex-none">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-500 group-focus-within:text-purple-400 transition-colors" />
            <input 
              type="text" 
              placeholder="Plaka, Şoför veya Rota..." 
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full sm:w-64 bg-slate-900/50 border border-slate-700 rounded-lg py-2 pl-9 pr-4 text-sm text-white placeholder:text-slate-500 focus:outline-none focus:border-purple-500/50 focus:ring-1 focus:ring-purple-500/50 transition-all"
            />
          </div>
          <button 
            onClick={() => setIsModalOpen(true)}
            className="flex items-center gap-2 bg-purple-500 hover:bg-purple-400 text-white font-semibold py-2 px-4 rounded-lg transition-colors"
          >
            <Plus className="w-4 h-4" />
            <span className="hidden sm:inline">Yeni Sefer</span>
          </button>
        </div>
      </div>

      {/* Cards List */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {loading ? (
          <div className="col-span-full py-12 flex justify-center">
             <Loader2 className="w-8 h-8 animate-spin text-purple-400" />
          </div>
        ) : filtered.length === 0 ? (
          <div className="col-span-full py-12 flex flex-col items-center justify-center text-slate-400 border border-white/5 bg-slate-900/30 rounded-2xl">
            <Route className="w-8 h-8 mb-4 opacity-50" />
            <p>Sefer kaydı bulunamadı.</p>
          </div>
        ) : (
          filtered.map(sefer => (
            <div key={sefer.id} className="bg-slate-900/80 backdrop-blur-md border border-white/5 rounded-2xl p-6 hover:border-purple-500/30 transition-colors group relative overflow-hidden">
              <div className="absolute top-0 right-0 p-4 opacity-0 group-hover:opacity-100 transition-opacity flex gap-2">
                {!sefer.onaylandiMi && (
                  <button onClick={() => handleOnayla(sefer.id)} className="p-2 bg-emerald-500/20 text-emerald-400 rounded-lg hover:bg-emerald-500/30 transition-colors" title="Seferi Onayla">
                    <CheckCircle className="w-4 h-4" />
                  </button>
                )}
              </div>

              <div className="flex justify-between items-start mb-6">
                <div>
                  <div className="flex items-center gap-3 mb-1">
                    <span className="text-lg font-bold text-white bg-slate-800 px-3 py-1 rounded-lg border border-slate-700 font-mono">
                      {sefer.aracPlaka}
                    </span>
                    {getDurumBadge(sefer.durumu)}
                    {sefer.onaylandiMi ? (
                      <span className="text-xs text-emerald-400 flex items-center gap-1"><CheckCircle className="w-3 h-3"/> Onaylı</span>
                    ) : (
                      <span className="text-xs text-amber-400 flex items-center gap-1"><XCircle className="w-3 h-3"/> Onay Bekliyor</span>
                    )}
                  </div>
                  <p className="text-slate-400 text-sm mt-2 flex items-center gap-2">
                    <Users className="w-4 h-4 text-slate-500" /> {sefer.soforAdSoyad}
                  </p>
                </div>
              </div>

              <div className="relative">
                <div className="absolute left-[11px] top-4 bottom-4 w-[2px] bg-slate-800 rounded-full" />
                
                <div className="flex items-start gap-4 mb-4 relative z-10">
                  <div className="w-6 h-6 rounded-full bg-slate-900 border-2 border-purple-500 flex items-center justify-center shrink-0 mt-0.5">
                    <div className="w-2 h-2 rounded-full bg-purple-500" />
                  </div>
                  <div>
                    <p className="text-white font-medium">{sefer.baslangicNoktasi}</p>
                    <p className="text-xs text-slate-500 flex items-center gap-1 mt-1">
                      <Calendar className="w-3 h-3" /> {formatDate(sefer.planlananBaslangic)}
                    </p>
                  </div>
                </div>

                <div className="flex items-start gap-4 relative z-10">
                  <div className="w-6 h-6 rounded-full bg-slate-900 border-2 border-cyan-500 flex items-center justify-center shrink-0 mt-0.5">
                    <MapPin className="w-3 h-3 text-cyan-500" />
                  </div>
                  <div>
                    <p className="text-white font-medium">{sefer.varisNoktasi}</p>
                    <p className="text-xs text-slate-500 flex items-center gap-1 mt-1">
                      <Calendar className="w-3 h-3" /> {formatDate(sefer.planlananBitis)}
                    </p>
                  </div>
                </div>
              </div>

              <div className="mt-6 pt-4 border-t border-white/5 flex justify-between items-center text-sm">
                <span className="text-slate-500">Mesafe</span>
                <span className="text-slate-300 font-medium">{sefer.planlananKm} km</span>
              </div>
            </div>
          ))
        )}
      </div>

      {/* Modal - Sefer Ekle */}
      {isModalOpen && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
          <div className="bg-slate-900 border border-slate-700 rounded-2xl w-full max-w-2xl shadow-2xl overflow-hidden">
            <div className="flex justify-between items-center p-6 border-b border-white/5">
              <h2 className="text-xl font-bold text-white flex items-center gap-2">
                <Route className="w-5 h-5 text-purple-400" /> Yeni Sefer Planla
              </h2>
              <button onClick={() => setIsModalOpen(false)} className="text-slate-400 hover:text-white"><XCircle className="w-5 h-5" /></button>
            </div>
            <form onSubmit={handleCreate} className="p-6 space-y-4">
               <div className="grid grid-cols-2 gap-4">
                 <div>
                    <label className="text-xs text-slate-400 mb-1 block">Araç Seçimi</label>
                    <select 
                      required value={formData.aracId} 
                      onChange={e => setFormData({...formData, aracId: parseInt(e.target.value)})}
                      className="w-full bg-slate-950 border border-slate-800 rounded-lg p-2.5 text-white outline-none focus:border-purple-500"
                    >
                      <option value={0} disabled>Araç Seçiniz</option>
                      {araclar.map(a => <option key={a.id} value={a.id}>{a.plaka}</option>)}
                    </select>
                 </div>
                 <div>
                    <label className="text-xs text-slate-400 mb-1 block">Şoför Seçimi</label>
                    <select 
                      required value={formData.soforId} 
                      onChange={e => setFormData({...formData, soforId: parseInt(e.target.value)})}
                      className="w-full bg-slate-950 border border-slate-800 rounded-lg p-2.5 text-white outline-none focus:border-purple-500"
                    >
                      <option value={0} disabled>Şoför Seçiniz</option>
                      {soforler.map(s => <option key={s.id} value={s.id}>{s.ad} {s.soyad}</option>)}
                    </select>
                 </div>
                 <div>
                    <label className="text-xs text-slate-400 mb-1 block">Başlangıç Noktası</label>
                    <input type="text" required value={formData.baslangicNoktasi} onChange={e => setFormData({...formData, baslangicNoktasi: e.target.value})} className="w-full bg-slate-950 border border-slate-800 rounded-lg p-2.5 text-white outline-none focus:border-purple-500" />
                 </div>
                 <div>
                    <label className="text-xs text-slate-400 mb-1 block">Varış Noktası</label>
                    <input type="text" required value={formData.varisNoktasi} onChange={e => setFormData({...formData, varisNoktasi: e.target.value})} className="w-full bg-slate-950 border border-slate-800 rounded-lg p-2.5 text-white outline-none focus:border-purple-500" />
                 </div>
                 <div>
                    <label className="text-xs text-slate-400 mb-1 block">Çıkış Zamanı</label>
                    <input type="datetime-local" required value={formData.planlananBaslangic} onChange={e => setFormData({...formData, planlananBaslangic: e.target.value})} className="w-full bg-slate-950 border border-slate-800 rounded-lg p-2.5 text-white outline-none focus:border-purple-500" />
                 </div>
                 <div>
                    <label className="text-xs text-slate-400 mb-1 block">Tahmini Varış</label>
                    <input type="datetime-local" required value={formData.planlananBitis} onChange={e => setFormData({...formData, planlananBitis: e.target.value})} className="w-full bg-slate-950 border border-slate-800 rounded-lg p-2.5 text-white outline-none focus:border-purple-500" />
                 </div>
                 <div>
                    <label className="text-xs text-slate-400 mb-1 block">Planlanan KM</label>
                    <input type="number" min="1" required value={formData.planlananKm} onChange={e => setFormData({...formData, planlananKm: parseInt(e.target.value)})} className="w-full bg-slate-950 border border-slate-800 rounded-lg p-2.5 text-white outline-none focus:border-purple-500" />
                 </div>
                 <div>
                    <label className="text-xs text-slate-400 mb-1 block">İrsaliye No</label>
                    <input type="text" value={formData.irsaliyeNo} onChange={e => setFormData({...formData, irsaliyeNo: e.target.value})} className="w-full bg-slate-950 border border-slate-800 rounded-lg p-2.5 text-white outline-none focus:border-purple-500" />
                 </div>
               </div>
               
               <div className="pt-6 flex justify-end gap-3">
                 <button type="button" onClick={() => setIsModalOpen(false)} className="px-5 py-2.5 text-slate-300">İptal</button>
                 <button type="submit" disabled={isSubmitting} className="px-5 py-2.5 bg-purple-500 hover:bg-purple-400 text-white font-medium rounded-lg transition-colors flex items-center gap-2">
                   {isSubmitting ? <Loader2 className="w-4 h-4 animate-spin" /> : 'Planla'}
                 </button>
               </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

import { useState, useEffect } from 'react';
import api from '../lib/api';
import { Plus, Search, Edit2, Trash2, Users, AlertCircle, X, Loader2, Star } from 'lucide-react';

interface Sofor {
  id: number;
  ad: string;
  soyad: string;
  tcNo: string;
  ehliyetNo: string;
  telefon: string;
  kanGrubu: string;
  isBaslangicTarihi: string;
  aktifMi: boolean;
  performansPuani: number;
}

export function Soforler() {
  const [soforler, setSoforler] = useState<Sofor[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  
  const [formData, setFormData] = useState({
    ad: '', soyad: '', tcNo: '', ehliyetNo: '', telefon: '', kanGrubu: '', subeId: 1
  });

  const fetchSoforler = async () => {
    try {
      setLoading(true);
      const response = await api.get('/Soforler');
      if (response.data) {
        setSoforler(response.data);
      }
    } catch (error) {
      console.error('Şoförler yüklenirken hata:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSoforler();
  }, []);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    try {
      await api.post('/Soforler', formData);
      setIsModalOpen(false);
      setFormData({ ad: '', soyad: '', tcNo: '', ehliyetNo: '', telefon: '', kanGrubu: '', subeId: 1 });
      fetchSoforler();
    } catch (error) {
      alert('Şoför eklenemedi.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Bu şoförü silmek istediğinize emin misiniz?')) {
      try {
        await api.delete(`/Soforler/${id}`);
        fetchSoforler();
      } catch (error) {
        alert('Şoför silinemedi.');
      }
    }
  };

  const filtered = soforler.filter(s => 
    (s.ad + ' ' + s.soyad).toLowerCase().includes(searchTerm.toLowerCase()) || 
    (s.tcNo && s.tcNo.includes(searchTerm))
  );

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-white flex items-center gap-2">
            <Users className="w-6 h-6 text-emerald-400" />
            Şoför Yönetimi
          </h1>
          <p className="text-sm text-slate-400 mt-1">Filonuzdaki şoförleri ve performanslarını takip edin.</p>
        </div>
        
        <div className="flex items-center gap-3 w-full sm:w-auto">
          <div className="relative group flex-1 sm:flex-none">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-500 group-focus-within:text-emerald-400 transition-colors" />
            <input 
              type="text" 
              placeholder="İsim veya TC Ara..." 
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full sm:w-64 bg-slate-900/50 border border-slate-700 rounded-lg py-2 pl-9 pr-4 text-sm text-white placeholder:text-slate-500 focus:outline-none focus:border-emerald-500/50 focus:ring-1 focus:ring-emerald-500/50 transition-all"
            />
          </div>
          <button 
            onClick={() => setIsModalOpen(true)}
            className="flex items-center gap-2 bg-emerald-500 hover:bg-emerald-400 text-slate-950 font-semibold py-2 px-4 rounded-lg transition-colors"
          >
            <Plus className="w-4 h-4" />
            <span className="hidden sm:inline">Yeni Şoför</span>
          </button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {loading ? (
          <div className="col-span-full py-12 flex justify-center">
             <Loader2 className="w-8 h-8 animate-spin text-emerald-400" />
          </div>
        ) : filtered.length === 0 ? (
          <div className="col-span-full py-12 flex flex-col items-center justify-center text-slate-400 border border-white/5 bg-slate-900/30 rounded-2xl">
            <AlertCircle className="w-8 h-8 mb-4 opacity-50" />
            <p>Kayıtlı şoför bulunamadı.</p>
          </div>
        ) : (
          filtered.map(sofor => (
            <div key={sofor.id} className="relative group rounded-2xl p-[1px] overflow-hidden">
               <div className="absolute inset-0 bg-gradient-to-br from-emerald-500/30 to-teal-400/30 opacity-0 group-hover:opacity-100 transition-opacity duration-500 blur-xl" />
               <div className="relative bg-slate-900/80 backdrop-blur-md rounded-2xl border border-white/5 p-6 h-full flex flex-col">
                  <div className="flex justify-between items-start mb-4">
                    <div className="w-12 h-12 rounded-full bg-emerald-500/20 flex items-center justify-center text-emerald-400 font-bold text-lg border border-emerald-500/30">
                      {sofor.ad.charAt(0)}{sofor.soyad.charAt(0)}
                    </div>
                    <div className="flex gap-1">
                      <button className="text-slate-500 hover:text-emerald-400 transition-colors p-1"><Edit2 className="w-4 h-4" /></button>
                      <button onClick={() => handleDelete(sofor.id)} className="text-slate-500 hover:text-rose-400 transition-colors p-1"><Trash2 className="w-4 h-4" /></button>
                    </div>
                  </div>
                  
                  <h3 className="text-lg font-bold text-white truncate">{sofor.ad} {sofor.soyad}</h3>
                  <p className="text-sm text-slate-400 font-mono mb-4">{sofor.tcNo || 'TC Belirtilmedi'}</p>
                  
                  <div className="mt-auto space-y-2">
                    <div className="flex justify-between text-sm">
                      <span className="text-slate-500">Telefon</span>
                      <span className="text-slate-300">{sofor.telefon || '-'}</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-slate-500">Ehliyet</span>
                      <span className="text-slate-300 font-mono">{sofor.ehliyetNo || '-'}</span>
                    </div>
                    <div className="flex justify-between text-sm items-center pt-2 border-t border-white/5">
                      <span className="text-slate-500">Performans</span>
                      <span className="text-amber-400 flex items-center gap-1 font-bold">
                        {sofor.performansPuani} <Star className="w-3.5 h-3.5 fill-amber-400" />
                      </span>
                    </div>
                  </div>
               </div>
            </div>
          ))
        )}
      </div>

      {/* Modal - Şoför Ekle */}
      {isModalOpen && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
          <div className="bg-slate-900 border border-slate-700 rounded-2xl w-full max-w-lg shadow-2xl overflow-hidden">
            <div className="flex justify-between items-center p-6 border-b border-white/5">
              <h2 className="text-xl font-bold text-white flex items-center gap-2">
                <Users className="w-5 h-5 text-emerald-400" /> Yeni Şoför Ekle
              </h2>
              <button onClick={() => setIsModalOpen(false)} className="text-slate-400 hover:text-white"><X className="w-5 h-5" /></button>
            </div>
            <form onSubmit={handleCreate} className="p-6 space-y-4">
               {/* Sadece Ad Soyad ve TC şimdilik zorunlu örnek form */}
               <div className="grid grid-cols-2 gap-4">
                 <div>
                    <label className="text-xs text-slate-400">Ad</label>
                    <input type="text" required value={formData.ad} onChange={e => setFormData({...formData, ad: e.target.value})} className="w-full bg-slate-950 border border-slate-800 rounded-lg p-2 text-white outline-none focus:border-emerald-500" />
                 </div>
                 <div>
                    <label className="text-xs text-slate-400">Soyad</label>
                    <input type="text" required value={formData.soyad} onChange={e => setFormData({...formData, soyad: e.target.value})} className="w-full bg-slate-950 border border-slate-800 rounded-lg p-2 text-white outline-none focus:border-emerald-500" />
                 </div>
                 <div>
                    <label className="text-xs text-slate-400">TC Kimlik</label>
                    <input type="text" value={formData.tcNo} onChange={e => setFormData({...formData, tcNo: e.target.value})} className="w-full bg-slate-950 border border-slate-800 rounded-lg p-2 text-white outline-none focus:border-emerald-500" />
                 </div>
                 <div>
                    <label className="text-xs text-slate-400">Telefon</label>
                    <input type="text" value={formData.telefon} onChange={e => setFormData({...formData, telefon: e.target.value})} className="w-full bg-slate-950 border border-slate-800 rounded-lg p-2 text-white outline-none focus:border-emerald-500" />
                 </div>
               </div>
               <div className="pt-4 flex justify-end gap-3">
                 <button type="button" onClick={() => setIsModalOpen(false)} className="px-4 py-2 text-slate-300">İptal</button>
                 <button type="submit" disabled={isSubmitting} className="px-4 py-2 bg-emerald-500 text-slate-950 font-medium rounded-lg">Kaydet</button>
               </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

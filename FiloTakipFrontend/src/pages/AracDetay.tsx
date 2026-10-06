import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../lib/api';
import { 
  Info, Wrench, Fuel, FileText, Camera, ArrowLeft, 
  MapPin, Hash, AlertCircle, Edit2
} from 'lucide-react';

export default function AracDetay() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  
  const [activeTab, setActiveTab] = useState<'genel' | 'km_durum' | 'bakim' | 'yakit' | 'belgeler' | 'galeri'>('genel');
  const [arac, setArac] = useState<any>(null);
  const [bakimlar, setBakimlar] = useState<any[]>([]);
  const [yakitlar, setYakitlar] = useState<any[]>([]);
  const [belgeler, setBelgeler] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  const [yeniKm, setYeniKm] = useState('');
  const [kmUpdating, setKmUpdating] = useState(false);
  const [aracDurumu, setAracDurumu] = useState(0);
  const [statusUpdating, setStatusUpdating] = useState(false);

  // Upload states
  const [photoUploading, setPhotoUploading] = useState(false);
  const [showBelgeForm, setShowBelgeForm] = useState(false);
  const [belgeUploading, setBelgeUploading] = useState(false);
  const [yeniBelge, setYeniBelge] = useState({ adi: '', tipi: 0, tarih: '' });
  const [belgeDosya, setBelgeDosya] = useState<File | null>(null);

  useEffect(() => {
    fetchData();
  }, [id]);

  const fetchData = async () => {
    setLoading(true);
    try {
      // Araç ana bilgisini al (bu başarısız olursa sayfa çalışmaz)
      const aracRes = await api.get(`/Araclar/${id}`);
      setArac(aracRes.data);
      setAracDurumu(aracRes.data.durumu);

      // Yan bilgileri paralel olarak al ama hata olursa yoksay
      Promise.allSettled([
        api.get(`/Bakim/arac/${id}`),
        api.get(`/Yakit/arac/${id}`),
        api.get(`/Belgeler/arac/${id}`)
      ]).then((results) => {
        const [bakimRes, yakitRes, belgeRes] = results;
        
        if (bakimRes.status === 'fulfilled') setBakimlar(bakimRes.value.data || []);
        else console.error('Bakım verisi alınamadı', bakimRes.reason);
        
        if (yakitRes.status === 'fulfilled') setYakitlar(yakitRes.value.data || []);
        else console.error('Yakıt verisi alınamadı', yakitRes.reason);
        
        if (belgeRes.status === 'fulfilled') setBelgeler(belgeRes.value.data || []);
        else console.error('Belge verisi alınamadı', belgeRes.reason);
      });

    } catch (error) {
      console.error('Araç bilgileri alınamadı:', error);
      setArac(null);
    } finally {
      setLoading(false);
    }
  };

  const handleKmUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    setKmUpdating(true);
    try {
      await api.patch(`/Araclar/${id}/kilometre`, { yeniKm: parseInt(yeniKm) });
      setYeniKm('');
      fetchData(); // refresh data
      alert('Kilometre başarıyla güncellendi.');
    } catch (error) {
      console.error('KM güncellenemedi:', error);
      alert('Kilometre güncellenirken hata oluştu.');
    } finally {
      setKmUpdating(false);
    }
  };

  const handleStatusUpdate = async () => {
    setStatusUpdating(true);
    try {
      // API expects AracGuncelleDto. We need to pass all required fields.
      const dto = {
        marka: arac.marka,
        model: arac.model,
        yil: arac.yil,
        sasiNo: arac.sasiNo,
        yakitTipi: arac.yakitTipi,
        aracDurumu: aracDurumu,
        guncelKm: arac.guncelKilometre,
        subeId: 1 // fallback
      };
      await api.put(`/Araclar/${id}`, dto);
      fetchData();
      alert('Araç durumu başarıyla güncellendi.');
    } catch (error) {
      console.error('Durum güncellenemedi:', error);
      alert('Araç durumu güncellenirken hata oluştu.');
    } finally {
      setStatusUpdating(false);
    }
  };

  const handlePhotoUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!e.target.files || e.target.files.length === 0) return;
    const file = e.target.files[0];
    const formData = new FormData();
    formData.append('file', file);
    
    try {
      setPhotoUploading(true);
      const uploadRes = await api.post('/Upload', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      });
      const fileUrl = uploadRes.data.url;
      
      await api.patch(`/Araclar/${id}/gorsel`, `"${fileUrl}"`, {
        headers: { 'Content-Type': 'application/json' }
      });
      
      fetchData();
      alert('Fotoğraf başarıyla yüklendi.');
    } catch (error) {
      console.error('Fotoğraf yüklenemedi:', error);
      alert('Fotoğraf yüklenirken hata oluştu.');
    } finally {
      setPhotoUploading(false);
    }
  };

  const handleBelgeSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!belgeDosya) return alert("Lütfen bir dosya seçin.");

    try {
      setBelgeUploading(true);
      const formData = new FormData();
      formData.append('file', belgeDosya);
      
      const uploadRes = await api.post('/Upload', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      });
      const fileUrl = uploadRes.data.url;

      const dto = {
        aracId: parseInt(id!),
        belgeTipi: yeniBelge.tipi,
        belgeAdi: yeniBelge.adi,
        dosyaYolu: fileUrl,
        gecerlilikTarihi: yeniBelge.tarih ? yeniBelge.tarih : null,
        uyariBildirimMi: true,
        uyariGunSayisi: 30
      };

      await api.post('/Belgeler', dto);
      
      setShowBelgeForm(false);
      setYeniBelge({ adi: '', tipi: 0, tarih: '' });
      setBelgeDosya(null);
      fetchData();
      alert('Belge başarıyla eklendi.');
    } catch (error) {
      console.error('Belge eklenemedi:', error);
      alert('Belge eklenirken hata oluştu.');
    } finally {
      setBelgeUploading(false);
    }
  };

  if (loading) {
    return <div className="p-6 text-center text-slate-400">Yükleniyor...</div>;
  }

  if (!arac) {
    return (
      <div className="p-6 text-center">
        <AlertCircle className="w-12 h-12 text-rose-500 mx-auto mb-4" />
        <h2 className="text-xl text-white font-bold mb-4">Araç Bulunamadı</h2>
        <button onClick={() => navigate('/araclar')} className="text-cyan-400 underline">Araçlar listesine dön</button>
      </div>
    );
  }

  const getDurumText = (durum: number) => {
    switch (durum) {
      case 0: return 'Aktif';
      case 1: return 'Bakımda';
      case 2: return 'Seferde';
      case 3: return 'Pasif';
      default: return 'Bilinmiyor';
    }
  };
  const getDurumColor = (durum: number) => {
    switch (durum) {
      case 0: return 'text-emerald-400 bg-emerald-500/10 border-emerald-500/20';
      case 1: return 'text-amber-400 bg-amber-500/10 border-amber-500/20';
      case 2: return 'text-blue-400 bg-blue-500/10 border-blue-500/20';
      case 3: return 'text-rose-400 bg-rose-500/10 border-rose-500/20';
      default: return 'text-slate-400 bg-slate-500/10 border-slate-500/20';
    }
  };

  return (
    <div className="space-y-6 pb-20">
      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 bg-slate-900/50 p-6 rounded-2xl border border-white/5 backdrop-blur-xl">
        <div className="flex items-center gap-4">
          <button onClick={() => navigate('/araclar')} className="p-2 bg-slate-800 rounded-lg hover:bg-slate-700 text-slate-300 transition-colors">
            <ArrowLeft className="w-5 h-5" />
          </button>
          <div>
            <div className="flex items-center gap-3">
              <h1 className="text-3xl font-black text-white tracking-wide uppercase">{arac.plaka}</h1>
              <span className={`px-3 py-1 rounded-full text-xs font-bold border ${getDurumColor(arac.durumu)}`}>
                {getDurumText(arac.durumu)}
              </span>
            </div>
            <p className="text-slate-400 font-medium mt-1">{arac.marka} {arac.model} • {arac.yil}</p>
          </div>
        </div>
        <div className="flex gap-2">
          <button className="px-4 py-2 bg-slate-800 text-slate-300 rounded-lg hover:bg-slate-700 font-medium flex items-center gap-2">
            <Edit2 className="w-4 h-4" /> Düzenle
          </button>
        </div>
      </div>

      <div className="flex flex-col lg:flex-row gap-6">
        {/* Sidebar Nav */}
        <div className="w-full lg:w-64 flex-shrink-0">
          <div className="bg-slate-900/50 backdrop-blur-xl border border-white/5 rounded-2xl p-3 flex flex-col gap-1">
            <button onClick={() => setActiveTab('genel')} className={`flex items-center gap-3 px-4 py-3 rounded-xl font-medium transition-colors ${activeTab === 'genel' ? 'bg-cyan-500/10 text-cyan-400' : 'text-slate-400 hover:bg-slate-800 hover:text-slate-200'}`}>
              <Info className="w-5 h-5" /> Genel Bilgiler
            </button>
            <button onClick={() => setActiveTab('km_durum')} className={`flex items-center gap-3 px-4 py-3 rounded-xl font-medium transition-colors ${activeTab === 'km_durum' ? 'bg-cyan-500/10 text-cyan-400' : 'text-slate-400 hover:bg-slate-800 hover:text-slate-200'}`}>
              <MapPin className="w-5 h-5" /> Kilometre & Durum
            </button>
            <button onClick={() => setActiveTab('belgeler')} className={`flex items-center gap-3 px-4 py-3 rounded-xl font-medium transition-colors ${activeTab === 'belgeler' ? 'bg-cyan-500/10 text-cyan-400' : 'text-slate-400 hover:bg-slate-800 hover:text-slate-200'}`}>
              <FileText className="w-5 h-5" /> Ruhsat & Belgeler
            </button>
            <button onClick={() => setActiveTab('bakim')} className={`flex items-center gap-3 px-4 py-3 rounded-xl font-medium transition-colors ${activeTab === 'bakim' ? 'bg-cyan-500/10 text-cyan-400' : 'text-slate-400 hover:bg-slate-800 hover:text-slate-200'}`}>
              <Wrench className="w-5 h-5" /> Bakım Geçmişi
            </button>
            <button onClick={() => setActiveTab('yakit')} className={`flex items-center gap-3 px-4 py-3 rounded-xl font-medium transition-colors ${activeTab === 'yakit' ? 'bg-cyan-500/10 text-cyan-400' : 'text-slate-400 hover:bg-slate-800 hover:text-slate-200'}`}>
              <Fuel className="w-5 h-5" /> Yakıt Kayıtları
            </button>
            <button onClick={() => setActiveTab('galeri')} className={`flex items-center gap-3 px-4 py-3 rounded-xl font-medium transition-colors ${activeTab === 'galeri' ? 'bg-cyan-500/10 text-cyan-400' : 'text-slate-400 hover:bg-slate-800 hover:text-slate-200'}`}>
              <Camera className="w-5 h-5" /> Fotoğraf Galerisi
            </button>
          </div>
        </div>

        {/* Content Area */}
        <div className="flex-1 bg-slate-900/50 backdrop-blur-xl border border-white/5 rounded-2xl p-6 min-h-[500px]">
          
          {/* TAB: GENEL BİLGİLER */}
          {activeTab === 'genel' && (
            <div className="space-y-6">
              <h2 className="text-xl font-bold text-white mb-6 border-b border-white/5 pb-4">Genel Bilgiler</h2>
              
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                <div className="bg-slate-800/50 p-4 rounded-xl border border-white/5">
                  <div className="text-sm text-slate-400 mb-1">Şasi Numarası</div>
                  <div className="font-semibold text-white tracking-wider uppercase">{arac.sasiNo || '-'}</div>
                </div>
                <div className="bg-slate-800/50 p-4 rounded-xl border border-white/5">
                  <div className="text-sm text-slate-400 mb-1">Yakıt Tipi</div>
                  <div className="font-semibold text-white">
                    {arac.yakitTipi === 0 ? 'Dizel' : arac.yakitTipi === 1 ? 'Benzin' : 'Elektrik/Hibrit'}
                  </div>
                </div>
                <div className="bg-slate-800/50 p-4 rounded-xl border border-white/5">
                  <div className="text-sm text-slate-400 mb-1">Bağlı Şube / Lokasyon</div>
                  <div className="font-semibold text-white">{arac.subeAdi || 'Merkez Şube'}</div>
                </div>
                <div className="bg-slate-800/50 p-4 rounded-xl border border-white/5">
                  <div className="text-sm text-slate-400 mb-1">Güncel Kilometre</div>
                  <div className="font-semibold text-white">{(arac.guncelKilometre || 0).toLocaleString('tr-TR')} km</div>
                </div>
                <div className="bg-slate-800/50 p-4 rounded-xl border border-white/5">
                  <div className="text-sm text-slate-400 mb-1">Sisteme Kayıt Tarihi</div>
                  <div className="font-semibold text-white">{new Date(arac.createdAt).toLocaleDateString('tr-TR')}</div>
                </div>
              </div>
            </div>
          )}

          {/* TAB: KILOMETRE VE DURUM */}
          {activeTab === 'km_durum' && (
            <div className="space-y-8">
              <h2 className="text-xl font-bold text-white mb-6 border-b border-white/5 pb-4">Kilometre ve Durum Yönetimi</h2>
              
              <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                {/* KM Guncelle */}
                <div className="bg-slate-800/30 p-6 rounded-2xl border border-white/5 relative overflow-hidden">
                  <div className="absolute top-0 right-0 p-4 opacity-10">
                    <Hash className="w-32 h-32 text-cyan-400" />
                  </div>
                  <h3 className="text-lg font-bold text-white mb-4 relative z-10">Kilometre Güncelle</h3>
                  <p className="text-sm text-slate-400 mb-6 relative z-10">Aracın güncel kilometresi: <span className="font-bold text-cyan-400">{(arac.guncelKilometre || 0).toLocaleString('tr-TR')} km</span></p>
                  
                  <form onSubmit={handleKmUpdate} className="space-y-4 relative z-10">
                    <div>
                      <label className="block text-sm text-slate-300 mb-2">Yeni Kilometre</label>
                      <div className="flex gap-2">
                        <input 
                          type="number" required
                          min={arac.guncelKilometre || 0}
                          value={yeniKm}
                          onChange={e => setYeniKm(e.target.value)}
                          placeholder="Örn: 155000"
                          className="flex-1 bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-cyan-500"
                        />
                        <button type="submit" disabled={kmUpdating} className="px-6 py-2 bg-cyan-500 hover:bg-cyan-400 text-slate-950 font-semibold rounded-lg transition-colors">
                          {kmUpdating ? 'Güncelleniyor...' : 'Kaydet'}
                        </button>
                      </div>
                    </div>
                  </form>
                </div>

                {/* Durum Guncelle */}
                <div className="bg-slate-800/30 p-6 rounded-2xl border border-white/5">
                  <h3 className="text-lg font-bold text-white mb-4">Araç Durumu Değiştir</h3>
                  <p className="text-sm text-slate-400 mb-6">Mevcut Durum: <span className="font-bold text-white">{getDurumText(arac.durumu)}</span></p>
                  
                  <div className="space-y-4">
                    <select 
                      value={aracDurumu}
                      onChange={e => setAracDurumu(parseInt(e.target.value))}
                      className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-cyan-500"
                    >
                      <option value={0}>Aktif</option>
                      <option value={1}>Bakımda</option>
                      <option value={2}>Seferde</option>
                      <option value={3}>Pasif (Hizmet Dışı)</option>
                    </select>
                    <button 
                      onClick={handleStatusUpdate} 
                      disabled={statusUpdating || aracDurumu === arac.durumu}
                      className="w-full py-2 bg-blue-500 hover:bg-blue-400 text-white font-semibold rounded-lg transition-colors disabled:opacity-50"
                    >
                      {statusUpdating ? 'Güncelleniyor...' : 'Durumu Güncelle'}
                    </button>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* TAB: BELGELER */}
          {activeTab === 'belgeler' && (
            <div className="space-y-6">
              <div className="flex justify-between items-center border-b border-white/5 pb-4">
                <h2 className="text-xl font-bold text-white">Ruhsat ve Belgeler</h2>
                <button 
                  onClick={() => setShowBelgeForm(!showBelgeForm)}
                  className="text-sm bg-slate-800 text-slate-300 px-3 py-1.5 rounded hover:bg-slate-700"
                >
                  {showBelgeForm ? 'İptal' : 'Yeni Belge Yükle'}
                </button>
              </div>

              {showBelgeForm && (
                <form onSubmit={handleBelgeSubmit} className="bg-slate-800/50 p-6 rounded-xl border border-white/5 space-y-4">
                  <h3 className="text-white font-semibold mb-2">Yeni Belge Ekle</h3>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm text-slate-300 mb-1">Belge Adı</label>
                      <input 
                        required type="text" 
                        value={yeniBelge.adi} onChange={e => setYeniBelge({...yeniBelge, adi: e.target.value})}
                        className="w-full bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-white" 
                        placeholder="Örn: Ruhsat, Vergi Levhası..."
                      />
                    </div>
                    <div>
                      <label className="block text-sm text-slate-300 mb-1">Belge Tipi</label>
                      <select 
                        value={yeniBelge.tipi} onChange={e => setYeniBelge({...yeniBelge, tipi: parseInt(e.target.value)})}
                        className="w-full bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-white"
                      >
                        <option value={0}>Ruhsat</option>
                        <option value={1}>Muayene</option>
                        <option value={2}>Diğer</option>
                      </select>
                    </div>
                    <div>
                      <label className="block text-sm text-slate-300 mb-1">Geçerlilik Tarihi (Opsiyonel)</label>
                      <input 
                        type="date" 
                        value={yeniBelge.tarih} onChange={e => setYeniBelge({...yeniBelge, tarih: e.target.value})}
                        className="w-full bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-white" 
                      />
                    </div>
                    <div>
                      <label className="block text-sm text-slate-300 mb-1">Dosya</label>
                      <input 
                        required type="file" 
                        onChange={e => setBelgeDosya(e.target.files ? e.target.files[0] : null)}
                        className="w-full text-slate-300 text-sm file:mr-4 file:py-2 file:px-4 file:rounded-lg file:border-0 file:bg-slate-700 file:text-white" 
                      />
                    </div>
                  </div>
                  <div className="flex justify-end mt-4">
                    <button 
                      type="submit" disabled={belgeUploading}
                      className="px-6 py-2 bg-cyan-500 hover:bg-cyan-400 text-slate-950 font-semibold rounded-lg disabled:opacity-50"
                    >
                      {belgeUploading ? 'Yükleniyor...' : 'Kaydet ve Yükle'}
                    </button>
                  </div>
                </form>
              )}

              {belgeler.length === 0 ? (
                <div className="text-center py-10">
                  <FileText className="w-12 h-12 text-slate-600 mx-auto mb-3" />
                  <p className="text-slate-400">Bu araca ait kayıtlı belge bulunmuyor.</p>
                </div>
              ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {belgeler.map(belge => (
                    <div key={belge.id} className="bg-slate-800/40 p-4 rounded-xl border border-white/5 flex justify-between items-start">
                      <div>
                        <h4 className="font-semibold text-white">{belge.belgeAdi}</h4>
                        <p className="text-xs text-slate-400 mt-1">Tür: {belge.belgeTipi === 0 ? 'Ruhsat' : belge.belgeTipi === 1 ? 'Muayene' : 'Diğer'}</p>
                        {belge.gecerlilikTarihi && (
                          <p className={`text-xs mt-2 ${new Date(belge.gecerlilikTarihi) < new Date() ? 'text-rose-400' : 'text-emerald-400'}`}>
                            Geçerlilik: {new Date(belge.gecerlilikTarihi).toLocaleDateString('tr-TR')}
                          </p>
                        )}
                      </div>
                      <a href={belge.dosyaYolu} target="_blank" rel="noopener noreferrer" className="text-cyan-400 text-sm hover:underline">Görüntüle</a>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {/* TAB: BAKIM */}
          {activeTab === 'bakim' && (
            <div className="space-y-6">
              <h2 className="text-xl font-bold text-white border-b border-white/5 pb-4">Bakım ve Arıza Geçmişi</h2>
              
              {bakimlar.length === 0 ? (
                <div className="text-center py-10">
                  <Wrench className="w-12 h-12 text-slate-600 mx-auto mb-3" />
                  <p className="text-slate-400">Bu araca ait bakım kaydı bulunmuyor.</p>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full text-left border-collapse">
                    <thead>
                      <tr className="border-b border-white/10 text-slate-400 text-sm">
                        <th className="py-3 px-4">Tarih</th>
                        <th className="py-3 px-4">İşlem Türü</th>
                        <th className="py-3 px-4">Açıklama</th>
                        <th className="py-3 px-4 text-right">Maliyet</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-white/5">
                      {bakimlar.map(b => (
                        <tr key={b.id} className="text-slate-300 hover:bg-white/[0.02]">
                          <td className="py-3 px-4">{new Date(b.tarih).toLocaleDateString('tr-TR')}</td>
                          <td className="py-3 px-4">{b.islemTuru}</td>
                          <td className="py-3 px-4 text-sm text-slate-400">{b.aciklama}</td>
                          <td className="py-3 px-4 text-right font-medium text-white">{(b.maliyet || 0).toLocaleString('tr-TR')} ₺</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}

          {/* TAB: YAKIT */}
          {activeTab === 'yakit' && (
            <div className="space-y-6">
              <h2 className="text-xl font-bold text-white border-b border-white/5 pb-4">Yakıt Alım Kayıtları</h2>
              
              {yakitlar.length === 0 ? (
                <div className="text-center py-10">
                  <Fuel className="w-12 h-12 text-slate-600 mx-auto mb-3" />
                  <p className="text-slate-400">Bu araca ait yakıt kaydı bulunmuyor.</p>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full text-left border-collapse">
                    <thead>
                      <tr className="border-b border-white/10 text-slate-400 text-sm">
                        <th className="py-3 px-4">Tarih</th>
                        <th className="py-3 px-4">Kilometre</th>
                        <th className="py-3 px-4">Miktar</th>
                        <th className="py-3 px-4">Tutar</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-white/5">
                      {yakitlar.map(y => (
                        <tr key={y.id} className="text-slate-300 hover:bg-white/[0.02]">
                          <td className="py-3 px-4">{new Date(y.tarih).toLocaleDateString('tr-TR')}</td>
                          <td className="py-3 px-4">{y.kilometre.toLocaleString('tr-TR')} km</td>
                          <td className="py-3 px-4 text-cyan-400">{y.litre} LT</td>
                          <td className="py-3 px-4 font-medium text-white">{y.toplamTutar.toLocaleString('tr-TR')} ₺</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}

          {/* TAB: GALERİ */}
          {activeTab === 'galeri' && (
            <div className="space-y-6">
              <div className="flex justify-between items-center border-b border-white/5 pb-4">
                <h2 className="text-xl font-bold text-white">Fotoğraf Galerisi</h2>
                <label className="cursor-pointer text-sm bg-cyan-500 text-slate-950 font-medium px-4 py-2 rounded-lg hover:bg-cyan-400 transition-colors">
                  {photoUploading ? 'Yükleniyor...' : 'Kapak Fotoğrafı Yükle'}
                  <input type="file" className="hidden" accept="image/*" onChange={handlePhotoUpload} disabled={photoUploading} />
                </label>
              </div>
              
              {arac.fotografUrl || arac.gorselUrl ? (
                <div className="rounded-2xl overflow-hidden border border-white/10 max-w-2xl mx-auto">
                  <img 
                    src={(arac.fotografUrl || arac.gorselUrl).startsWith('http') ? (arac.fotografUrl || arac.gorselUrl) : `http://localhost:5223${arac.fotografUrl || arac.gorselUrl}`} 
                    alt={arac.plaka} 
                    className="w-full h-auto object-cover"
                  />
                </div>
              ) : (
                <div className="text-center py-16 bg-slate-800/20 rounded-2xl border border-dashed border-slate-700">
                  <Camera className="w-16 h-16 text-slate-600 mx-auto mb-4" />
                  <h3 className="text-lg font-medium text-white mb-2">Henüz Fotoğraf Yok</h3>
                  <p className="text-slate-400 text-sm max-w-md mx-auto">Sağ üstten araca ait bir kapak fotoğrafı yükleyebilirsiniz.</p>
                </div>
              )}
            </div>
          )}

        </div>
      </div>
    </div>
  );
}

import { useState, useEffect } from 'react';
import api from '../lib/api';
import { FileText, Shield, File, Download, AlertTriangle } from 'lucide-react';

interface Belge {
  id: number;
  aracId: number;
  belgeTuru: string;
  dosyaYolu: string;
  yuklenmeTarihi: string;
}

interface Sigorta {
  id: number;
  aracId: number;
  sigortaSirketi: string;
  policeNo: string;
  baslangicTarihi: string;
  bitisTarihi: string;
  primTutari: number;
}

interface Arac {
  id: number;
  plaka: string;
  marka: string;
}

const Belgeler = () => {
  const [activeTab, setActiveTab] = useState<'sigortalar' | 'belgeler'>('sigortalar');
  const [sigortalar, setSigortalar] = useState<Sigorta[]>([]);
  const [belgeler, setBelgeler] = useState<Belge[]>([]);
  const [araclar, setAraclar] = useState<Arac[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchAraclar();
    fetchData();
  }, [activeTab]);

  const fetchData = async () => {
    setLoading(true);
    try {
      if (activeTab === 'sigortalar') {
        const response = await api.get('/Sigortalar');
        setSigortalar(response.data || []);
      } else {
        const response = await api.get('/Belgeler');
        setBelgeler(response.data || []);
      }
    } catch (error) {
      console.error('Veriler alınamadı:', error);
    } finally {
      setLoading(false);
    }
  };

  const fetchAraclar = async () => {
    try {
      const response = await api.get('/Araclar?sayfa=1&boyut=1000');
      if (response.data?.veriler) setAraclar(response.data.veriler);
    } catch (error) {
      console.error(error);
    }
  };

  const isExpired = (dateString: string) => new Date(dateString) < new Date();
  const getAracPlaka = (aracId: number) => araclar.find(a => a.id === aracId)?.plaka || 'Bilinmiyor';

  return (
    <div className="p-6 max-w-7xl mx-auto">
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-3">
            <FileText className="h-8 w-8 text-blue-600" />
            Belge & Sigorta Yönetimi
          </h1>
          <p className="text-gray-500 mt-1">Araç poliçeleri, muayene evrakları ve diğer belgeler</p>
        </div>
      </div>

      <div className="flex border-b border-gray-200 mb-6">
        <button
          onClick={() => setActiveTab('sigortalar')}
          className={`pb-4 px-4 text-sm font-medium border-b-2 transition-colors flex items-center gap-2 ${
            activeTab === 'sigortalar'
              ? 'border-blue-600 text-blue-600'
              : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
          }`}
        >
          <Shield className="w-4 h-4" /> Sigorta Poliçeleri
        </button>
        <button
          onClick={() => setActiveTab('belgeler')}
          className={`pb-4 px-4 text-sm font-medium border-b-2 transition-colors flex items-center gap-2 ${
            activeTab === 'belgeler'
              ? 'border-blue-600 text-blue-600'
              : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
          }`}
        >
          <File className="w-4 h-4" /> Diğer Belgeler
        </button>
      </div>

      {loading ? (
        <div className="text-center py-10">Yükleniyor...</div>
      ) : (
        <div className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          {activeTab === 'sigortalar' ? (
            <div className="overflow-x-auto">
              <table className="w-full text-left border-collapse">
                <thead>
                  <tr className="bg-gray-50 border-b border-gray-100">
                    <th className="p-4 text-sm font-semibold text-gray-600">Araç (Plaka)</th>
                    <th className="p-4 text-sm font-semibold text-gray-600">Şirket</th>
                    <th className="p-4 text-sm font-semibold text-gray-600">Poliçe No</th>
                    <th className="p-4 text-sm font-semibold text-gray-600">Başlangıç</th>
                    <th className="p-4 text-sm font-semibold text-gray-600">Bitiş</th>
                    <th className="p-4 text-sm font-semibold text-gray-600">Prim (₺)</th>
                  </tr>
                </thead>
                <tbody>
                  {sigortalar.length === 0 ? (
                    <tr><td colSpan={6} className="p-8 text-center text-gray-500">Poliçe kaydı bulunamadı.</td></tr>
                  ) : (
                    sigortalar.map(s => {
                      const expired = isExpired(s.bitisTarihi);
                      return (
                        <tr key={s.id} className="border-b border-gray-50 hover:bg-gray-50 transition-colors">
                          <td className="p-4 font-bold text-gray-900">{getAracPlaka(s.aracId)}</td>
                          <td className="p-4 text-sm text-gray-600">{s.sigortaSirketi}</td>
                          <td className="p-4 text-sm text-gray-500">{s.policeNo}</td>
                          <td className="p-4 text-sm text-gray-600">{new Date(s.baslangicTarihi).toLocaleDateString('tr-TR')}</td>
                          <td className="p-4 text-sm">
                            <span className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium ${expired ? 'bg-red-50 text-red-700 border border-red-200' : 'bg-green-50 text-green-700 border border-green-200'}`}>
                              {expired && <AlertTriangle className="w-3.5 h-3.5" />}
                              {new Date(s.bitisTarihi).toLocaleDateString('tr-TR')}
                            </span>
                          </td>
                          <td className="p-4 font-medium text-gray-900">{s.primTutari.toLocaleString('tr-TR')} ₺</td>
                        </tr>
                      );
                    })
                  )}
                </tbody>
              </table>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-left border-collapse">
                <thead>
                  <tr className="bg-gray-50 border-b border-gray-100">
                    <th className="p-4 text-sm font-semibold text-gray-600">Araç (Plaka)</th>
                    <th className="p-4 text-sm font-semibold text-gray-600">Belge Türü</th>
                    <th className="p-4 text-sm font-semibold text-gray-600">Yüklenme Tarihi</th>
                    <th className="p-4 text-sm font-semibold text-gray-600">İşlem</th>
                  </tr>
                </thead>
                <tbody>
                  {belgeler.length === 0 ? (
                    <tr><td colSpan={4} className="p-8 text-center text-gray-500">Belge bulunamadı.</td></tr>
                  ) : (
                    belgeler.map(b => (
                      <tr key={b.id} className="border-b border-gray-50 hover:bg-gray-50 transition-colors">
                        <td className="p-4 font-bold text-gray-900">{getAracPlaka(b.aracId)}</td>
                        <td className="p-4 text-sm text-gray-600">{b.belgeTuru}</td>
                        <td className="p-4 text-sm text-gray-500">{new Date(b.yuklenmeTarihi).toLocaleDateString('tr-TR')}</td>
                        <td className="p-4 text-sm">
                          <a href={b.dosyaYolu} target="_blank" rel="noreferrer" className="text-blue-600 hover:text-blue-800 font-medium inline-flex items-center gap-1">
                            <Download className="w-4 h-4" /> İndir
                          </a>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default Belgeler;

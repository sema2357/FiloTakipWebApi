import { useState, useEffect } from 'react';
import api from '../lib/api';
import { BarChart3, PieChart, FileSpreadsheet, Activity, Wallet, Car, Wrench } from 'lucide-react';

interface FiloOzeti {
  toplamArac: number;
  aktifSeferSayisi: number;
  aylikToplamYakitMaliyeti: number;
  aylikToplamBakimMaliyeti: number;
}

interface AracMaliyet {
  aracId: number;
  plaka: string;
  toplamMaliyet: number;
  yakitMaliyeti: number;
  bakimMaliyeti: number;
}

const Raporlar = () => {
  const [ozet, setOzet] = useState<FiloOzeti | null>(null);
  const [aracMaliyetleri, setAracMaliyetleri] = useState<AracMaliyet[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    setLoading(true);
    try {
      const today = new Date();
      const firstDay = new Date(today.getFullYear(), today.getMonth(), 1).toISOString();
      const lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0).toISOString();

      const [ozetRes, maliyetRes] = await Promise.all([
        api.get('/Raporlar/filo-ozeti'),
        api.get(`/Raporlar/arac-maliyeti?baslangic=${firstDay}&bitis=${lastDay}`)
      ]);
      setOzet(ozetRes.data);
      setAracMaliyetleri(maliyetRes.data || []);
    } catch (error) {
      console.error('Rapor verileri alınamadı:', error);
    } finally {
      setLoading(false);
    }
  };

  const StatCard = ({ title, value, icon, subtitle }: any) => (
    <div className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100 flex items-start gap-4">
      <div className="p-3 bg-blue-50 text-blue-600 rounded-xl">
        {icon}
      </div>
      <div>
        <p className="text-sm font-medium text-gray-500 mb-1">{title}</p>
        <h3 className="text-2xl font-bold text-gray-900">{value}</h3>
        {subtitle && <p className="text-xs text-gray-400 mt-1">{subtitle}</p>}
      </div>
    </div>
  );

  return (
    <div className="p-6 max-w-7xl mx-auto">
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-3">
            <BarChart3 className="h-8 w-8 text-blue-600" />
            Raporlar & Analiz
          </h1>
          <p className="text-gray-500 mt-1">Filo maliyetleri, performans ve operasyonel veriler</p>
        </div>
        <button className="bg-green-600 text-white px-5 py-2.5 rounded-xl hover:bg-green-700 transition-colors flex items-center gap-2 font-medium shadow-sm hover:shadow-md">
          <FileSpreadsheet className="h-5 w-5" />
          Excel'e Aktar
        </button>
      </div>

      {loading ? (
        <div className="text-center py-10">Veriler yükleniyor...</div>
      ) : (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
            <StatCard
              title="Toplam Araç"
              value={ozet?.toplamArac || 0}
              icon={<Car className="w-6 h-6" />}
              subtitle="Aktif Filo Büyüklüğü"
            />
            <StatCard
              title="Aktif Seferler"
              value={ozet?.aktifSeferSayisi || 0}
              icon={<Activity className="w-6 h-6" />}
              subtitle="Şu an devam eden"
            />
            <StatCard
              title="Aylık Yakıt Gideri"
              value={`${(ozet?.aylikToplamYakitMaliyeti || 0).toLocaleString('tr-TR')} ₺`}
              icon={<Wallet className="w-6 h-6" />}
              subtitle="Bu ayki toplam yakıt"
            />
            <StatCard
              title="Aylık Bakım Gideri"
              value={`${(ozet?.aylikToplamBakimMaliyeti || 0).toLocaleString('tr-TR')} ₺`}
              icon={<Wrench className="w-6 h-6" />} // Replaced Wrench with standard lucid icon or just imported Wrench
              subtitle="Bu ayki toplam servis"
            />
          </div>

          <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 className="text-lg font-bold text-gray-900 mb-6 flex items-center gap-2">
              <PieChart className="w-5 h-5 text-gray-400" /> Araç Bazlı Maliyet Analizi
            </h2>
            <div className="overflow-x-auto">
              <table className="w-full text-left border-collapse">
                <thead>
                  <tr className="bg-gray-50 border-b border-gray-100">
                    <th className="p-4 text-sm font-semibold text-gray-600">Araç (Plaka)</th>
                    <th className="p-4 text-sm font-semibold text-gray-600">Yakıt Maliyeti</th>
                    <th className="p-4 text-sm font-semibold text-gray-600">Bakım Maliyeti</th>
                    <th className="p-4 text-sm font-semibold text-gray-600">Toplam Maliyet</th>
                  </tr>
                </thead>
                <tbody>
                  {aracMaliyetleri.length === 0 ? (
                    <tr><td colSpan={4} className="p-8 text-center text-gray-500">Maliyet verisi bulunamadı.</td></tr>
                  ) : (
                    aracMaliyetleri.map((arac, idx) => (
                      <tr key={idx} className="border-b border-gray-50 hover:bg-gray-50 transition-colors">
                        <td className="p-4 font-bold text-gray-900">{arac.plaka}</td>
                        <td className="p-4 text-sm text-gray-600">{arac.yakitMaliyeti?.toLocaleString('tr-TR')} ₺</td>
                        <td className="p-4 text-sm text-gray-600">{arac.bakimMaliyeti?.toLocaleString('tr-TR')} ₺</td>
                        <td className="p-4 font-bold text-blue-600">{arac.toplamMaliyet?.toLocaleString('tr-TR')} ₺</td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default Raporlar;

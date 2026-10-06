import { useEffect, useState } from 'react';
import api from '../lib/api';
import { 
  Car, Users, Route, Activity, Wrench, Fuel, 
  TrendingUp, TrendingDown, ArrowRight 
} from 'lucide-react';
import { clsx } from 'clsx';

interface OzetData {
  toplamArac: number;
  aktifArac: number;
  bakimdakiArac: number;
  toplamSofor: number;
  bugunkuSefer: number;
  devamEdenSefer: number;
  aylikYakitMaliyeti: number;
  aylikBakimMaliyeti: number;
}

export function Dashboard() {
  const [data, setData] = useState<OzetData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchOzet = async () => {
      try {
        const response = await api.get('/Rapor/filo-ozeti');
        setData(response.data);
      } catch (error) {
        console.error('Özet verisi alınamadı:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchOzet();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-[60vh]">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-cyan-400"></div>
      </div>
    );
  }

  const StatCard = ({ title, value, icon: Icon, color, trend }: any) => (
    <div className="relative group rounded-2xl p-[1px] overflow-hidden">
      <div className={clsx("absolute inset-0 bg-gradient-to-br opacity-40 group-hover:opacity-100 transition-opacity duration-500 blur-xl", color)} />
      <div className="relative bg-slate-900/80 backdrop-blur-md rounded-2xl p-6 border border-white/5 hover:border-white/10 transition-colors h-full flex flex-col justify-between">
        <div className="flex justify-between items-start mb-4">
          <div className={clsx("p-3 rounded-xl bg-gradient-to-br bg-opacity-10", color)}>
            <Icon className="w-6 h-6 text-white" />
          </div>
          {trend && (
            <div className={clsx("flex items-center gap-1 text-sm font-medium", trend > 0 ? "text-emerald-400" : "text-rose-400")}>
              {trend > 0 ? <TrendingUp className="w-4 h-4" /> : <TrendingDown className="w-4 h-4" />}
              {Math.abs(trend)}%
            </div>
          )}
        </div>
        <div>
          <h3 className="text-slate-400 font-medium mb-1">{title}</h3>
          <p className="text-3xl font-bold text-white">{value}</p>
        </div>
      </div>
    </div>
  );

  return (
    <div className="space-y-8">
      <div className="flex flex-col sm:flex-row sm:items-end justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-white mb-2">Genel Bakış</h1>
          <p className="text-slate-400">Filonuzun güncel durumunu takip edin.</p>
        </div>
        <button className="flex items-center gap-2 text-cyan-400 hover:text-cyan-300 font-medium group transition-colors">
          Detaylı Raporlar
          <ArrowRight className="w-4 h-4 group-hover:translate-x-1 transition-transform" />
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <StatCard 
          title="Toplam Araç" 
          value={data?.toplamArac || 0} 
          icon={Car} 
          color="from-blue-500 to-cyan-400" 
        />
        <StatCard 
          title="Aktif Seferler" 
          value={data?.devamEdenSefer || 0} 
          icon={Route} 
          color="from-emerald-500 to-teal-400" 
          trend={+12}
        />
        <StatCard 
          title="Bakımdaki Araçlar" 
          value={data?.bakimdakiArac || 0} 
          icon={Wrench} 
          color="from-orange-500 to-amber-400" 
        />
        <StatCard 
          title="Aktif Şoförler" 
          value={data?.toplamSofor || 0} 
          icon={Users} 
          color="from-purple-500 to-pink-400" 
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mt-8">
        <div className="bg-slate-900/50 backdrop-blur-md rounded-2xl border border-white/5 p-6">
          <h2 className="text-xl font-bold text-white mb-6 flex items-center gap-2">
            <Fuel className="w-5 h-5 text-cyan-400" />
            Aylık Maliyet Özeti
          </h2>
          <div className="space-y-6">
            <div>
              <div className="flex justify-between text-sm mb-2">
                <span className="text-slate-400">Yakıt Maliyeti</span>
                <span className="text-white font-medium">₺{data?.aylikYakitMaliyeti.toLocaleString('tr-TR')}</span>
              </div>
              <div className="h-2 rounded-full bg-slate-800 overflow-hidden">
                <div className="h-full bg-gradient-to-r from-cyan-400 to-blue-500 rounded-full w-[70%]" />
              </div>
            </div>
            <div>
              <div className="flex justify-between text-sm mb-2">
                <span className="text-slate-400">Bakım Maliyeti</span>
                <span className="text-white font-medium">₺{data?.aylikBakimMaliyeti.toLocaleString('tr-TR')}</span>
              </div>
              <div className="h-2 rounded-full bg-slate-800 overflow-hidden">
                <div className="h-full bg-gradient-to-r from-orange-400 to-amber-500 rounded-full w-[30%]" />
              </div>
            </div>
          </div>
        </div>

        <div className="bg-slate-900/50 backdrop-blur-md rounded-2xl border border-white/5 p-6">
           <h2 className="text-xl font-bold text-white mb-6 flex items-center gap-2">
            <Activity className="w-5 h-5 text-emerald-400" />
            Günlük Operasyon
          </h2>
          <div className="flex items-center justify-center h-40">
             <div className="text-center">
                <div className="text-5xl font-extrabold text-transparent bg-clip-text bg-gradient-to-r from-emerald-400 to-cyan-400 mb-2">
                  {data?.bugunkuSefer || 0}
                </div>
                <div className="text-slate-400">Bugün Planlanan Sefer</div>
             </div>
          </div>
        </div>
      </div>
    </div>
  );
}

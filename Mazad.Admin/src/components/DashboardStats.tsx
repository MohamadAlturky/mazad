import React from 'react';
import { Users, MapPin, Grid3X3, TrendingUp } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useLanguage } from '@/contexts/LanguageContext';

const DashboardStats: React.FC = () => {
  const { t } = useLanguage();

  const stats = [
    {
      title: t('totalUsers'),
      value: '1,234',
      icon: Users,
      change: '+12%',
      changeType: 'positive' as const,
    },
    {
      title: t('totalRegions'),
      value: '45',
      icon: MapPin,
      change: '+5%',
      changeType: 'positive' as const,
    },
    {
      title: t('totalCategories'),
      value: '89',
      icon: Grid3X3,
      change: '+8%',
      changeType: 'positive' as const,
    },
    {
      title: 'Active Auctions',
      value: '156',
      icon: TrendingUp,
      change: '+23%',
      changeType: 'positive' as const,
    },
  ];

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      {stats.map((stat, index) => (
        <Card key={index} className="border-purple-200 hover:shadow-lg transition-shadow bg-white">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium text-purple-700">
              {stat.title}
            </CardTitle>
            <stat.icon className="h-5 w-5 text-purple-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-purple-900">{stat.value}</div>
            <p className="text-xs text-purple-600 mt-1">
              {stat.change} from last month
            </p>
          </CardContent>
        </Card>
      ))}
    </div>
  );
};

export default DashboardStats;

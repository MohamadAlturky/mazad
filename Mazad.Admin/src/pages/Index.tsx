import React from 'react';
import { useLanguage } from '@/contexts/LanguageContext';
import AdminLayout from '@/components/AdminLayout';
import DashboardStats from '@/components/DashboardStats';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';

const Index = () => {
  const { t } = useLanguage();

  return (
    <AdminLayout>
      <div className="space-y-6">
        {/* Welcome Section */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-purple-900 mb-2">
            {t('welcome')} {t('mazad')} {t('adminPanel')}
          </h1>
          <p className="text-purple-600">
            Manage your auction platform with ease and efficiency
          </p>
        </div>

        {/* Statistics Cards */}
        <DashboardStats />

        {/* Recent Activity */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <Card className="border-purple-200">
            <CardHeader>
              <CardTitle className="text-purple-900">Recent Users</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                {['أحمد محمد', 'فاطمة علي', 'محمد خالد', 'سارة أحمد'].map((name, index) => (
                  <div key={index} className="flex items-center justify-between p-3 bg-purple-50 rounded-lg">
                    <div className="flex items-center gap-3">
                      <div className="w-8 h-8 bg-purple-200 rounded-full flex items-center justify-center">
                        <span className="text-purple-700 font-semibold text-sm">
                          {name.charAt(0)}
                        </span>
                      </div>
                      <span className="text-purple-900">{name}</span>
                    </div>
                    <span className="text-sm text-purple-600">Today</span>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>

          <Card className="border-purple-200">
            <CardHeader>
              <CardTitle className="text-purple-900">Quick Actions</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-2 gap-3">
                {[
                  { label: t('addUser'), color: 'bg-purple-500 hover:bg-purple-600' },
                  { label: t('addRegion'), color: 'bg-purple-600 hover:bg-purple-700' },
                  { label: t('addCategory'), color: 'bg-purple-700 hover:bg-purple-800' },
                  { label: 'View Reports', color: 'bg-purple-800 hover:bg-purple-900' },
                ].map((action, index) => (
                  <button
                    key={index}
                    className={`${action.color} text-white p-4 rounded-lg transition-colors text-sm font-medium`}
                  >
                    {action.label}
                  </button>
                ))}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </AdminLayout>
  );
};

export default Index;

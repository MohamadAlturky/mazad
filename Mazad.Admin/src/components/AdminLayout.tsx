import React, { useState } from 'react';
import { Menu, Globe, Users, MapPin, Grid3X3 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { useLanguage } from '@/contexts/LanguageContext';
import { cn } from '@/lib/utils';
import { useLocation, Link } from 'react-router-dom';

interface AdminLayoutProps {
  children: React.ReactNode;
}

const AdminLayout: React.FC<AdminLayoutProps> = ({ children }) => {
  const [sidebarOpen, setSidebarOpen] = useState(true);
  const { language, setLanguage, t, isRTL } = useLanguage();
  const location = useLocation();

  const menuItems = [
    { icon: Grid3X3, label: t('dashboard'), href: '/' },
    { icon: Users, label: t('users'), href: '/users' },
    { icon: MapPin, label: t('regions'), href: '/regions' },
    { icon: Grid3X3, label: t('categories'), href: '/categories' },
  ];

  return (
    <div className={cn("min-h-screen bg-gray-50", isRTL ? "rtl" : "ltr")}>
      {/* Navbar */}
      <nav className="bg-white border-b border-purple-200 px-4 py-3 fixed w-full top-0 z-50 shadow-sm">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-4">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => setSidebarOpen(!sidebarOpen)}
              className="text-purple-600 hover:bg-purple-50"
            >
              <Menu className="h-5 w-5" />
            </Button>
            
            <div className="flex items-center gap-2">
              <div className="w-8 h-8 bg-gradient-to-br from-purple-500 to-purple-600 rounded-lg flex items-center justify-center">
                <span className="text-white font-bold text-sm">م</span>
              </div>
              <div>
                <h1 className="text-xl font-bold text-purple-900">{t('mazad')}</h1>
                <p className="text-sm text-purple-600">{t('adminPanel')}</p>
              </div>
            </div>
          </div>

          <Button
            variant="outline"
            size="sm"
            onClick={() => setLanguage(language === 'ar' ? 'en' : 'ar')}
            className="text-purple-600 border-purple-200 hover:bg-purple-50"
          >
            <Globe className="h-4 w-4 me-2" />
            {language === 'ar' ? 'English' : 'العربية'}
          </Button>
        </div>
      </nav>

      {/* Sidebar */}
      <aside className={cn(
        "fixed top-0 left-0 z-40 h-screen pt-20 transition-transform bg-white border-e border-purple-200 shadow-lg",
        isRTL && "left-auto right-0 border-e-0 border-s",
        sidebarOpen ? "translate-x-0" : (isRTL ? "translate-x-full" : "-translate-x-full"),
        "w-64"
      )}>
        <div className="h-full px-3 pb-4 overflow-y-auto">
          <ul className="space-y-2 font-medium">
            {menuItems.map((item, index) => (
              <li key={index}>
                <Link
                  to={item.href}
                  className={cn(
                    "flex items-center p-3 rounded-lg transition-colors",
                    location.pathname === item.href
                      ? "bg-purple-100 text-purple-900" 
                      : "text-purple-700 hover:bg-purple-50"
                  )}
                >
                  <item.icon className={cn("w-5 h-5", isRTL ? "ml-3" : "mr-3")} />
                  <span className="flex-1">{item.label}</span>
                </Link>
              </li>
            ))}
          </ul>
        </div>
      </aside>

      {/* Main Content */}
      <main className={cn(
        "pt-20 transition-all duration-300",
        sidebarOpen ? (isRTL ? "mr-64" : "ml-64") : "ml-0"
      )}>
        <div className="p-6">
          {children}
        </div>
      </main>

      {/* Overlay for mobile */}
      {sidebarOpen && (
        <div 
          className="fixed inset-0 z-30 bg-black/50 md:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}
    </div>
  );
};

export default AdminLayout;

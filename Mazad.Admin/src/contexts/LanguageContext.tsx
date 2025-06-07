import React, { createContext, useContext, useState, useEffect } from 'react';

type Language = 'ar' | 'en';

interface LanguageContextType {
  language: Language;
  setLanguage: (lang: Language) => void;
  t: (key: string) => string;
  isRTL: boolean;
}

const translations = {
  ar: {
    // App Name
    mazad: 'مزاد',
    adminPanel: 'لوحة الإدارة',
    
    // Navigation
    dashboard: 'الرئيسية',
    users: 'المستخدمين',
    regions: 'المناطق',
    categories: 'الفئات',
    settings: 'الإعدادات',
    
    // Actions
    add: 'إضافة',
    edit: 'تعديل',
    delete: 'حذف',
    view: 'عرض',
    save: 'حفظ',
    cancel: 'إلغاء',
    search: 'بحث',
    
    // Table Headers
    name: 'الاسم',
    email: 'البريد الإلكتروني',
    status: 'الحالة',
    createdAt: 'تاريخ الإنشاء',
    actions: 'الإجراءات',
    
    // Status
    active: 'نشط',
    inactive: 'غير نشط',
    
    // Pagination
    previous: 'السابق',
    next: 'التالي',
    page: 'صفحة',
    of: 'من',
    
    // Forms
    addUser: 'إضافة مستخدم جديد',
    addRegion: 'إضافة منطقة جديدة',
    addCategory: 'إضافة فئة جديدة',
    code: 'رمز',
    codeRequired: 'الرمز مطلوب',
    enterRegionCode: 'أدخل رمز المنطقة',
    nameRequired: 'الاسم مطلوب',
    enterRegionName: 'أدخل اسم المنطقة',
    addSubcategory: 'إضافة فئة فرعية',
    subcategories: 'الفئات الفرعية',
    
    // Welcome
    welcome: 'مرحباً بك في',
    totalUsers: 'إجمالي المستخدمين',
    totalRegions: 'إجمالي المناطق',
    totalCategories: 'إجمالي الفئات',
    toggleActivation: 'تبديل التفعيل',
    errorTogglingActivation: 'حدث خطأ أثناء تبديل التفعيل',
    
    // Activation Toggle
    activate: 'تفعيل',
    deactivate: 'إلغاء تفعيل',
    noParent: 'فئة أساسية بدون أب',
    
    // New Key for Categories Tree
    categoriesTree: 'شجرة الفئات',
    parentName: 'الفئة الأب',
    noDataAvailable: 'لا يوجد بيانات متاحة',
  },
  en: {
    // App Name
    mazad: 'Mazad',
    adminPanel: 'Admin Panel',
    
    // Navigation
    dashboard: 'Dashboard',
    users: 'Users',
    regions: 'Regions',
    categories: 'Categories',
    settings: 'Settings',
    
    // Actions
    add: 'Add',
    edit: 'Edit',
    delete: 'Delete',
    view: 'View',
    save: 'Save',
    cancel: 'Cancel',
    search: 'Search',
    
    // Table Headers
    name: 'Name',
    email: 'Email',
    status: 'Status',
    createdAt: 'Created At',
    actions: 'Actions',
    
    // Status
    active: 'Active',
    inactive: 'Inactive',
    
    // Pagination
    previous: 'Previous',
    next: 'Next',
    page: 'Page',
    of: 'of',
    
    // Forms
    addUser: 'Add New User',
    addRegion: 'Add New Region',
    addCategory: 'Add New Category',
    code: 'Code',
    codeRequired: 'Code is required',
    enterRegionCode: 'Enter region code',
    nameRequired: 'Name is required',
    enterRegionName: 'Enter region name',
    addSubcategory: 'Add Subcategory',
    subcategories: 'sub categories',
    
    // Welcome
    welcome: 'Welcome to',
    totalUsers: 'Total Users',
    totalRegions: 'Total Regions',
    totalCategories: 'Total Categories',
    toggleActivation: 'Toggle Activation',
    errorTogglingActivation: 'Error toggling activation',
    noParent: 'base category with no parent',
    
    // New Key for Categories Tree
    categoriesTree: 'Categories Tree',
    parentName: 'Parent Category',
    noDataAvailable: 'No data available',
  }
};

const LanguageContext = createContext<LanguageContextType | undefined>(undefined);

export const LanguageProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [language, setLanguage] = useState<Language>('ar');

  useEffect(() => {
    const savedLanguage = localStorage.getItem('language') as Language;
    if (savedLanguage && (savedLanguage === 'ar' || savedLanguage === 'en')) {
      setLanguage(savedLanguage);
    }
  }, []);

  useEffect(() => {
    localStorage.setItem('language', language);
    document.documentElement.setAttribute('dir', language === 'ar' ? 'rtl' : 'ltr');
    document.documentElement.setAttribute('lang', language);
  }, [language]);

  const t = (key: string): string => {
    return translations[language][key as keyof typeof translations['ar']] || key;
  };

  const isRTL = language === 'ar';

  return (
    <LanguageContext.Provider value={{ language, setLanguage, t, isRTL }}>
      {children}
    </LanguageContext.Provider>
  );
};

export const useLanguage = () => {
  const context = useContext(LanguageContext);
  if (context === undefined) {
    throw new Error('useLanguage must be used within a LanguageProvider');
  }
  return context;
};

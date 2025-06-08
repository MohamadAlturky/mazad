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
    dynamicAttributes: 'السمات الديناميكية',
    
    // Actions
    add: 'إضافة',
    edit: 'تعديل',
    delete: 'حذف',
    view: 'عرض',
    save: 'حفظ',
    cancel: 'إلغاء',
    search: 'بحث',
    openAll: 'فتح الكل',
    
    // Table Headers
    name: 'الاسم',
    nameEnglish: 'الاسم بالإنجليزية',
    nameArabic: 'الاسم بالعربية',
    email: 'البريد الإلكتروني',
    status: 'الحالة',
    createdAt: 'تاريخ الإنشاء',
    actions: 'الإجراءات',
    type: 'النوع',
    
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
    addAttribute: 'إضافة سمة جديدة',
    editAttribute: 'تعديل السمة',
    code: 'رمز',
    codeRequired: 'الرمز مطلوب',
    enterRegionCode: 'أدخل رمز المنطقة',
    nameRequired: 'الاسم مطلوب',
    enterRegionName: 'أدخل اسم المنطقة',
    enterName: 'أدخل الاسم',
    enterNameArabic: 'أدخل الاسم بالعربية',
    enterNameEnglish: 'أدخل الاسم بالإنجليزية',
    addSubcategory: 'إضافة فئة فرعية',
    editSubcategory: 'تعديل الفئة ',
    parentCategory: 'الفئة الأب',
    subcategories: 'الفئات الفرعية',
    selectType: 'اختر النوع',
    text: 'نص',
    number: 'رقم',
    boolean: 'قيمة منطقية',
    date: 'تاريخ',
    
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

    // Dynamic Attributes Messages
    errorFetchingAttributes: 'حدث خطأ أثناء جلب السمات',
    errorCreatingAttribute: 'حدث خطأ أثناء إنشاء السمة',
    errorUpdatingAttribute: 'حدث خطأ أثناء تحديث السمة',
    errorDeletingAttribute: 'حدث خطأ أثناء حذف السمة',
    attributeDeletedSuccessfully: 'تم حذف السمة بنجاح',
    attributeValueType: 'النوع',
    categoryAttributes: 'سمات الفئة',

    // Tooltips
    addNew: 'إضافة جديد',
    viewCategoriesTree: 'عرض شجرة الفئات',
    viewSubcategories: 'عرض الفئات الفرعية',
    viewAttributes: 'عرض السمات',
    deactivateItem: 'إلغاء تفعيل العنصر',
    activateItem: 'تفعيل العنصر',

    // Search
    searchAttributes: 'ابحث عن السمات...',
    noMatchingAttributes: 'لا توجد سمات مطابقة للبحث',
    resultsFound: 'نتيجة',
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
    dynamicAttributes: 'Dynamic Attributes',
    
    // Actions
    add: 'Add',
    edit: 'Edit',
    delete: 'Delete',
    view: 'View',
    save: 'Save',
    cancel: 'Cancel',
    search: 'Search',
    openAll: 'Open All',
    
    // Table Headers
    name: 'Name',
    nameEnglish: 'Name in English',
    nameArabic: 'Name in Arabic',
    email: 'Email',
    status: 'Status',
    createdAt: 'Created At',
    actions: 'Actions',
    type: 'Type',
    
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
    addAttribute: 'Add New Attribute',
    editAttribute: 'Edit Attribute',
    code: 'Code',
    codeRequired: 'Code is required',
    enterRegionCode: 'Enter region code',
    nameRequired: 'Name is required',
    enterRegionName: 'Enter region name',
    enterName: 'Enter name',
    enterNameArabic: 'Enter name in Arabic',
    enterNameEnglish: 'Enter name in English',
    addSubcategory: 'Add category',
    editSubcategory: 'Edit category',
    parentCategory: 'Parent Category',
    subcategories: 'sub categories',
    selectType: 'Select type',
    text: 'Text',
    number: 'Number',
    boolean: 'Boolean',
    date: 'Date',
    
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

    // Dynamic Attributes Messages
    errorFetchingAttributes: 'Error fetching attributes',
    errorCreatingAttribute: 'Error creating attribute',
    errorUpdatingAttribute: 'Error updating attribute',
    errorDeletingAttribute: 'Error deleting attribute',
    attributeDeletedSuccessfully: 'Attribute deleted successfully',
    attributeValueType: 'type',
    categoryAttributes: 'Category Attributes',

    // Tooltips
    addNew: 'Add New',
    viewCategoriesTree: 'View Categories Tree',
    viewSubcategories: 'View Subcategories',
    viewAttributes: 'View Attributes',
    deactivateItem: 'Deactivate Item',
    activateItem: 'Activate Item',

    // Search
    searchAttributes: 'Search attributes...',
    noMatchingAttributes: 'No matching attributes found',
    resultsFound: 'results found',
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

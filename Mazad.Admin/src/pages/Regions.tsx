import React, { useState } from 'react';
import { Badge } from '@/components/ui/badge';
import { useLanguage } from '@/contexts/LanguageContext';
import DataTable from '@/components/DataTable';
import Subcategories from '@/components/Subcategories';
import RegionForm from '@/components/RegionForm';
import AdminLayout from '@/components/AdminLayout';

const Regions: React.FC = () => {
  const { t } = useLanguage();
  const [viewMode, setViewMode] = useState<'list' | 'subcategories'>('list');
  const [selectedRegion, setSelectedRegion] = useState<any>(null);
  const [isFormOpen, setIsFormOpen] = useState(false);

  // Mock data
  const regions = [
    {
      id: 1,
      name: 'الرياض',
      status: 'active',
      createdAt: '2024-01-15',
    },
    {
      id: 2,
      name: 'جدة',
      status: 'active',
      createdAt: '2024-01-10',
    },
    {
      id: 3,
      name: 'الدمام',
      status: 'inactive',
      createdAt: '2024-01-08',
    },
    // Add more mock data
    ...Array.from({ length: 12 }, (_, i) => ({
      id: i + 4,
      name: `Region ${i + 4}`,
      status: i % 3 === 0 ? 'inactive' : 'active',
      createdAt: `2024-01-${String(Math.floor(Math.random() * 28) + 1).padStart(2, '0')}`,
    })),
  ];

  const columns = [
    { key: 'name', label: t('name') },
    {
      key: 'status',
      label: t('status'),
      render: (status: string) => (
        <Badge
          variant={status === 'active' ? 'default' : 'secondary'}
          className={status === 'active' ? 'bg-purple-100 text-purple-800' : 'bg-gray-100 text-gray-800'}
        >
          {t(status)}
        </Badge>
      ),
    }
  ];

  const handleAdd = () => {
    setIsFormOpen(true);
  };

  const handleFormSubmit = (data: any) => {
    console.log('Create new region:', data);
    // Here you would typically call an API to create the region
  };

  const handleEdit = (region: any) => {
    console.log('Edit region:', region);
  };

  const handleDelete = (region: any) => {
    console.log('Delete region:', region);
  };

  const handleView = (region: any) => {
    console.log('View region:', region);
  };

  const handleViewSubcategories = (region: any) => {
    setSelectedRegion(region);
    setViewMode('subcategories');
  };

  const handleBackToList = () => {
    setViewMode('list');
    setSelectedRegion(null);
  };

  return (
    <AdminLayout>
      {viewMode === 'list' ? (
        <>
          <DataTable
            title={t('regions')}
            columns={columns}
            data={regions}
            onAdd={handleAdd}
            onEdit={handleEdit}
            onDelete={handleDelete}
            onView={handleView}
            onViewSubcategories={handleViewSubcategories}
            addButtonText={t('addRegion')}
            showSubcategoriesAction={true}
          />
          <RegionForm
            open={isFormOpen}
            onOpenChange={setIsFormOpen}
            onSubmit={handleFormSubmit}
          />
        </>
      ) : (
        <Subcategories
          parent={selectedRegion}
          onBack={handleBackToList}
          type="region"
        />
      )}
    </AdminLayout>
  );
};

export default Regions;

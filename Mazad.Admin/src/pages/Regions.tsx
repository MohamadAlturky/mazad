import React, { useState } from 'react';
import { Badge } from '@/components/ui/badge';
import { useLanguage } from '@/contexts/LanguageContext';
import DataTable from '@/components/DataTable';
import Subcategories from '@/components/Subcategories';
import RegionForm from '@/components/RegionForm';
import AdminLayout from '@/components/AdminLayout';
import { BaseTable, Region, RegionFormData } from '@/types';

const Regions: React.FC = () => {
  const { t } = useLanguage();
  const [viewMode, setViewMode] = useState<'list' | 'subcategories'>('list');
  const [selectedRegion, setSelectedRegion] = useState<Region | null>(null);
  const [isFormOpen, setIsFormOpen] = useState(false);

  // Mock data
  const regions: Region[] = [
    {
      id: 1,
      name: 'الرياض',
      isActive: true,
      status: 'active',
      createdAt: '2024-01-15',
    },
    {
      id: 2,
      name: 'جدة',
      isActive: true,
      status: 'active',
      createdAt: '2024-01-10',
    },
    {
      id: 3,
      name: 'الدمام',
      isActive: false,
      status: 'inactive',
      createdAt: '2024-01-08',
    },
    // Add more mock data
    ...Array.from({ length: 12 }, (_, i) => ({
      id: i + 4,
      name: `Region ${i + 4}`,
      isActive: i % 3 === 0 ? false : true,
      status: i % 3 === 0 ? 'inactive' : 'active' as const,
      createdAt: `2024-01-${String(Math.floor(Math.random() * 28) + 1).padStart(2, '0')}`,
    })),
  ];

  const columns = [
    { key: 'name', label: t('name') },
    {
      key: 'status',
      label: t('status'),
      render: (status: Region['status']) => (
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

  const handleFormSubmit = (data: RegionFormData) => {
    console.log('Create new region:', data);
    // Here you would typically call an API to create the region
  };

  const handleEdit = (region: Region) => {
    console.log('Edit region:', region);
  };

  const handleDelete = (region: Region) => {
    console.log('Delete region:', region);
  };

  const handleView = (region: Region) => {
    console.log('View region:', region);
  };

  const handleViewSubcategories = (region: Region) => {
    setSelectedRegion(region);
    setViewMode('subcategories');
  };

  const handleBackToList = () => {
    setViewMode('list');
    setSelectedRegion(null);
  };

  return (
    <AdminLayout loading={false}>
      {viewMode === 'list' ? (
        <>
          <DataTable
            title={t('regions')}
            columns={columns}
            data={regions}
            onAdd={handleAdd}
            onEdit={handleEdit}
            onDelete={handleDelete}
            showtree={true}
            currentPage={1}
            onPageChange={() => {}}
            pageSize={10}
            onView={handleView}
            totalCount={regions.length}
            isLoading={false}
            onToggleActivation={() => {}}
            key={`regions-${viewMode}`}
            onViewSubcategories={handleViewSubcategories}
            addButtonText={t('addRegion')}
            showSubcategoriesAction={true}
          />
          {/* <RegionForm
            open={isFormOpen}
            onOpenChange={setIsFormOpen}
            onSubmit={handleFormSubmit}
          /> */}
        </>
      ) : (
        <></>
        // <Subcategories
        //   parent={selectedRegion}
        //   onBack={handleBackToList}
        //   type="region"
        // />
      )}
    </AdminLayout>
  );
};

export default Regions;

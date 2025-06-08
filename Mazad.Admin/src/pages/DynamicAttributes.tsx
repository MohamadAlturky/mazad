import React, { useState, useEffect } from 'react';
import { Badge } from '@/components/ui/badge';
import { useLanguage } from '@/contexts/LanguageContext';
import DataTable from '@/components/DataTable';
import DynamicAttributeForm, { DynamicAttributeFormData } from '@/components/DynamicAttributeForm';
import AdminLayout from '@/components/AdminLayout';
import axios from 'axios';
import { DynamicAttribute } from '../types';
import { toast } from 'sonner';
import EditDynamicAttributeForm, { EditDynamicAttributeFormData } from '@/components/EditDynamicAttributeForm';

const DynamicAttributes: React.FC = () => {
  const { t, language } = useLanguage();
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [attributes, setAttributes] = useState<DynamicAttribute[]>([]);
  const [totalCount, setTotalCount] = useState<number>(0);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [currentPage, setCurrentPage] = useState<number>(1);
  const [pageSize, setPageSize] = useState<number>(10);
  const [isRefetching, setIsRefetching] = useState<boolean>(false);
  const [editMode, setEditMode] = useState<boolean>(false);
  const [selectedAttribute, setSelectedAttribute] = useState<DynamicAttribute | null>(null);

  useEffect(() => {
    const fetchAttributes = async () => {
      setIsLoading(true);
      try {
        const response = await axios.get(`http://localhost:5032/api/dynamic-attributes`, {
          headers: {
            'Accept-Language': language,
          },
        });

        if (response.data.success) {
          setAttributes(response.data.data);
          setTotalCount(response.data.data.length);
        } else {
          toast.error(t('errorFetchingAttributes'));
        }
      } catch (error) {
        toast.error(t('errorFetchingAttributes'));
      } finally {
        setIsLoading(false);
      }
    };

    fetchAttributes();
  }, [language, t, isRefetching]);

  const columns = [
    { key: 'name', label: t('name') },
    { key: 'attributeValueTypeString', label: t('attributeValueType') },
    {
      key: 'isActive',
      label: t('status'),
      render: (status: boolean) => (
        <Badge
          variant={status === true ? 'default' : 'secondary'}
          className={status === true ? 'bg-purple-100 text-purple-800' : 'bg-gray-100 text-gray-800'}
        >
          {t(status ? 'active' : 'inactive')}
        </Badge>
      ),
    }
  ];

  const handleAdd = () => {
    setIsFormOpen(true);
  };

  const handleEditSubmit = (data: EditDynamicAttributeFormData) => {
    console.log('Updated attribute:', data);
    setIsRefetching(isRefetching => !isRefetching);
  };

  const handleFormSubmit = (data: DynamicAttributeFormData) => {
    console.log('Create new attribute:', data);
    setIsRefetching(isRefetching => !isRefetching);
  };

  const handleEdit = (attribute: DynamicAttribute) => {
    console.log('Edit attribute:', attribute);
    setEditMode(true);
    setSelectedAttribute(attribute);
  };

  const handleDelete = async (attribute: DynamicAttribute) => {
    try {
      setIsLoading(true);
      const response = await axios.delete('http://localhost:5032/api/dynamic-attributes', {
        headers: {
          'Accept-Language': language,
          'Content-Type': 'application/json',
        },
        data: {
          id: attribute.id,
        },
      });

      if (response.data.success) {
        setAttributes((prevAttributes) =>
          prevAttributes.filter((attr) => attr.id !== attribute.id)
        );
        toast.success(response.data.message || t('attributeDeletedSuccessfully'));
        setIsRefetching(isRefetching => !isRefetching);
      } else {
        toast.error(response.data.message);
      }
    } catch (error) {
      toast.error(t('errorDeletingAttribute'));
    } finally {
      setIsLoading(false);
    }
  };

  const handleView = (attribute: DynamicAttribute) => {
    console.log('View attribute:', attribute);
  };

  const handleToggleActivation = async (attribute: DynamicAttribute) => {
    try {
      setIsLoading(true);
      const response = await axios.put(`http://localhost:5032/api/dynamic-attributes/toggle-activation/${attribute.id}`, null, {
        headers: {
          'Accept-Language': language,
        },
      });
      if (response.data.success) {
        toast.success(response.data.message);
        setAttributes((prevAttributes) =>
          prevAttributes.map((attr) =>
            attr.id === attribute.id ? { ...attr, isActive: !attr.isActive } : attr
          )
        );
      } else {
        toast.error(t('errorTogglingActivation'));
      }
    } catch (error) {
      toast.error(t('errorTogglingActivation'));
    } finally {
      setIsLoading(false);
    }
  };

  const handlePageChange = (newPage: number) => {
    setCurrentPage(newPage);
  };

  return (
    <AdminLayout loading={isLoading}>
      <>
        <DataTable
          title={t('dynamicAttributes')}
          columns={columns}
          data={attributes}
          onAdd={handleAdd}
          onEdit={handleEdit}
          onDelete={handleDelete}
          onView={handleView}
          onToggleActivation={handleToggleActivation}
          addButtonText={t('addAttribute')}
          currentPage={currentPage}
          pageSize={pageSize}
          totalCount={totalCount}
          onPageChange={handlePageChange}
          showtree={false}
        />
        <DynamicAttributeForm
          open={isFormOpen}
          onOpenChange={setIsFormOpen}
          onSubmit={handleFormSubmit}
        />
      </>
      {editMode && (
        <EditDynamicAttributeForm
          open={editMode}
          onOpenChange={setEditMode}
          onSubmit={handleEditSubmit}
          id={selectedAttribute?.id}
        />
      )}
    </AdminLayout>
  );
};

export default DynamicAttributes; 
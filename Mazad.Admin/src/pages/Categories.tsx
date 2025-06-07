import React, { useState, useEffect } from 'react';
import { Badge } from '@/components/ui/badge';
import { useLanguage } from '@/contexts/LanguageContext';
import DataTable from '@/components/DataTable';
import CategoryForm from '@/components/CategoryForm';
import AdminLayout from '@/components/AdminLayout';
import axios from 'axios';
import { Category } from '../types'; // Assuming you have a Category type defined
import { CategoryFormData } from '@/components/CategoryForm';
import { toast } from 'sonner';
import { Link } from 'react-router-dom';

const Categories: React.FC = () => {
  const { t, language } = useLanguage();
  const [viewMode, setViewMode] = useState<'list' | 'subcategories'>('list');
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(null);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [categories, setCategories] = useState<Category[]>([]);
  const [totalCount, setTotalCount] = useState<number>(0);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [currentPage, setCurrentPage] = useState<number>(1);
  const [pageSize, setPageSize] = useState<number>(10);
  const [isRefetching, setIsRefetching] = useState<boolean>(false);

  useEffect(() => {
    const fetchCategories = async () => {
      console.log(currentPage, pageSize);
      setIsLoading(true);
      try {
        const response = await axios.get(`http://localhost:5032/api/categories/list?pageNumber=${currentPage}&pageSize=${pageSize}`, {
          headers: {
            'Accept-Language': language,
          },
        });

        if (response.data.success) {
          setCategories(response.data.data.categories);
          setTotalCount(response.data.data.totalCount);
          // toast.success(response.data.message || t('categoryFetchedSuccessfully'));
        } else {
          toast.error(t('errorFetchingCategories'));
        }
      } catch (error) {
        toast.error(t('errorFetchingCategories'));
      } finally {
        setIsLoading(false);
      }
    };

    fetchCategories();
  }, [language, t, currentPage, pageSize, isRefetching]);

  const columns = [
    { key: 'name', label: t('name') },
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

  const handleFormSubmit = (data: CategoryFormData) => {
    console.log('Create new category:', data);
    setIsRefetching(isRefetching => !isRefetching);
  };

  const handleEdit = (category: Category) => {
    console.log('Edit category:', category);
  };

  const handleDelete = async (category: Category) => {
    try {
      const response = await axios.delete('http://localhost:5032/api/categories', {
        headers: {
          'Accept-Language': language,
          'Content-Type': 'application/json',
        },
        data: {
          id: category.id,
        },
      });

      if (response.data.success) {
        setCategories((prevCategories) =>
          prevCategories.filter((cat) => cat.id !== category.id)
        );
        toast.success(response.data.message || t('categoryDeletedSuccessfully'));
        setIsRefetching(isRefetching => !isRefetching);
        // window.location.reload();
      } else {
        toast.error(response.data.message);
      }
    } catch (error) {
      toast.error(t('errorDeletingCategory'));
    }
  };

  const handleView = (category: Category) => {
    console.log('View category:', category);
  };

  const handleViewSubcategories = (category: Category) => {
    setSelectedCategory(category);
    setViewMode('subcategories');
  };

  const handleBackToList = () => {
    setViewMode('list');
    setSelectedCategory(null);
  };

  const handleToggleActivation = async (category: Category) => {
    try {
      const response = await axios.put(`http://localhost:5032/api/categories/toggle-activation/${category.id}`, null, {
        headers: {
          'Accept-Language': language,
        },
      });
      if (response.data.success) {
        toast.success(response.data.message);
        setCategories((prevCategories) =>
          prevCategories.map((cat) =>
            cat.id === category.id ? { ...cat, isActive: !cat.isActive } : cat
          )
        );
      } else {
        toast.error(t('errorTogglingActivation'));
      }
    } catch (error) {
      toast.error(t('errorTogglingActivation'));
    }
  };

  const handlePageChange = (newPage: number) => {
    setCurrentPage(newPage);
  };

  // Add a link to navigate to the Subcategories page
  const renderCategoryLink = (category: Category) => (
    <Link to={`/subcategories/${category.id}`} className="text-blue-500 hover:underline">
      {t('viewSubcategories')}
    </Link>
  );

  return (
    <AdminLayout>
      {isLoading ? (
        <div>{t('loading')}</div>
      ) : <>
        <DataTable
          title={t('categories')}
          columns={columns}
          data={categories}
          onAdd={handleAdd}
          onEdit={handleEdit}
          onDelete={handleDelete}
          onView={handleView}
          onViewSubcategories={(category) => {
            window.location.href = `/subcategories/${category.id}`;
          }}
          onToggleActivation={handleToggleActivation}
          addButtonText={t('addCategory')}
          showSubcategoriesAction={true}
          currentPage={currentPage}
          pageSize={pageSize}
          totalCount={totalCount}
          onPageChange={handlePageChange}
        />
        <CategoryForm
          open={isFormOpen}
          onOpenChange={setIsFormOpen}
          onSubmit={handleFormSubmit}
        />
      </>}
    </AdminLayout>
  );
};

export default Categories;
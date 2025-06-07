import React from 'react';
import { Badge } from '@/components/ui/badge';
import { useLanguage } from '@/contexts/LanguageContext';
import DataTable from '@/components/DataTable';
import AdminLayout from '@/components/AdminLayout';

const Users: React.FC = () => {
  const { t } = useLanguage();

  // Mock data
  const users = [
    {
      id: 1,
      name: 'أحمد محمد',
      email: 'ahmed@example.com',
      status: 'active',
      createdAt: '2024-01-15',
    },
    {
      id: 2,
      name: 'فاطمة علي',
      email: 'fatima@example.com',
      status: 'inactive',
      createdAt: '2024-01-10',
    },
    {
      id: 3,
      name: 'محمد خالد',
      email: 'mohamed@example.com',
      status: 'active',
      createdAt: '2024-01-08',
    },
    // Add more mock data
    ...Array.from({ length: 15 }, (_, i) => ({
      id: i + 4,
      name: `User ${i + 4}`,
      email: `user${i + 4}@example.com`,
      status: i % 2 === 0 ? 'active' : 'inactive',
      createdAt: `2024-01-${String(Math.floor(Math.random() * 28) + 1).padStart(2, '0')}`,
    })),
  ];

  const columns = [
    { key: 'name', label: t('name') },
    { key: 'email', label: t('email') },
    {
      key: 'status',
      label: t('status'),
      render: (status: string) => (
        <Badge
          variant={status === 'active' ? 'default' : 'secondary'}
          className={status === 'active' ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}
        >
          {t(status)}
        </Badge>
      ),
    },
    { key: 'createdAt', label: t('createdAt') },
  ];

  const handleAdd = () => {
    console.log('Add new user');
  };

  const handleEdit = (user: any) => {
    console.log('Edit user:', user);
  };

  const handleDelete = (user: any) => {
    console.log('Delete user:', user);
  };

  const handleView = (user: any) => {
    console.log('View user:', user);
  };

  return (
    <AdminLayout>
      <DataTable
        title={t('users')}
        columns={columns}
        data={users}
        onAdd={handleAdd}
        onEdit={handleEdit}
        onDelete={handleDelete}
        onView={handleView}
        addButtonText={t('addUser')}
      />
    </AdminLayout>
  );
};

export default Users;

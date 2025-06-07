import React, { useState } from 'react';
import { Edit, Trash2, Eye, Plus, Search, ChevronRight, ChevronLeft } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { useLanguage } from '@/contexts/LanguageContext';
import { cn } from '@/lib/utils';
import { BaseTable } from '@/types';

interface Column {
  key: string;
  label: string;
  align?: 'left' | 'center' | 'right';
  render?: (value: unknown, row: BaseTable) => React.ReactNode;
}

interface DataTableProps {
  title: string;
  columns: Column[];
  data: BaseTable[];
  onAdd?: () => void;
  onEdit?: (row: BaseTable) => void;
  onDelete?: (row: BaseTable) => void;
  onView?: (row: BaseTable) => void;
  onViewSubcategories?: (row: BaseTable) => void;
  addButtonText?: string;
  showSubcategoriesAction?: boolean;
  isLoading?: boolean;
  onToggleActivation?: (row: BaseTable) => void;
  currentPage: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (newPage: number) => void;
}

const Pagination: React.FC<{ currentPage: number; totalPages: number; onPageChange: (page: number) => void; }> = ({ currentPage, totalPages, onPageChange }) => {
  const { t } = useLanguage();

  return (
    <div className="flex items-center justify-between mt-4 pt-4 border-t border-purple-200">
      <div className="text-sm text-purple-600">
        {t('page')} {currentPage} {t('of')} {totalPages}
      </div>

      <div className="flex gap-2">
        <Button
          variant="outline"
          size="sm"
          onClick={() => onPageChange(currentPage - 1)}
          disabled={currentPage === 1}
          className="border-purple-200 text-purple-600 hover:bg-purple-50"
        >
          {t('previous')}
        </Button>

        {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
          const page = i + 1;
          return (
            <Button
              key={page}
              variant={currentPage === page ? "default" : "outline"}
              size="sm"
              onClick={() => onPageChange(page)}
              className={cn(
                currentPage === page
                  ? "bg-purple-600 text-white"
                  : "border-purple-200 text-purple-600 hover:bg-purple-50"
              )}
            >
              {page}
            </Button>
          );
        })}

        <Button
          variant="outline"
          size="sm"
          onClick={() => onPageChange(currentPage + 1)}
          disabled={currentPage === totalPages}
          className="border-purple-200 text-purple-600 hover:bg-purple-50"
        >
          {t('next')}
        </Button>
      </div>
    </div>
  );
};

const DataTable: React.FC<DataTableProps> = ({
  title,
  columns,
  data,
  onAdd,
  onEdit,
  onDelete,
  onView,
  onViewSubcategories,
  addButtonText,
  showSubcategoriesAction = false,
  isLoading = false,
  onToggleActivation,
  currentPage,
  pageSize,
  totalCount,
  onPageChange
}) => {
  const { t, isRTL, language } = useLanguage();
  const [searchTerm, setSearchTerm] = useState('');

  const filteredData = data.filter(item =>
    Object.values(item).some(value =>
      value?.toString().toLowerCase().includes(searchTerm.toLowerCase())
    )
  );

  const totalPages = Math.ceil(totalCount / pageSize);
  const startIndex = (currentPage - 1) * pageSize;
  const paginatedData = filteredData;

  const handlePageChange = (page: number) => {
    onPageChange(page);
  };

  const renderCell = (column: Column, row: BaseTable) => {
    if (column.render) {
      return column.render(row[column.key], row);
    }
    const value = row[column.key];
    if (value === null || value === undefined) {
      return '';
    }
    return String(value);
  };

  if (isLoading) {
    return (
      <div className="w-full overflow-x-auto">
        <table className="w-full">
          <thead>
            <tr className="border-b border-purple-200 bg-purple-50">
              {columns.map((column, idx) => (
                <th
                  key={column.key}
                  className={cn(
                    "py-3 px-5 font-semibold text-purple-700 whitespace-nowrap",
                    idx === 0 ? "min-w-[160px]" : "min-w-[120px]",
                    column.align === 'center' ? 'text-center' : isRTL ? 'text-right' : 'text-left'
                  )}
                >
                  {column.label}
                </th>
              ))}
              <th className={cn(
                "py-3 px-5 font-semibold text-purple-700 min-w-[140px] whitespace-nowrap",
                isRTL ? "text-right" : "text-center"
              )}>
                {t('actions')}
              </th>
            </tr>
          </thead>
          <tbody>
            {Array.from({ length: 5 }).map((_, index) => (
              <tr key={index} className="border-b border-purple-100">
                {columns.map((column) => (
                  <td
                    key={column.key}
                    className={cn(
                      "py-3 px-5 text-purple-900",
                      column.align === 'center' ? 'text-center' : isRTL ? 'text-right' : 'text-left'
                    )}
                  >
                    <div className="h-4 bg-purple-100 rounded animate-pulse" />
                  </td>
                ))}
                <td className="py-3 px-5 text-center">
                  <div className="h-4 bg-purple-100 rounded animate-pulse" />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    );
  }

  return (
    <Card className="border-purple-200">
      <CardHeader>
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
          <CardTitle className="text-purple-900">{title}</CardTitle>

          <div className="flex flex-col sm:flex-row gap-3 w-full sm:w-auto">
            <div className="relative">
              <Search className={cn("absolute top-3 h-4 w-4 text-purple-400", isRTL ? "right-3" : "left-3")} />
              <Input
                placeholder={t('search')}
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className={cn("border-purple-200 focus:border-purple-400", isRTL ? "pr-10" : "pl-10")}
              />
            </div>

            {onAdd && (
              <Button
                onClick={onAdd}
                className="bg-purple-600 hover:bg-purple-700 text-white"
              >
                <Plus className={cn("h-4 w-4", isRTL ? "ml-2" : "mr-2")} />
                {addButtonText || t('add')}
              </Button>
            )}
          </div>
        </div>
      </CardHeader>

      <CardContent>
        <div className="overflow-x-auto">
          <table className="w-full min-w-[600px]">
            <thead>
              <tr className="border-b border-purple-200 bg-purple-50">
                {columns.map((column, idx) => (
                  <th
                    key={column.key}
                    className={cn(
                      "py-3 px-5 font-semibold text-purple-700 whitespace-nowrap",
                      idx === 0 ? "min-w-[160px]" : "min-w-[120px]",
                      column.align === 'center' ? 'text-center' : isRTL ? 'text-right' : 'text-left'
                    )}
                  >
                    {column.label}
                  </th>
                ))}
                <th className={cn(
                  "py-3 px-5 font-semibold text-purple-700 min-w-[140px] whitespace-nowrap",
                  isRTL ? "text-center" : "text-center"
                )}>
                  {t('actions')}
                </th>
              </tr>
            </thead>
            <tbody>
              {paginatedData.map((row, index) => (
                <tr key={index} className="border-b border-purple-100 hover:bg-purple-50 transition-colors">
                  {columns.map((column, idx) => (
                    <td key={column.key} className={cn("py-3 px-5 align-middle whitespace-nowrap", idx === 0 ? "min-w-[160px]" : "min-w-[120px]", isRTL ? "text-right" : "text-left")}>
                      {renderCell(column, row)}
                    </td>
                  ))}
                  <td className={cn("py-3 px-5 align-middle min-w-[140px]", isRTL ? "text-center" : "text-center")}>
                    <div className="flex items-center justify-center gap-2">
                      {showSubcategoriesAction && onViewSubcategories && (
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => onViewSubcategories(row)}
                          className="text-purple-600 hover:bg-purple-100"
                        >
                          <Eye className="h-4 w-4" />

                          {/* {language === 'ar' ? <ChevronLeft className="h-4 w-4" /> : <ChevronRight className="h-4 w-4" />} */}

                        </Button>
                      )}
                      {/* {onEdit && (
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => onEdit(row)}
                          className="text-purple-600 hover:bg-purple-100"
                        >
                          <Edit className="h-4 w-4" />
                        </Button>
                      )} */}
                      {onDelete && (
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => onDelete(row)}
                          className="text-red-600 hover:bg-red-100"
                        >
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      )}
                      {onToggleActivation && (
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => onToggleActivation(row)}
                          className="text-yellow-600 hover:bg-yellow-100 w-24"
                        >
                          <span >{row.isActive ? t('deactivate') : t('activate')}</span>
                        </Button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {totalPages > 1 && (
          <Pagination
            currentPage={currentPage}
            totalPages={totalPages}
            onPageChange={handlePageChange}
          />
        )}
      </CardContent>
    </Card>
  );
};

export default DataTable;

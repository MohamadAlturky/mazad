import React, { useState } from 'react';
import { Edit, Trash2, Eye, Plus, Search, ChevronRight, ChevronLeft } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { useLanguage } from '@/contexts/LanguageContext';
import { cn } from '@/lib/utils';
import { BaseTable } from '@/types';
import { Link } from 'react-router-dom';

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
  showtree: boolean;
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
  onPageChange,
  showtree
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

            {/* New Link to /categories-tree */}
            {showtree && <>
              <Button asChild className="bg-purple-600 hover:bg-purple-700 text-white">
                <Link to="/categories-tree">
                  {t('categoriesTree')}
                </Link>
              </Button>
            </>}
          </div>
        </div>
      </CardHeader>

      <CardContent>
        <div className="overflow-x-auto">
          {paginatedData.length === 0 ? (
            <>
              <div className="flex flex-col justify-center items-center h-32 text-purple-600">
                <svg fill="#7c3aed" version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlnsXlink="http://www.w3.org/1999/xlink" width="100px" height="100px" viewBox="0 0 59.227 59.227" xmlSpace="preserve">

                  <g id="SVGRepo_bgCarrier" strokeWidth="0" />

                  <g id="SVGRepo_tracerCarrier" strokeLinecap="round" strokeLinejoin="round" />

                  <g id="SVGRepo_iconCarrier"> <g> <g> <path d="M51.586,10.029c-0.333-0.475-0.897-0.689-1.449-0.607c-0.021-0.005-0.042-0.014-0.063-0.017L27.469,6.087 c-0.247-0.037-0.499-0.01-0.734,0.076L8.63,12.799c-0.008,0.003-0.015,0.008-0.023,0.011c-0.019,0.008-0.037,0.02-0.057,0.027 c-0.099,0.044-0.191,0.096-0.276,0.157c-0.026,0.019-0.051,0.038-0.077,0.059c-0.093,0.076-0.178,0.159-0.249,0.254 c-0.004,0.006-0.01,0.009-0.014,0.015L0.289,23.78c-0.293,0.401-0.369,0.923-0.202,1.391c0.167,0.469,0.556,0.823,1.038,0.947 l6.634,1.713v16.401c0,0.659,0.431,1.242,1.062,1.435l24.29,7.422c0.008,0.004,0.017,0.001,0.025,0.005 c0.13,0.036,0.266,0.059,0.402,0.06c0.003,0,0.007,0.002,0.011,0.002l0,0h0.001c0.143,0,0.283-0.026,0.423-0.067 c0.044-0.014,0.085-0.033,0.13-0.052c0.059-0.022,0.117-0.038,0.175-0.068l17.43-9.673c0.477-0.265,0.772-0.767,0.772-1.312 V25.586l5.896-2.83c0.397-0.19,0.69-0.547,0.802-0.973c0.111-0.427,0.03-0.88-0.223-1.241L51.586,10.029z M27.41,9.111 l17.644,2.59L33.35,17.143l-18.534-3.415L27.41,9.111z M9.801,15.854l21.237,3.914l-6.242,9.364l-20.78-5.365L9.801,15.854z M10.759,43.122V28.605l14.318,3.697c0.125,0.031,0.25,0.048,0.375,0.048c0.493,0,0.965-0.244,1.248-0.668l5.349-8.023v25.968 L10.759,43.122z M49.479,41.1l-14.431,8.007V25.414l2.635,5.599c0.171,0.361,0.479,0.641,0.854,0.773 c0.163,0.06,0.333,0.087,0.502,0.087c0.223,0,0.444-0.05,0.649-0.146l9.789-4.698L49.479,41.1L49.479,41.1z M39.755,28.368 l-4.207-8.938L49.85,12.78l5.634,8.037L39.755,28.368z" /> </g> </g> </g>

                </svg>
                {t('noDataAvailable')}
              </div>
            </>
          ) : (
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
                        {onEdit && (
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => onEdit(row)}
                            className="text-purple-600 hover:bg-purple-100"
                          >
                            <Edit className="h-4 w-4" />
                          </Button>
                        )}
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
          )}
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

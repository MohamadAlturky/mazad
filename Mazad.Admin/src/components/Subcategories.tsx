import React, { useState, useEffect } from 'react';
import { ArrowLeft, Plus, ChevronDown, ChevronRight, Edit } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { useLanguage } from '@/contexts/LanguageContext';
import { cn } from '@/lib/utils';
import EditSubcategoryForm from './EditSubcategoryForm';
import { useParams } from 'react-router-dom';
import AdminLayout from './AdminLayout';

interface Parent {
  id: number;
  name: string;
  // Add other properties if needed
}

interface SubcategoryItem {
  id: number;
  name: string;
  nameArabic: string;
  nameEnglish: string;
  isActive: boolean;
  children?: SubcategoryItem[];
}

const Subcategories: React.FC = () => {
  const { id } = useParams();
  const { t, isRTL, language } = useLanguage();
  const [expandedItems, setExpandedItems] = useState<Set<number>>(new Set());
  const [subcategories, setSubcategories] = useState<SubcategoryItem[]>([]);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [selectedSubcategory, setSelectedSubcategory] = useState<SubcategoryItem | null>(null);

  useEffect(() => {
    const fetchSubcategories = async () => {
      try {
        const response = await fetch(`http://localhost:5032/api/categories/tree/${id}`, {
          headers: {
            'Accept-Language': language,
          },
        });
        const data = await response.json();
        if (data.success) {
          setSubcategories(data.data);
        }
      } catch (error) {
        console.error('Error fetching subcategories:', error);
      }
    };

    fetchSubcategories();
  }, [language, id]);

  const toggleExpanded = (id: number) => {
    const newExpanded = new Set(expandedItems);
    if (newExpanded.has(id)) {
      newExpanded.delete(id);
    } else {
      newExpanded.add(id);
    }
    setExpandedItems(newExpanded);
  };

  const handleEdit = (item: SubcategoryItem) => {
    setSelectedSubcategory(item);
    setEditDialogOpen(true);
  };

  const handleEditSubmit = (data: { nameArabic: string; nameEnglish: string; }) => {
    // Update the subcategory in the state or refetch the data
    console.log('Updated subcategory:', data);
    window.location.reload();
  };

  const renderSubcategoryItem = (item: SubcategoryItem, level: number = 0) => {
    const hasChildren = item.children && item.children.length > 0;
    const isExpanded = expandedItems.has(item.id);
    const paddingValue = level * 24;

    // Use paddingRight for RTL, paddingLeft for LTR
    const itemStyle = isRTL
      ? { paddingRight: `${paddingValue + 16}px` }
      : { paddingLeft: `${paddingValue + 16}px` };

    return (
      <div key={item.id}>
        <div
          className="flex items-center justify-between py-3 px-4 border-b border-purple-100 hover:bg-purple-50 transition-colors"
          style={itemStyle}
        >
          <div className="flex items-center gap-3">
            {hasChildren ? (
              <button
                onClick={() => toggleExpanded(item.id)}
                className="text-purple-600 hover:text-purple-800"
              >
                {isExpanded ? (
                  <ChevronDown className={cn("h-4 w-4", isRTL ? "rotate-180" : "")} />
                ) : (
                  <ChevronRight className={cn("h-4 w-4", isRTL ? "rotate-180" : "")} />
                )}
              </button>
            ) : (
              <div className="w-4" />
            )}
            <span className="font-medium text-purple-900">{item.name}</span>
            <Button
              variant="ghost"
              size="sm"
              onClick={() => handleEdit(item)}
              className="text-purple-600 hover:bg-purple-50 ml-2"
            >
              <Edit className={cn("h-4 w-4", isRTL ? "ml-2" : "mr-2")} />
            </Button>
          </div>

          <Badge
            variant={item.isActive === true ? 'default' : 'secondary'}
            className={item.isActive === true ? 'bg-purple-100 text-purple-800' : 'bg-gray-100 text-gray-800'}
          >
            {t(item.isActive === true ? 'active' : 'inactive')}
          </Badge>
        </div>

        {hasChildren && isExpanded && (
          <div>
            {item.children!.map(child => renderSubcategoryItem(child, level + 1))}
          </div>
        )}
      </div>
    );
  };

  return (
    <AdminLayout>

    <Card className="border-purple-200">
      <CardHeader>
        <CardTitle className="text-purple-900"> {t('subcategories')} </CardTitle>
      </CardHeader>

      <CardContent>
        <div className="space-y-1">
          {subcategories.map(item => renderSubcategoryItem(item))}
        </div>
      </CardContent>

      {selectedSubcategory && (
        <EditSubcategoryForm
          open={editDialogOpen}
          onOpenChange={setEditDialogOpen}
          onSubmit={handleEditSubmit}
          subcategory={selectedSubcategory}
        />
      )}
    </Card>
    </AdminLayout>
  );
};

export default Subcategories;

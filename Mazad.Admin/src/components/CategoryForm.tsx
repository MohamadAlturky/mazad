import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useLanguage } from '@/contexts/LanguageContext';
import axios from 'axios';

import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from '@/components/ui/select';
import { BaseTable } from '@/types';
import { toast } from 'sonner';

export interface CategoryFormData {
  nameArabic: string;
  nameEnglish: string;
  parentCategoryId?: number | null;
}

interface CategoryFormProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: CategoryFormData) => void;
}

interface Category extends BaseTable {
  id: number;
  name: string;
}

const CategoryForm: React.FC<CategoryFormProps> = ({ open, onOpenChange, onSubmit }) => {
  const { t, language } = useLanguage();
  const { register, handleSubmit, reset, setValue, formState: { errors } } = useForm<CategoryFormData>();
  const [categories, setCategories] = useState<Category[]>([]);
  const [openCombobox, setOpenCombobox] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(null);
  const [searchText, setSearchText] = useState('');

  useEffect(() => {
    axios.get('http://localhost:5032/api/categories/dropdown', {
      headers: {
        'Accept-Language': language
      }
    })
      .then(response => {
        setCategories(response.data.data);
      })
      .catch(error => {
        console.error('Error fetching categories:', error);
      });
  }, [language]);

  useEffect(() => {
    setValue('parentCategoryId', selectedCategory ? selectedCategory.id : null);
  }, [selectedCategory, setValue]);

  const handleFormSubmit = (data: CategoryFormData) => {
    const requestData = {
      nameArabic: data.nameArabic,
      nameEnglish: data.nameEnglish,
      parentId: selectedCategory?.id || null
    };

    axios.post('http://localhost:5032/api/categories', requestData, {
      headers: {
        'Accept-Language': language,
        'Content-Type': 'application/json'
      }
    })
      .then(response => {
        if (response.data.success) {
          toast.success(response.data.message);
        }
        else {
          toast.error(response.data.message);
        }
        console.log('Category created successfully:', response.data);
        onSubmit({ ...data, parentCategoryId: selectedCategory?.id || null });
        reset();
        setSelectedCategory(null);
        setSearchText('');
        onOpenChange(false);
      })
      .catch(error => {
        console.error('Error creating category:', error);
      });
  };

  const handleClearSelection = (e: React.MouseEvent) => {
    e.stopPropagation();
    setSelectedCategory(null);
    // Don't close the combobox, allow re-selection immediately
  };

  const handleClearSearch = () => {
    setSearchText('');
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px] bg-white p-6 rounded-lg shadow-lg">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold text-purple-800">{t('addRegion')}</DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-6">
          <div className="space-y-2">
            <Label htmlFor="nameArabic" className="text-purple-700 text-base font-medium">{t('nameArabic')}</Label>
            <Input
              id="nameArabic"
              {...register('nameArabic', { required: t('nameArabicRequired') })}
              className="border-purple-300 focus:border-purple-500 focus:ring-purple-500 rounded-md p-2 text-gray-800"
              placeholder={t('enterNameArabic')}
            />
            {errors.nameArabic && (
              <p className="text-red-600 text-sm mt-1">{errors.nameArabic.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="nameEnglish" className="text-purple-700 text-base font-medium">{t('nameEnglish')}</Label>
            <Input
              id="nameEnglish"
              {...register('nameEnglish', { required: t('nameEnglishRequired') })}
              className="border-purple-300 focus:border-purple-500 focus:ring-purple-500 rounded-md p-2 text-gray-800"
              placeholder={t('enterNameEnglish')}
            />
            {errors.nameEnglish && (
              <p className="text-red-600 text-sm mt-1">{errors.nameEnglish.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="parentCategory" className="text-purple-700 text-base font-medium">{t('parentCategory')}</Label>
            <Select onValueChange={(value) => {
              const category = categories.find(cat => cat.id.toString() === value);
              setSelectedCategory(category || null);
            }}>
              <SelectTrigger className="w-full justify-between border-purple-300 text-purple-700 hover:bg-purple-100 focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all duration-200 ease-in-out rounded-md p-2">
                <SelectValue placeholder={t('selectParentCategory')} />
              </SelectTrigger>
              <SelectContent className="bg-white">
                <SelectGroup>
                  <SelectLabel>{t('categories')}</SelectLabel>
                  <SelectItem value="none" onSelect={() => setSelectedCategory(null)}>{language === 'ar' ? 'فئة اساسية بدون أب' : 'base category with no parent'}</SelectItem>
                  {categories.map((category) => (
                    <SelectItem key={category.id} value={category.id.toString()}>{category.name}</SelectItem>
                  ))}
                </SelectGroup>
              </SelectContent>
            </Select>
          </div>

          <DialogFooter className="flex justify-end space-x-3 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={() => {
                onOpenChange(false);
                reset();
                setSelectedCategory(null);
                setSearchText('');
              }}
              className="border-purple-300 text-purple-700 hover:bg-purple-100 px-4 py-2 rounded-md transition-colors duration-200 ml-3"
            >
              {t('cancel')}
            </Button>
            <Button
              type="submit"
              className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-md transition-colors duration-200"
            >
              {t('create')}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
};

export default CategoryForm;
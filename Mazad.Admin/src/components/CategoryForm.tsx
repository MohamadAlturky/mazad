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
import { Riple } from 'react-loading-indicators';

export interface CategoryFormData {
  nameArabic: string;
  nameEnglish: string;
  parentId: number | null;
}

interface CategoryFormProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: CategoryFormData) => void;
  categoriesList: Category[];
}

interface Category extends BaseTable {
  id: number;
  name: string;
}

const CategoryForm: React.FC<CategoryFormProps> = ({ open, onOpenChange, onSubmit, categoriesList }) => {
  const { t, language } = useLanguage();
  const { register, handleSubmit, reset, setValue, formState: { errors } } = useForm<CategoryFormData>();
  const [categories, setCategories] = useState<Category[]>([]);
  const [openCombobox, setOpenCombobox] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(null);
  const [searchText, setSearchText] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    setIsLoading(true);
    axios.get('http://localhost:5032/api/categories/dropdown', {
      headers: {
        'Accept-Language': language
      }
    })
      .then(response => {
        const fetchedCategories = response.data.data;
        const newCategory = { id: null, name: t('noParent') };
        setCategories([...fetchedCategories, newCategory]);
      })
      .catch(error => {
        console.error('Error fetching categories:', error);
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, [language, setIsLoading, t, categoriesList]);

  // useEffect(() => {
  //   setValue('parentCategory', selectedCategory ? selectedCategory.id : null);
  //   console.log('selectedCategory', selectedCategory);
  // }, [selectedCategory, setValue]);

  const handleFormSubmit = (data: CategoryFormData) => {
    const requestData = {
      nameArabic: data.nameArabic,
      nameEnglish: data.nameEnglish,
      parentId: data.parentId
    };
    setIsLoading(true);
    console.log('requestData', requestData);

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
        onSubmit({ ...data });
        reset();
        setSelectedCategory(null);
        setSearchText('');
        onOpenChange(false);
      })
      .catch(error => {
        console.error('Error creating category:', error);
      })
      .finally(() => {
        setIsLoading(false);
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
      <DialogContent className="sm:max-w-[425px] bg-card p-6 rounded-lg shadow-lg">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold text-foreground">{t('addRegion')}</DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-6">
          <div className="space-y-2">
            <Label htmlFor="nameArabic" className="text-foreground text-base font-medium">{t('nameArabic')}</Label>
            <Input
              id="nameArabic"
              {...register('nameArabic', { required: t('nameArabicRequired') })}
              className="border-border focus:border-primary focus:ring-primary rounded-md p-2 text-foreground bg-background"
              placeholder={t('enterNameArabic')}
            />
            {errors.nameArabic && (
              <p className="text-destructive text-sm mt-1">{errors.nameArabic.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="nameEnglish" className="text-foreground text-base font-medium">{t('nameEnglish')}</Label>
            <Input
              id="nameEnglish"
              {...register('nameEnglish', { required: t('nameEnglishRequired') })}
              className="border-border focus:border-primary focus:ring-primary rounded-md p-2 text-foreground bg-background"
              placeholder={t('enterNameEnglish')}
            />
            {errors.nameEnglish && (
              <p className="text-destructive text-sm mt-1">{errors.nameEnglish.message}</p>
            )}
          </div>

          <div className="space-y-2 w-full">
            <Label htmlFor="parentCategory" className="w-full text-foreground text-lg font-semibold tracking-wide">
              {t('parentCategory')}
            </Label>
            <Select onValueChange={(value) => {
              setValue('parentId', value ? parseInt(value) : null);
              console.log('parentCategoryId', value);
            }}>
              <SelectTrigger className={`w-full 
                text-center 
                border-2 
                border-border 
                rounded-lg 
                py-2 
                px-4 
                text-foreground 
                bg-background
                transition-all 
                duration-200 
                ease-in-out
                hover:border-primary 
                focus:outline-none 
                focus:ring-2 
                focus:ring-primary 
                focus:border-transparent 
                ${language === 'ar' ? 'text-right' : 'text-left'}
              `}>
                <SelectValue placeholder={t('noParent')} />
              </SelectTrigger>
              <SelectContent
                className={`
                  bg-card 
                  rounded-lg 
                  shadow-xl 
                  border 
                  border-border 
                  mt-1 
                  ${language === 'ar' ? 'text-right' : 'text-left'}
                `}
              >
                <SelectGroup className={`bg-card ${language === 'ar' ? 'text-right' : 'text-left'}`}>
                  <SelectLabel className="px-4 py-2 text-primary font-medium text-sm border-b border-border">
                    {t('categories')}
                  </SelectLabel>
                  {categories.map((category) => (
                    <SelectItem
                      key={category.id}
                      value={category.id != null ? category.id.toString() : null}
                      className={`flex py-2 px-8 cursor-pointer text-foreground transition-colors duration-150 ease-in-out hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground outline-none ${language === 'ar' ? 'justify-end' : 'justify-start'}`}>
                      {category.name}
                    </SelectItem>
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
              className="border-border text-foreground hover:bg-accent px-4 py-2 rounded-md transition-colors duration-200 ml-3"
            >
              {t('cancel')}
            </Button>
            {isLoading ? (
              <Button
                type="submit"
                className="bg-primary hover:bg-primary/90 text-primary-foreground px-4 py-2 rounded-md transition-colors duration-200"
              >
                {t('loading')} ...
              </Button>
            ) : (
              <Button
                type="submit"
                className="bg-primary hover:bg-primary/90 text-primary-foreground px-4 py-2 rounded-md transition-colors duration-200"
              >
                {t('create')}
              </Button>
            )}
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
};

export default CategoryForm;
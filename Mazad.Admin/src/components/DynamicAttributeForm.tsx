import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useLanguage } from '@/contexts/LanguageContext';
import axios from 'axios';
import { toast } from 'sonner';
import { Riple } from 'react-loading-indicators';

export interface DynamicAttributeFormData {
  nameArabic: string;
  nameEnglish: string;
  attributeValueType: number;
}

export interface DynamicAttributeFormProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: DynamicAttributeFormData) => void;
}

const DynamicAttributeForm: React.FC<DynamicAttributeFormProps> = ({ open, onOpenChange, onSubmit }) => {
  const [isLoading, setIsLoading] = useState(false);
  const { t, language } = useLanguage();
  const { register, handleSubmit, reset, setValue, watch, formState: { errors } } = useForm<DynamicAttributeFormData>({
    defaultValues: {
      nameArabic: '',
      nameEnglish: '',
      attributeValueType: 1,
    }
  });

  const attributeValueType = watch('attributeValueType');

  const handleFormSubmit = (data: DynamicAttributeFormData) => {
    setIsLoading(true);
    axios.post(`http://localhost:5032/api/dynamic-attributes`, data, {
      headers: {
        'Accept-Language': language,
        'Content-Type': 'application/json'
      }
    })
      .then(response => {
        if (response.data.success) {
          toast.success(response.data.message);
        } else {
          toast.error(response.data.message);
        }
        onSubmit(data);
        reset();
        onOpenChange(false);
      })
      .catch(error => {
        console.error('Error creating attribute:', error);
        toast.error(t('errorCreatingAttribute'));
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px] bg-white p-6 rounded-lg shadow-lg">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold text-purple-800">{t('addAttribute')}</DialogTitle>
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

          <div className="space-y-2 w-full">
            <Label htmlFor="attributeValueType" className="w-full text-purple-800 text-lg font-semibold tracking-wide">
              {t('type')}
            </Label>
            <Select
              value={attributeValueType.toString()}
              onValueChange={(value) => setValue('attributeValueType', parseInt(value))}
            >
              <SelectTrigger className={`w-full 
                text-center 
                border-2 
                border-gray-300 
                rounded-lg 
                py-2 
                px-4 
                text-gray-700 
                transition-all 
                duration-200 
                ease-in-out
                hover:border-purple-600 
                focus:outline-none 
                focus:ring-2 
                focus:ring-purple-500 
                focus:border-transparent 
                ${language === 'ar' ? 'text-right' : 'text-left'}
              `}>
                <SelectValue placeholder={t('selectType')} />
              </SelectTrigger>
              <SelectContent className={`
                bg-white 
                rounded-lg 
                shadow-xl 
                border 
                border-gray-200 
                mt-1 
                ${language === 'ar' ? 'text-right' : 'text-left'}
              `}>
                <SelectGroup className={`bg-white ${language === 'ar' ? 'text-right' : 'text-left'}`}>
                  <SelectLabel className="px-4 py-2 text-purple-600 font-medium text-sm border-b border-gray-100">
                    {t('types')}
                  </SelectLabel>
                  <SelectItem value="1" className={`flex py-2 px-8 cursor-pointer text-gray-800 transition-colors duration-150 ease-in-out hover:bg-purple-100 hover:text-purple-800 focus:bg-purple-100 focus:text-purple-800 outline-none ${language === 'ar' ? 'justify-end' : 'justify-start'}`}>
                    {t('text')}
                  </SelectItem>
                  <SelectItem value="2" className={`flex py-2 px-8 cursor-pointer text-gray-800 transition-colors duration-150 ease-in-out hover:bg-purple-100 hover:text-purple-800 focus:bg-purple-100 focus:text-purple-800 outline-none ${language === 'ar' ? 'justify-end' : 'justify-start'}`}>
                    {t('number')}
                  </SelectItem>
                  <SelectItem value="3" className={`flex py-2 px-8 cursor-pointer text-gray-800 transition-colors duration-150 ease-in-out hover:bg-purple-100 hover:text-purple-800 focus:bg-purple-100 focus:text-purple-800 outline-none ${language === 'ar' ? 'justify-end' : 'justify-start'}`}>
                    {t('boolean')}
                  </SelectItem>
                </SelectGroup>
              </SelectContent>
            </Select>
            {errors.attributeValueType && (
              <p className="text-red-600 text-sm mt-1">{errors.attributeValueType.message}</p>
            )}
          </div>

          <DialogFooter className="flex justify-end space-x-3 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={() => {
                onOpenChange(false);
                reset();
              }}
              className="border-purple-300 text-purple-700 hover:bg-purple-100 px-4 py-2 rounded-md transition-colors duration-200 ml-3"
            >
              {t('cancel')}
            </Button>
            {isLoading ? (
              <Button
                type="submit"
                className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-md transition-colors duration-200"
              >
                {t('loading')} ...
              </Button>
            ) : (
              <Button
                type="submit"
                className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-md transition-colors duration-200"
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

export default DynamicAttributeForm;
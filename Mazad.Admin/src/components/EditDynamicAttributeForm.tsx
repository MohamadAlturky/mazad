import React, { useState, useEffect } from 'react';
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

export interface EditDynamicAttributeFormData {
  nameArabic: string;
  nameEnglish: string;
  attributeValueType: number;
}

export interface EditDynamicAttributeFormProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: EditDynamicAttributeFormData) => void;
  id?: number;
}

const EditDynamicAttributeForm: React.FC<EditDynamicAttributeFormProps> = ({ open, onOpenChange, onSubmit, id }) => {
  const [isLoading, setIsLoading] = useState(false);
  const { t, language } = useLanguage();
  const { register, handleSubmit, reset, setValue, watch, formState: { errors } } = useForm<EditDynamicAttributeFormData>({
    defaultValues: {
      nameArabic: '',
      nameEnglish: '',
      attributeValueType: 1,
    }
  });

  const attributeValueType = watch('attributeValueType');

  useEffect(() => {
    if (id) {
      const fetchAttribute = async () => {
        try {
          const response = await axios.get(`http://localhost:5032/api/dynamic-attributes/${id}`, {
            headers: {
              'Accept-Language': language,
            },
          });

          if (response.data.success) {
            const attribute = response.data.data;
            setValue('nameArabic', attribute.nameArabic);
            setValue('nameEnglish', attribute.nameEnglish);
            setValue('attributeValueType', attribute.attributeValueType);
          }
        } catch (error) {
          console.error('Error fetching attribute:', error);
          toast.error(t('errorFetchingAttribute'));
        }
      };

      fetchAttribute();
    }
  }, [id, language, setValue, t]);

  const handleFormSubmit = (data: EditDynamicAttributeFormData) => {
    const requestData = {
      id: id,
      nameArabic: data.nameArabic,
      nameEnglish: data.nameEnglish,
      attributeValueType: data.attributeValueType,
    };

    setIsLoading(true);
    axios.put(`http://localhost:5032/api/dynamic-attributes`, requestData, {
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
        console.error('Error updating attribute:', error);
        toast.error(t('errorUpdatingAttribute'));
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px] bg-card p-6 rounded-lg shadow-lg">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold text-foreground">{t('editAttribute')}</DialogTitle>
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
            <Label htmlFor="attributeValueType" className="w-full text-foreground text-lg font-semibold tracking-wide">
              {t('type')}
            </Label>
            <Select
              value={attributeValueType.toString()}
              onValueChange={(value) => setValue('attributeValueType', parseInt(value))}
            >
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
                <SelectValue placeholder={t('selectType')} />
              </SelectTrigger>
              <SelectContent className={`
                bg-card 
                rounded-lg 
                shadow-xl 
                border 
                border-border 
                mt-1 
                ${language === 'ar' ? 'text-right' : 'text-left'}
              `}>
                <SelectGroup className={`bg-card ${language === 'ar' ? 'text-right' : 'text-left'}`}>
                  <SelectLabel className="px-4 py-2 text-primary font-medium text-sm border-b border-border">
                    {t('types')}
                  </SelectLabel>
                  <SelectItem value="1" className={`flex py-2 px-8 cursor-pointer text-foreground transition-colors duration-150 ease-in-out hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground outline-none ${language === 'ar' ? 'justify-end' : 'justify-start'}`}>
                    {t('text')}
                  </SelectItem>
                  <SelectItem value="2" className={`flex py-2 px-8 cursor-pointer text-foreground transition-colors duration-150 ease-in-out hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground outline-none ${language === 'ar' ? 'justify-end' : 'justify-start'}`}>
                    {t('number')}
                  </SelectItem>
                  <SelectItem value="3" className={`flex py-2 px-8 cursor-pointer text-foreground transition-colors duration-150 ease-in-out hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground outline-none ${language === 'ar' ? 'justify-end' : 'justify-start'}`}>
                    {t('boolean')}
                  </SelectItem>
                </SelectGroup>
              </SelectContent>
            </Select>
            {errors.attributeValueType && (
              <p className="text-destructive text-sm mt-1">{errors.attributeValueType.message}</p>
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
                {t('save')}
              </Button>
            )}
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
};

export default EditDynamicAttributeForm;
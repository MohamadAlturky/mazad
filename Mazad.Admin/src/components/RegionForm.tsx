
import React from 'react';
import { useForm } from 'react-hook-form';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useLanguage } from '@/contexts/LanguageContext';

interface RegionFormData {
  name: string;
  code: string;
}

interface RegionFormProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: RegionFormData) => void;
}

const RegionForm: React.FC<RegionFormProps> = ({ open, onOpenChange, onSubmit }) => {
  const { t } = useLanguage();
  const { register, handleSubmit, reset, formState: { errors } } = useForm<RegionFormData>();

  const handleFormSubmit = (data: RegionFormData) => {
    onSubmit(data);
    reset();
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px] bg-white">
        <DialogHeader>
          <DialogTitle className="text-purple-900">{t('addRegion')}</DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="name" className="text-purple-700">{t('name')}</Label>
            <Input
              id="name"
              {...register('name', { required: 'Name is required' })}
              className="border-purple-200 focus:border-purple-400"
              placeholder="Enter region name"
            />
            {errors.name && (
              <p className="text-red-500 text-sm">{errors.name.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="code" className="text-purple-700">Code</Label>
            <Input
              id="code"
              {...register('code', { required: 'Code is required' })}
              className="border-purple-200 focus:border-purple-400"
              placeholder="Enter region code"
            />
            {errors.code && (
              <p className="text-red-500 text-sm">{errors.code.message}</p>
            )}
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
              className="border-purple-200 text-purple-600 hover:bg-purple-50"
            >
              {t('cancel')}
            </Button>
            <Button
              type="submit"
              className="bg-purple-600 hover:bg-purple-700 text-white"
            >
              {t('create')}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
};

export default RegionForm;

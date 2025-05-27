import React from 'react';
import { Button } from '@/components/ui/button';
import { Separator } from '@/components/ui/separator';

export type ReportCategory = 'deposits' | 'creditPrograms';

interface ReportSidebarProps {
  selectedCategory: ReportCategory | null;
  onCategorySelect: (category: ReportCategory) => void;
  onReset: () => void;
}

export const ReportSidebar = ({
  selectedCategory,
  onCategorySelect,
  onReset,
}: ReportSidebarProps) => {
  return (
    <div className="w-64 border-r bg-muted/10 p-4">
      <div className="space-y-4">
        <div>
          <h3 className="text-lg font-semibold mb-3">Категории отчетов</h3>
          <div className="space-y-2">
            <Button
              variant={selectedCategory === 'deposits' ? 'default' : 'outline'}
              className="w-full justify-start"
              onClick={() => onCategorySelect('deposits')}
            >
              Отчеты по депозитам
            </Button>
            <Button
              variant={
                selectedCategory === 'creditPrograms' ? 'default' : 'outline'
              }
              className="w-full justify-start"
              onClick={() => onCategorySelect('creditPrograms')}
            >
              Отчеты по кредитным программам
            </Button>
          </div>
        </div>

        <Separator />

        <Button
          variant="secondary"
          className="w-full"
          onClick={onReset}
          disabled={!selectedCategory}
        >
          Сбросить выбор
        </Button>
      </div>
    </div>
  );
};

import React from 'react';
import { Button } from '@/components/ui/button';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import { Label } from '@/components/ui/label';
import { FileText, FileSpreadsheet } from 'lucide-react';

type ReportCategory = 'pdf' | 'word-excel' | null;

interface ReportSidebarProps {
  selectedCategory: ReportCategory;
  onCategoryChange: (category: ReportCategory) => void;
}

export const ReportSidebar = ({
  selectedCategory,
  onCategoryChange,
}: ReportSidebarProps): React.JSX.Element => {
  return (
    <div className="w-80 border-r bg-background">
      <div className="space-y-4 p-4">
        <div>
          <h3 className="mb-4 text-lg font-medium">Категории отчетов</h3>
          <RadioGroup
            value={selectedCategory || ''}
            onValueChange={(value) => onCategoryChange(value as ReportCategory)}
          >
            <div className="flex items-center space-x-2">
              <RadioGroupItem value="pdf" id="pdf" />
              <Label
                htmlFor="pdf"
                className="flex items-center gap-2 cursor-pointer"
              >
                <FileText className="h-4 w-4" />
                PDF отчет по валютам и периодам
              </Label>
            </div>
            <div className="flex items-center space-x-2">
              <RadioGroupItem value="word-excel" id="word-excel" />
              <Label
                htmlFor="word-excel"
                className="flex items-center gap-2 cursor-pointer"
              >
                <FileSpreadsheet className="h-4 w-4" />
                Word/Excel отчет по кредитным программам
              </Label>
            </div>
          </RadioGroup>
        </div>

        {selectedCategory && (
          <div className="pt-4 border-t">
            <Button
              variant="outline"
              onClick={() => onCategoryChange(null)}
              className="w-full"
            >
              Сбросить выбор
            </Button>
          </div>
        )}
      </div>
    </div>
  );
};

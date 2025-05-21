import React from 'react';
import type { SelectedReport } from '../pages/Reports';
import { Button } from '../ui/button';
import { PdfViewer } from './PdfViewer';
import { WordViewer } from './WordViewer';
import { ExcelViewer } from './ExcelViewer';

type ReportViewerProps = {
  selectedReport: SelectedReport;
};

export const ReportViewer = ({
  selectedReport,
}: ReportViewerProps): React.JSX.Element => {
  return (
    <div className="w-full">
      <div className="flex gap-10">
        <Button>Сгенерировать</Button>
        <Button>Сохранить</Button>
        <Button>Отправить</Button>
      </div>
      <div>
        {selectedReport === 'pdf' && (
          <PdfViewer reportType={'clientsByCreditProgram'} />
        )}
        {selectedReport === 'word' && (
          <WordViewer reportType={'clientsByCreditProgram'} />
        )}
        {selectedReport === 'excel' && (
          <ExcelViewer reportType={'clientsByCreditProgram'} />
        )}
        {!selectedReport && <>не выбран отчет</>}
      </div>
    </div>
  );
};

import React from 'react';
import { ReportSidebar } from '../layout/ReportSidebar';
import { ReportViewer } from '../features/ReportViewer';

export type SelectedReport = 'word' | 'pdf' | 'excel' | undefined;

export const Reports = (): React.JSX.Element => {
  const [selectedReport, setSelectedReport] = React.useState<SelectedReport>();

  return (
    <main className="flex">
      <ReportSidebar
        onWordClick={() => setSelectedReport('word')}
        onPdfClick={() => setSelectedReport('pdf')}
        onExcelClick={() => setSelectedReport('excel')}
      />
      <ReportViewer selectedReport={selectedReport} />
    </main>
  );
};

import React from 'react';
import DocViewer, { DocViewerRenderers } from 'react-doc-viewer';
import { useReports } from '@/hooks/useReports';
import { type ReportType, type ReportParams } from '@/api/client';

interface ExcelViewerProps {
  reportType: ReportType;
  params?: ReportParams;
}

export const ExcelViewer = ({ reportType, params }: ExcelViewerProps) => {
  const { excelReport, isExcelLoading, isExcelError, excelError } = useReports(
    reportType,
    params,
  );
  const [documents, setDocuments] = React.useState<
    { uri: string; fileType: string }[]
  >([]);

  React.useEffect(() => {
    if (excelReport?.blob) {
      const uri = URL.createObjectURL(excelReport.blob);
      setDocuments([{ uri, fileType: 'xlsx' }]);
      return () => URL.revokeObjectURL(uri);
    }
  }, [excelReport]);

  if (isExcelLoading) return <div className="p-4">Загрузка Excel...</div>;
  if (isExcelError)
    return (
      <div className="p-4 text-red-500">Ошибка: {excelError?.message}</div>
    );

  return (
    <div className="p-4">
      {documents.length > 0 && (
        <DocViewer
          documents={documents}
          pluginRenderers={DocViewerRenderers}
          style={{ height: '500px' }}
        />
      )}
    </div>
  );
};

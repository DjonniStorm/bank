import React from 'react';
import DocViewer, { DocViewerRenderers } from 'react-doc-viewer';
import { useReports } from '@/hooks/useReports';
import { type ReportType, type ReportParams } from '@/api/client';

interface PdfViewerProps {
  reportType: ReportType;
  params?: ReportParams;
}

export const PdfViewer = ({ reportType, params }: PdfViewerProps) => {
  const { pdfReport, isPdfLoading, isPdfError, pdfError } = useReports(
    reportType,
    params,
  );
  const [documents, setDocuments] = React.useState<
    { uri: string; fileType: string }[]
  >([]);

  React.useEffect(() => {
    if (pdfReport?.blob) {
      const uri = URL.createObjectURL(pdfReport.blob);
      setDocuments([{ uri, fileType: 'pdf' }]);
      return () => URL.revokeObjectURL(uri);
    }
  }, [pdfReport]);

  if (isPdfLoading) return <div className="p-4">Загрузка PDF...</div>;
  if (isPdfError)
    return <div className="p-4 text-red-500">Ошибка: {pdfError?.message}</div>;

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

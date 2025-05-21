import React from 'react';
import DocViewer, { DocViewerRenderers } from 'react-doc-viewer';
import { useReports } from '@/hooks/useReports';
import { type ReportType, type ReportParams } from '@/api/client';

interface WordViewerProps {
  reportType: ReportType;
  params?: ReportParams;
}

export const WordViewer = ({ reportType, params }: WordViewerProps) => {
  const { wordReport, isWordLoading, isWordError, wordError } = useReports(
    reportType,
    params,
  );
  const [documents, setDocuments] = React.useState<
    { uri: string; fileType: string }[]
  >([]);

  React.useEffect(() => {
    if (wordReport?.blob) {
      const uri = URL.createObjectURL(wordReport.blob);
      setDocuments([{ uri, fileType: 'docx' }]);
      return () => URL.revokeObjectURL(uri);
    }
  }, [wordReport]);

  if (isWordLoading) return <div className="p-4">Загрузка Word...</div>;
  if (isWordError)
    return <div className="p-4 text-red-500">Ошибка: {wordError?.message}</div>;

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

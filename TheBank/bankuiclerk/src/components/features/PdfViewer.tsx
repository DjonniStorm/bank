import React from 'react';
import { Document, Page, pdfjs } from 'react-pdf';
import 'react-pdf/dist/esm/Page/AnnotationLayer.css';
import 'react-pdf/dist/esm/Page/TextLayer.css';
import { Button } from '../ui/button';

// Настройка worker для PDF.js
pdfjs.GlobalWorkerOptions.workerSrc = `https://cdn.jsdelivr.net/npm/pdfjs-dist@${pdfjs.version}/build/pdf.worker.mjs`;

interface PdfViewerProps {
  report: { blob: Blob; fileName: string; mimeType: string } | undefined | null;
}

export const PdfViewer = ({ report }: PdfViewerProps) => {
  const [numPages, setNumPages] = React.useState<number | null>(null);
  const [pageNumber, setPageNumber] = React.useState(1);
  const [pdfUrl, setPdfUrl] = React.useState<string | null>(null);
  const [error, setError] = React.useState<string | null>(null);

  React.useEffect(() => {
    if (report?.blob) {
      const url = URL.createObjectURL(report.blob);
      setPdfUrl(url);
      setError(null);

      return () => {
        URL.revokeObjectURL(url);
      };
    } else {
      setPdfUrl(null);
      setNumPages(null);
      setPageNumber(1);
    }
  }, [report]);

  const onDocumentLoadSuccess = ({ numPages }: { numPages: number }) => {
    setNumPages(numPages);
    setPageNumber(1);
    setError(null);
  };

  const onDocumentLoadError = (error: Error) => {
    console.error('Ошибка загрузки PDF:', error);
    setError(
      'Ошибка при загрузке PDF документа. Пожалуйста, попробуйте снова.',
    );
  };

  const handlePrevPage = () => {
    setPageNumber((prev) => Math.max(prev - 1, 1));
  };

  const handleNextPage = () => {
    setPageNumber((prev) => Math.min(prev + 1, numPages || 1));
  };

  if (!pdfUrl) {
    return (
      <div className="p-4 text-center">
        {report
          ? 'Подготовка PDF для отображения...'
          : 'Нет данных для отображения PDF.'}
      </div>
    );
  }

  return (
    <div className="p-4">
      <Document
        file={pdfUrl}
        onLoadSuccess={onDocumentLoadSuccess}
        onLoadError={onDocumentLoadError}
        loading={<div className="text-center py-4">Загрузка PDF...</div>}
        error={
          <div className="text-center text-red-500 py-4">
            Не удалось загрузить PDF
          </div>
        }
      >
        <Page
          pageNumber={pageNumber}
          renderTextLayer={false}
          renderAnnotationLayer={false}
          scale={1.2}
          loading={<div className="text-center py-2">Загрузка страницы...</div>}
          error={
            <div className="text-center text-red-500 py-2">
              Ошибка загрузки страницы
            </div>
          }
        />
      </Document>

      {error ? (
        <div className="text-red-500 py-2 text-center">{error}</div>
      ) : numPages ? (
        <div className="flex justify-between items-center mt-4">
          <Button onClick={handlePrevPage} disabled={pageNumber <= 1}>
            Предыдущая
          </Button>
          <p className="text-sm text-muted-foreground">
            Страница {pageNumber} из {numPages}
          </p>
          <Button onClick={handleNextPage} disabled={pageNumber >= numPages}>
            Следующая
          </Button>
        </div>
      ) : (
        <div className="text-center py-2 text-muted-foreground">
          Загрузка документа...
        </div>
      )}
    </div>
  );
};

import React from 'react';
import { Document, Page } from 'react-pdf';
import * as pdfjs from 'pdfjs-dist';
import 'react-pdf/dist/esm/Page/AnnotationLayer.css';
import 'react-pdf/dist/esm/Page/TextLayer.css';
import { Button } from '../ui/button';

// Используем встроенный worker
pdfjs.GlobalWorkerOptions.workerSrc = `//unpkg.com/pdfjs-dist@${pdfjs.version}/build/pdf.worker.min.js`;

interface PdfViewerProps {
  report: { blob: Blob; fileName: string; mimeType: string } | undefined | null;
}

export const PdfViewer = ({ report }: PdfViewerProps) => {
  const [numPages, setNumPages] = React.useState<number | null>(null);
  const [pageNumber, setPageNumber] = React.useState(1);
  const [pdfUrl, setPdfUrl] = React.useState<string | undefined>(undefined);
  const [error, setError] = React.useState<string | null>(null);

  // Создаем URL для Blob при изменении report
  React.useEffect(() => {
    if (report?.blob) {
      const url = URL.createObjectURL(report.blob);
      setPdfUrl(url);
      setError(null);
      // Очищаем URL при размонтировании компонента или изменении report
      return () => URL.revokeObjectURL(url);
    } else {
      setPdfUrl(undefined);
    }
  }, [report]);

  const onDocumentLoadSuccess = ({ numPages }: { numPages: number }) => {
    setNumPages(numPages);
    setPageNumber(1);
  };

  const handlePrevPage = () => {
    setPageNumber((prev) => Math.max(prev - 1, 1));
  };

  const handleNextPage = () => {
    setPageNumber((prev) => Math.min(prev + 1, numPages || 1));
  };

  if (!pdfUrl) {
    return (
      <div className="p-4">Загрузка или нет данных для отображения PDF.</div>
    );
  }

  return (
    <div className="p-4">
      <Document
        file={pdfUrl}
        onLoadSuccess={onDocumentLoadSuccess}
        onLoadError={(error) => {
          console.error('Ошибка загрузки PDF:', error);
          setError(
            'Ошибка при загрузке PDF документа. Пожалуйста, попробуйте снова.',
          );
        }}
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
        <div className="text-red-500 py-2">{error}</div>
      ) : (
        <div className="flex justify-between items-center mt-4">
          <Button onClick={handlePrevPage} disabled={pageNumber <= 1}>
            Предыдущая
          </Button>
          <p>
            Страница {pageNumber} из {numPages || 1}
          </p>
          <Button
            onClick={handleNextPage}
            disabled={pageNumber >= (numPages || 1)}
          >
            Следующая
          </Button>
        </div>
      )}
    </div>
  );
};

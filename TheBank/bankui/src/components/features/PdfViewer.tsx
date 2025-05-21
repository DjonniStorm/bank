import React from 'react';
import { Document, Page, pdfjs } from 'react-pdf';
import 'react-pdf/dist/esm/Page/AnnotationLayer.css';
import 'react-pdf/dist/esm/Page/TextLayer.css';

pdfjs.GlobalWorkerOptions.workerSrc = `//unpkg.com/pdfjs-dist@${pdfjs.version}/build/pdf.worker.min.js`;

interface PdfViewerProps {
  report: { blob: Blob; fileName: string; mimeType: string } | undefined | null;
}

export const PdfViewer = ({ report }: PdfViewerProps) => {
  const [numPages, setNumPages] = React.useState<number | null>(null);
  const [pageNumber, setPageNumber] = React.useState(1);
  const [pdfUrl, setPdfUrl] = React.useState<string | undefined>(undefined);

  // Создаем URL для Blob при изменении report
  React.useEffect(() => {
    if (report?.blob) {
      const url = URL.createObjectURL(report.blob);
      setPdfUrl(url);
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
        onLoadError={console.error}
      >
        {Array.from(new Array(numPages || 0), (el, index) => (
          <Page key={`page_${index + 1}`} pageNumber={index + 1} />
        ))}
      </Document>
      <p>
        Страница {pageNumber} из {numPages}
      </p>
    </div>
  );
};

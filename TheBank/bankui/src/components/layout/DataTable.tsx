import React from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '../ui/table';
import { Checkbox } from '../ui/checkbox';

type DataTableProps<T> = {
  data: T[];
  columns: ColumnDef<T>[];
};

export type ColumnDef<T> = {
  accessorKey: keyof T | string;
  header: string;
  renderCell?: (item: T) => React.ReactNode;
};

export const DataTable = <T extends {}>({
  data,
  columns,
}: DataTableProps<T>): React.JSX.Element => {
  const [selectedRowId, setSelectedRowId] = React.useState<
    string | undefined
  >();

  const handleRowSelect = (id: string) => {
    setSelectedRowId((prev) => (prev === id ? undefined : id));
  };
  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead className="w-[50px]"></TableHead>
            {columns.map((column) => (
              <TableHead key={column.accessorKey as string}>
                {column.header}
              </TableHead>
            ))}
          </TableRow>
        </TableHeader>
        <TableBody>
          {data.length === 0 ? (
            <TableRow>
              <TableCell
                colSpan={columns.length + 1}
                className="h-24 text-center"
              >
                Нет данных
              </TableCell>
            </TableRow>
          ) : (
            data.map((item, index) => (
              <TableRow
                key={(item as any).id || index}
                data-state={
                  selectedRowId === (item as any).id ? 'selected' : undefined
                }
              >
                <TableCell>
                  <Checkbox
                    checked={selectedRowId === (item as any).id}
                    onCheckedChange={() => handleRowSelect((item as any).id)}
                    aria-label="Select row"
                  />
                </TableCell>
                {columns.map((column) => (
                  <TableCell key={column.accessorKey as string}>
                    {column.renderCell
                      ? column.renderCell(item)
                      : (item as any)[column.accessorKey] ?? 'N/A'}
                  </TableCell>
                ))}
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </div>
  );
};

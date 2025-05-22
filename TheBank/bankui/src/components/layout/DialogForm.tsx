import React from 'react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '../ui/dialog';

type DialogFormProps<T> = {
  children: React.ReactElement<{ onSubmit: (data: T) => void }>;
  title: string;
  description: string;
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: T) => void;
};

export const DialogForm = <T,>({
  title,
  description,
  children,
  isOpen,
  onClose,
  onSubmit,
}: DialogFormProps<T>): React.JSX.Element => {
  console.log(onSubmit);
  const wrappedSubmit = (data: T) => {
    onSubmit(data);
    onClose();
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
          <DialogDescription>{description}</DialogDescription>
        </DialogHeader>
        {React.cloneElement(children, { onSubmit: wrappedSubmit })}
      </DialogContent>
    </Dialog>
  );
};

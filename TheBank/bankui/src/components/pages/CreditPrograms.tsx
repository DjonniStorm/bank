import { useQuery } from '@tanstack/react-query';
import React from 'react';

const getCreditPrograms = async () => {
  const res = await fetch(
    ' https://localhost:7204/api/creditPrograms/getAllRecords',
    {
      mode: 'no-cors',
    },
  );
  if (!res.ok) {
    throw new Error('cannot get');
  }
  const data = await res.json();
  return data;
};

const useCreditPrograms = () => {
  const { data, isError, isPending, isSuccess, isFetched } = useQuery({
    queryKey: ['credit-programs'],
    queryFn: getCreditPrograms,
  });

  return { data, isError, isPending, isSuccess, isFetched };
};

export const CreditPrograms = (): React.JSX.Element => {
  const cp = useCreditPrograms();
  console.log(cp);
  return (
    <main className="flex-1 w-full">
      {cp.isPending && <>loading</>}
      {cp.isFetched && cp.isError && <>empty list</>}
      {cp.isSuccess && <></>}
      <div className="flex gap-10">
        <div>+</div>
        <div>изменить</div>
      </div>
    </main>
  );
};

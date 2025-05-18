import React from 'react';
import { Link } from 'react-router-dom';
import {
  Menubar,
  MenubarContent,
  MenubarItem,
  MenubarMenu,
  MenubarSeparator,
  MenubarTrigger,
} from '../ui/menubar';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '../ui/dropdown-menu';
import { Avatar, AvatarFallback } from '../ui/avatar';
import { Button } from '../ui/button';

type NavOptionValue = {
  name: string;
  link: string;
  id: number;
};

type NavOption = {
  name: string;
  options: NavOptionValue[];
};

const navOptions = [
  {
    name: 'Валюты',
    options: [
      {
        id: 1,
        name: 'Просмотреть',
        link: '/currencies',
      },
      {
        id: 2,
        name: 'Создать',
        link: '',
      },
    ],
  },
  {
    name: 'Кредитные программы',
    options: [
      {
        id: 1,
        name: 'Просмотреть',
        link: '/credit-programs',
      },
      {
        id: 2,
        name: 'Создать',
        link: '',
      },
    ],
  },
  {
    name: 'Сроки',
    options: [
      {
        id: 1,
        name: 'Просмотреть',
        link: '/periods',
      },
      {
        id: 2,
        name: 'Создать',
        link: '',
      },
    ],
  },
  {
    name: 'Кладовщики',
    options: [
      {
        id: 1,
        name: 'Просмотреть',
        link: '/storekeepers',
      },
    ],
  },
];

export const Header = (): React.JSX.Element => {
  return (
    <header className="flex w-full p-2 justify-between">
      <nav className="text-black">
        <Menubar className="flex gap-10">
          {navOptions.map((item) => (
            <MenuOption item={item} key={item.name} />
          ))}
        </Menubar>
      </nav>
      <div>
        <ProfileIcon name={'Евгений Эгов'} />
      </div>
    </header>
  );
};

const MenuOption = ({ item }: { item: NavOption }) => {
  return (
    <MenubarMenu>
      <MenubarTrigger className="">{item.name}</MenubarTrigger>
      <MenubarContent className="">
        {item.options.map((x, i) => (
          <React.Fragment key={x.id}>
            {i == 1 && item.options.length > 1 && <MenubarSeparator />}
            <MenubarItem className="">
              <Link className="" to={x.link}>
                {x.name}
              </Link>
            </MenubarItem>
          </React.Fragment>
        ))}
        <MenubarSeparator />
      </MenubarContent>
    </MenubarMenu>
  );
};

type ProfileIconProps = {
  name: string;
};

export const ProfileIcon = ({ name }: ProfileIconProps): React.JSX.Element => {
  return (
    <div>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Avatar className="h-9 w-9">
            <AvatarFallback>{name[0]}</AvatarFallback>
          </Avatar>
        </DropdownMenuTrigger>
        <DropdownMenuContent className="w-56">
          <DropdownMenuItem className="font-bold text-lg">
            {name}
          </DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem asChild>
            <Link to="/" className="block w-full text-left">
              Профиль
            </Link>
          </DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem asChild>
            <Button variant="outline" className="block w-full text-left">
              Выйти
            </Button>
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  );
};

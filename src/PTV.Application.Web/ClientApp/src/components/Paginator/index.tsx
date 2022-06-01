import React from 'react';
import { usePagination } from '@mui/lab';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { Icon } from 'suomifi-ui-components';

const useStyles = makeStyles({
  ul: {
    listStyle: 'none',
    padding: 0,
    margin: 0,
    display: 'flex',
  },
  paginationItem: {
    display: 'flex',
    borderRadius: 0,
  },
  paginationButton: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: 0,
    fontSize: '16px',
    padding: '8px',
    minWidth: '38px',
    height: '38px',
    fontWeight: 'normal',
    color: 'rgb(42, 110, 187)',
    borderLeft: '0px',
    borderTop: '1px solid rgb(200, 205, 208)',
    borderBottom: '1px solid rgb(200, 205, 208)',
    borderRight: '1px solid rgb(200, 205, 208)',
    backgroundColor: 'rgb(255, 255, 255)',
    '&:focus': {
      outline: 'none',
      border: '1px solid rgb(43, 110, 187)',
      borderRadius: 0,
    },
  },
  firstButton: {
    borderLeft: '1px solid rgb(200, 205, 208)',
  },
  selected: {
    color: 'rgb(255, 255, 255)',
    border: '1px solid rgb(200, 205, 208)',
    fontWeight: 600,
    backgroundColor: 'rgb(43, 110, 187)',
  },
});

type PaginatorProps = {
  currentPage: number;
  pageCount: number;
  onChange: (page: number) => void;
};

export default function Paginator(props: PaginatorProps): React.ReactElement | null {
  const classes = useStyles();

  const { items } = usePagination({
    count: props.pageCount,
    onChange: onChange,
    page: props.currentPage,
    disabled: props.pageCount === 1,
  });

  if (props.pageCount < 1) {
    return null;
  }

  function onChange(_event: React.ChangeEvent<unknown>, page: number) {
    props.onChange(page);
  }

  return (
    <nav>
      <ul className={classes.ul}>
        {items.map(({ page, type, selected, ...item }, index) => {
          let children = null;

          const buttonClassName = clsx(classes.paginationButton, {
            [classes.selected]: selected === true,
            [classes.firstButton]: index === 0,
          });

          if (type === 'start-ellipsis' || type === 'end-ellipsis') {
            children = (
              <button className={buttonClassName} type='button' {...item} disabled={true}>
                {`…`}
              </button>
            );
          } else if (type === 'page') {
            children = (
              <button className={buttonClassName} type='button' {...item}>
                {page}
              </button>
            );
          } else if (type === 'previous' || type === 'next') {
            children = (
              <button className={buttonClassName} type='button' {...item}>
                {type === 'previous' ? (
                  <Icon icon='chevronLeft' fill='rgb(165, 173, 177)' />
                ) : (
                  <Icon icon='chevronRight' fill='rgb(165, 173, 177)' />
                )}
              </button>
            );
          } else {
            children = (
              <button className={buttonClassName} type='button' {...item}>
                {type}
              </button>
            );
          }

          return (
            <li className={classes.paginationItem} key={index}>
              {children}
            </li>
          );
        })}
      </ul>
    </nav>
  );
}
